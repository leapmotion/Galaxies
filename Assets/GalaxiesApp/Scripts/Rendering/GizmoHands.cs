using Leap.Unity.Query;
using Leap.Unity.RuntimeGizmos;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class GizmoHands : MonoBehaviour, IRuntimeGizmoComponent {

    public LeapProvider provider;
    [Tooltip("Used for rendering camera-facing circles.")]
    public Camera facingCamera;
    public Color color = Color.white;

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

      if (left != null || right != null) {
        drawer.color = color;
      }

      if (left != null) {
        renderGizmoHand(left, drawer);
      }
      if (right != null) {
        renderGizmoHand(right, drawer);
      }
    }

    private void renderGizmoHand(Hand hand, RuntimeGizmoDrawer drawer) {
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

      for (int fIdx = 0; fIdx < hand.Fingers.Count; fIdx++) {

      }
    }
    
    private void drawCircle(Vector3 pos, float radius,
                            RuntimeGizmoDrawer drawer) {
      var dirToCam = (facingCamera.transform.position - pos).normalized;
      var radialStartDir = dirToCam.Perpendicular();

      drawer.DrawWireArc(pos, dirToCam, radialStartDir, radius, 1f, 20);
    }
    private void drawCircle(Vector3 pos, float radius, Vector3 normal,
                            RuntimeGizmoDrawer drawer) {
      var radialStartDir = normal.Perpendicular();

      drawer.DrawWireArc(pos, normal, radialStartDir, radius, 1f, 20);
    }

  }

}
