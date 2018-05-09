using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[CreateAssetMenu(order = 900)]
public class RenderPreset : ScriptableObject {

  [Header("Star Coloring")]
  public BlitMode blitMode = BlitMode.Solid;
  public Color baseColor = Color.white;
  public bool enableStarGradient = false;
  public float preScalar = 1;
  public Gradient starRamp;
  public float postScalar = 1;

  [Header("Post Processing")]
  public PostProcessProfile profile;

  public enum BlitMode {
    Solid,
    BySpeed,
    ByDirection,
    ByAccel,
    ByStartingBlackHole
  }

  public enum PostProcessMode {
    None = 0,
    HeatMap = 1
  }

  private Texture2D _starTex;

  public Texture2D starTex {
    get {
      if (_starTex == null) {
        _starTex = starRamp.ToTexture();
      }
      return _starTex;
    }
  }

  void OnValidate() {
    _starTex = starRamp.ToTexture();
  }
}

