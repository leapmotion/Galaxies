using Leap.Unity.Gestures;
using Leap.Unity.Query;
using Leap.Unity.RuntimeGizmos;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class GizmoHands : MonoBehaviour, IRuntimeGizmoComponent {

    public LeapProvider provider;
    [Tooltip("Used for rendering camera-facing circles.")]
    public Camera facingCamera;
    public Color handColor;

    [Header("TRS Mode Gizmo Support")]
    public LeapTRS2 leapTRS2;
    public PinchGesture leftPinch;
    public PinchGesture rightPinch;

    [Header("Mode Gizmo Colors")]
    public Color ineligibleGizmoColor = Color.gray.Lerp(Color.black, 0.5f);
    public Color eligibleGizmoColor = Color.gray;
    public Color activeGizmoColor = Color.white;
    public float gizmoColorLerpSpeed = 10f;
    private Color _rightGizmoColor = Color.gray;
    private Color _leftGizmoColor = Color.gray;

    [Header("Pinch Orb")]
    public Color inactivePinchOrbColor = Color.black;
    public Color activePinchOrbcolor = Color.white;
    public float maxPinchOrbRadius = 0.02f;
    public float pinchOrbLerpSpeed = 20f;
    private float _leftPinchOrbRadius = 0f;
    private float _rightPinchOrbRadius = 0f;

    private void Reset() {
      if (provider == null) provider = Hands.Provider;
      if (facingCamera == null) facingCamera = Camera.main;
    }
    private void Start() {
      if (provider == null) provider = Hands.Provider;
      if (facingCamera == null) facingCamera = Camera.main;
    }

    public void OnDrawRuntimeGizmos(RuntimeGizmoDrawer drawer) {
      Hand left = null, right = null;
      if (provider != null) {
        var frame = provider.CurrentFrame;
        left = frame.Hands.Query().FirstOrDefault(h => h.IsLeft);
        right = frame.Hands.Query().FirstOrDefault(h => h.IsRight);
      }
      if (left != null) {
        renderGizmoHand(left, leftPinch, ref _leftGizmoColor, ref _leftPinchOrbRadius,
          drawer);
      }
      if (right != null) {
        renderGizmoHand(right, rightPinch, ref _rightGizmoColor, ref _rightPinchOrbRadius,
          drawer);
      }
    }

    private Quaternion _trsGizmoRotator = Quaternion.AngleAxis(90f, Vector3.up);
    private void renderGizmoHand(Hand hand, PinchGesture pinchGesture,
      ref Color currentGizmoColor, ref float pinchOrbRadius, RuntimeGizmoDrawer drawer) {
      drawer.color = handColor;

      // Wrist.
      var wristPos = hand.WristPosition.ToVector3();
      var wristRadius = hand.GetPinky().Length * 0.5f;
      var wristNormal = hand.Arm.Basis.yBasis.ToVector3();
      var wristOffsetAlongNormal = hand.GetIndex().bones[3].Length;
      wristPos += wristNormal * wristOffsetAlongNormal;
      drawCircle(wristPos, wristRadius, wristNormal, drawer);

      // Palm.
      var palmPos = hand.PalmPosition.ToVector3();
      var palmRadius = hand.GetMiddle().Length * 0.5f;
      var palmNormal = hand.PalmarAxis() * -1f;
      var palmOffsetAlongNormal = hand.GetIndex().bones[3].Length * 2f;
      palmPos += palmNormal * palmOffsetAlongNormal;
      drawCircle(palmPos, palmRadius, palmNormal, drawer);

      // Fingers.
      for (int fIdx = 0; fIdx < hand.Fingers.Count; fIdx++) {
        var finger = hand.Fingers[fIdx];
        var radius = finger.bones[3].Length * 0.5f;

        for (int bId = 1; bId < finger.bones.Length; bId++) {
          var bone = finger.bones[bId];
          
          var boneNormal = bone.Basis.yBasis.ToVector3();
          var offsetAlongNormal = radius;

          var bonePos = bone.NextJoint.ToVector3() + boneNormal * offsetAlongNormal;

          drawCircle(bonePos, radius, boneNormal, drawer, 16);
        }
      }

      // TRS gizmos.
      if (leapTRS2 != null && leapTRS2.isEnabledAndConfigured) {

        // Wrist gizmo color.
        Color targetGizmoColor;
        if (!pinchGesture.isEligible) {
          targetGizmoColor = ineligibleGizmoColor;
        } else if (pinchGesture.isActive) {
          targetGizmoColor = activeGizmoColor;
        } else {
          targetGizmoColor = eligibleGizmoColor;
        }
        currentGizmoColor = currentGizmoColor.Lerp(targetGizmoColor,
          gizmoColorLerpSpeed * Time.deltaTime);
        drawer.color = currentGizmoColor;

        // Wrist gizmo.
        var wristRot = hand.Arm.Basis.rotation.ToQuaternion();
        var stemLength = wristRadius * 0.55f;
        var stemVector = stemLength * Vector3.forward;
        for (int i = 0; i < 4; i++) {
          var stemEnd = wristPos + wristRot * stemVector;
          drawer.DrawLine(wristPos, stemEnd);

          var arrowLength = stemLength * 0.33f;
          var arrowPoint = stemEnd + wristRot * Vector3.right * arrowLength
            + wristRot * Vector3.back * arrowLength;
          drawer.DrawLine(arrowPoint, stemEnd);
          arrowPoint = stemEnd + wristRot * Vector3.left * arrowLength
            + wristRot * Vector3.back * arrowLength;
          drawer.DrawLine(arrowPoint, stemEnd);

          wristRot = wristRot * _trsGizmoRotator;
        }

        float targetPinchOrbRadius = 0f;
        if (pinchGesture != null && pinchGesture.isActive) {
          targetPinchOrbRadius = maxPinchOrbRadius;
        }
        pinchOrbRadius = Mathf.Lerp(pinchOrbRadius, targetPinchOrbRadius,
          pinchOrbLerpSpeed * Time.deltaTime);
        var pinchOrbColor = Color.Lerp(inactivePinchOrbColor, activePinchOrbcolor,
          pinchOrbRadius.Map(0f, maxPinchOrbRadius, 0f, 1f));
        if (pinchOrbRadius > 0.0001f) {
          drawer.color = pinchOrbColor;
          drawer.DrawSphere(hand.GetPinchPosition(), pinchOrbRadius);
        }
      }
    }

    private void drawCircle(Vector3 pos, float radius,
                            RuntimeGizmoDrawer drawer, int resolution = 27) {
      var dirToCam = (facingCamera.transform.position - pos).normalized;
      var radialStartDir = dirToCam.Perpendicular();

      drawer.DrawWireArc(pos, dirToCam, radialStartDir, radius, 1f, resolution);
    }
    private void drawCircle(Vector3 pos, float radius, Vector3 normal,
                            RuntimeGizmoDrawer drawer, int resolution = 27) {
      var radialStartDir = normal.Perpendicular();

      drawer.DrawWireArc(pos, normal, radialStartDir, radius, 1f, resolution);
    }

  }

}
