using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Leap.Unity;
using Leap.Unity.DevGui;
using Leap.Unity.Attributes;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class GalaxyRenderer : MonoBehaviour {
  private const string BOX_FILTER_KEYWORD = "BOX_FILTER";
  private const string STAR_RAMP_KEYWORD = "USE_RAMP";

  private const string BY_SPEED_KEYWORD = "BY_SPEED";
  private const string BY_DIRECTION_KEYWORD = "BY_DIRECTION";
  private const string BY_ACCEL_KEYWORD = "BY_ACCEL";
  private const string BY_BLACK_HOLE_KEYWORD = "BY_BLACK_HOLE";

  private const string START_TEX_PROPERTY = "_Stars";
  private const string GRADIENT_PROPERTY = "_Gradient";

  private const string GAMMA_PROPERTY = "_Gamma";
  private const string ADJACENT_PROPERTY = "_AdjacentFilter";
  private const string DIAGONAL_PROPERTY = "_DiagonalFilter";

  private const string CROSS_TEX_KEYWORD = "INTERPOLATION_CROSSES_TEX_BOUNDARY";

  public List<IPropertyMultiplier> startBrightnessMultipliers = new List<IPropertyMultiplier>();

  [SerializeField]
  private GalaxySimulation _sim;

  [SerializeField]
  private Transform _displayAnchor;
  public Transform displayAnchor {
    get { return _displayAnchor; }
  }

  [DevCategory("General Settings")]
  [Range(0.01f, 2f)]
  [SerializeField, DevValue]
  private float _scale = 1;

  //[SerializeField, DevValue]
  //private float _forwardOffset = 0;

  [Header("Black Hole Rendering"), DevCategory]
  [SerializeField, DevValue("Render")]
  private bool _renderBlackHoles = true;

  [Range(0, 1)]
  [SerializeField, DevValue("Size")]
  private float _blackHoleSize = 0.05f;

  [SerializeField]
  private Mesh _blackHoleMesh;

  [SerializeField]
  private Material _blackHoleMat;

  [Header("Star Rendering"), DevCategory]
  [SerializeField, DevValue]
  private bool _renderStars = true;

  [Range(0, 0.05f)]
  [FormerlySerializedAs("starSize")]
  [SerializeField, DevValue]
  private float _starSize;

  [Range(0, 1)]
  [FormerlySerializedAs("starBrightness")]
  [SerializeField, DevValue]
  private float _starBrightness;

  [Disable]
  [SerializeField]
  private RenderType _renderType;

  [SerializeField]
  private Material _pointMat;

  [SerializeField]
  private Material _quadMat;

  [SerializeField]
  private Material _lightMat;


  [Header("Render Presets")]
  [SerializeField]
  private RenderPreset preset;

  [SerializeField]
  private Material _postProcessMat;

  [Range(0, 2)]
  [SerializeField, DevValue]
  private float _gammaValue = 0.3f;

  [SerializeField, DevValue]
  private bool _enableBoxFilter = true;

  [Range(0, 1)]
  [SerializeField, DevValue]
  private float _adjacentFilter = 0.75f;

  [Range(0, 1)]
  [SerializeField, DevValue]
  private float _diagonalFilter = 0.5f;

  private Camera _myCamera;
  private CommandBuffer _renderCommands;

  private RenderState _currRenderState;
  private RenderState _prevRenderState;

  public float scale {
    get {
      return _scale;
    }
    set {
      _scale = value;
    }
  }

  public enum RenderType {
    Point,
    Quad,
    PointBright
  }

  public void SetPreset(RenderPreset preset) {
    this.preset = preset;
    uploadGradientTextures();
  }

  private void OnValidate() {
    uploadGradientTextures();
  }

  private void OnEnable() {
    _renderCommands = new CommandBuffer();
    _renderCommands.name = "Draw Starts";

    _myCamera = GetComponent<Camera>();
    Camera.onPostRender += drawCamera;

    uploadGradientTextures();

    StartCoroutine(endOfFrameCoroutine());

    updateCameraCommandBuffer();
  }

  private void OnDisable() {
    _renderCommands.Dispose();

    Camera.onPostRender -= drawCamera;
  }

  private IEnumerator endOfFrameCoroutine() {
    var eofWaiter = new WaitForEndOfFrame();
    while (true) {
      yield return eofWaiter;
      _prevRenderState.CopyFrom(_currRenderState);
    }
  }

  public void UpdatePositions(Texture currPosition, Texture prevPosition, Texture lastPosition, float interpolationFraction) {
    _currRenderState.currPosition = currPosition;
    _currRenderState.prevPosition = prevPosition;
    _currRenderState.lastPosition = lastPosition;
    _currRenderState.interpolationFraction = interpolationFraction;
  }

  public void DrawBlackHole(Vector3 position) {
    if (_renderBlackHoles) {
      _blackHoleMat.SetColor("_Color", preset.baseColor);

      Graphics.DrawMesh(_blackHoleMesh,
                        _displayAnchor.localToWorldMatrix * Matrix4x4.TRS(position, Quaternion.identity, Vector3.one * _blackHoleSize),
                        _blackHoleMat,
                        0);
    }
  }

  [ContextMenu("Update Command Buffer")]
  private void updateCameraCommandBuffer() {
    _myCamera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, _renderCommands);
    generateCommandBuffer();
    _myCamera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _renderCommands);
  }

  private void generateCommandBuffer() {
    _renderCommands.Clear();

    if (_currRenderState.currPosition == null) {
      return;
    }

    RenderTextureDescriptor desc = new RenderTextureDescriptor(_myCamera.pixelWidth, _myCamera.pixelHeight, RenderTextureFormat.ARGBHalf, 0);
    desc.sRGB = false;
    desc.msaaSamples = 1;
    desc.autoGenerateMips = false;
    desc.useMipMap = false;

    int id = Shader.PropertyToID(START_TEX_PROPERTY);
    RenderTargetIdentifier identifier = new RenderTargetIdentifier(id);

    _renderCommands.GetTemporaryRT(id, desc);
    _renderCommands.SetRenderTarget(identifier);

    _renderCommands.ClearRenderTarget(clearDepth: false, clearColor: true, backgroundColor: Color.black);

    Material mat = null;

    switch (_renderType) {
      case RenderType.Point:
        mat = _pointMat;
        break;
      case RenderType.Quad:
        mat = _quadMat;
        break;
      case RenderType.PointBright:
        mat = _lightMat;
        break;
    }

    _renderCommands.DrawProcedural(Matrix4x4.identity, mat, 0, MeshTopology.Points, _currRenderState.currPosition.width * _currRenderState.currPosition.height);
    _renderCommands.SetGlobalTexture("_Stars", identifier);

    _renderCommands.Blit(identifier, new RenderTargetIdentifier(BuiltinRenderTextureType.CameraTarget), _postProcessMat, (int)preset.postProcessMode);
    _renderCommands.ReleaseTemporaryRT(id);
  }

  private void updateMaterials() {
    //Post process material
    {
      if (_enableBoxFilter) {
        _postProcessMat.EnableKeyword(BOX_FILTER_KEYWORD);
        _postProcessMat.SetFloat(ADJACENT_PROPERTY, _adjacentFilter);
        _postProcessMat.SetFloat(DIAGONAL_PROPERTY, _diagonalFilter);
      } else {
        _postProcessMat.DisableKeyword(BOX_FILTER_KEYWORD);
      }

      _postProcessMat.SetFloat(GAMMA_PROPERTY, _gammaValue);
    }

    Material mat = null;

    switch (_renderType) {
      case RenderType.Point:
        mat = _pointMat;
        break;
      case RenderType.Quad:
        mat = _quadMat;
        break;
      case RenderType.PointBright:
        mat = _lightMat;
        break;
    }

    mat.DisableKeyword(BY_SPEED_KEYWORD);
    mat.DisableKeyword(BY_DIRECTION_KEYWORD);
    mat.DisableKeyword(BY_ACCEL_KEYWORD);
    mat.DisableKeyword(BY_BLACK_HOLE_KEYWORD);
    switch (preset.blitMode) {
      case RenderPreset.BlitMode.BySpeed:
        mat.EnableKeyword(BY_SPEED_KEYWORD);
        break;
      case RenderPreset.BlitMode.ByDirection:
        mat.EnableKeyword(BY_DIRECTION_KEYWORD);
        break;
      case RenderPreset.BlitMode.ByAccel:
        mat.EnableKeyword(BY_ACCEL_KEYWORD);
        break;
      case RenderPreset.BlitMode.ByStartingBlackHole:
        mat.EnableKeyword(BY_BLACK_HOLE_KEYWORD);
        break;
    }

    if (preset.enableStarGradient) {
      mat.EnableKeyword(STAR_RAMP_KEYWORD);
    } else {
      mat.DisableKeyword(STAR_RAMP_KEYWORD);
    }

    mat.SetTexture("_CurrPosition", _currRenderState.currPosition);
    mat.SetTexture("_PrevPosition", _currRenderState.prevPosition);
    mat.SetTexture("_LastPosition", _currRenderState.lastPosition);

    mat.SetFloat("_CurrInterpolation", _currRenderState.interpolationFraction);
    mat.SetFloat("_PrevInterpolation", _prevRenderState.interpolationFraction);

    if (_currRenderState.currPosition != _prevRenderState.currPosition) {
      mat.EnableKeyword(CROSS_TEX_KEYWORD);
    } else {
      mat.DisableKeyword(CROSS_TEX_KEYWORD);
    }

    switch (preset.blitMode) {
      case RenderPreset.BlitMode.BySpeed:
        mat.SetFloat("_PreScalar", preset.preScalar / _sim.timescale);
        break;
      case RenderPreset.BlitMode.ByAccel:
        mat.SetFloat("_PreScalar", preset.preScalar / _sim.timescale / _sim.timescale);
        break;
      default:
        mat.SetFloat("_PreScalar", preset.preScalar);
        break;
    }

    mat.SetFloat("_PostScalar", preset.postScalar);

    mat.SetMatrix("_ToWorldMat", _displayAnchor.localToWorldMatrix);
    mat.SetFloat("_Scale", _displayAnchor.lossyScale.x);
    mat.SetFloat("_Size", _starSize);


    float finalBrightness = _starBrightness;
    foreach (var multiplier in startBrightnessMultipliers) {
      finalBrightness *= multiplier.multiplier;
    }

    mat.SetFloat("_Bright", finalBrightness);

#if UNITY_EDITOR
    uploadGradientTextures();
#endif
  }

  private void OnPreRender() {
    updateMaterials();
  }

  private void OnPostRender() {
    _prevRenderState.CopyFrom(_currRenderState);
  }

  private void drawCamera(Camera camera) {
    if (_myCamera == camera) {
      return;
    }

    //drawStars();
    //TODO
  }

  private void uploadGradientTextures() {
    _postProcessMat.SetTexture(GRADIENT_PROPERTY, preset.heatTex);

    var starTex = preset.starTex;
    _pointMat.SetTexture("_Ramp", starTex);
    _quadMat.SetTexture("_Ramp", starTex);
    _lightMat.SetTexture("_Ramp", starTex);
  }

  private struct RenderState {
    public Texture currPosition;
    public Texture prevPosition;
    public Texture lastPosition;
    public float interpolationFraction;

    public void CopyFrom(RenderState other) {
      currPosition = other.currPosition;
      prevPosition = other.prevPosition;
      lastPosition = other.lastPosition;
      interpolationFraction = other.interpolationFraction;
    }
  }
}
