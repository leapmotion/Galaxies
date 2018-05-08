using System;
using UnityEngine;

public class EnableDisablePathControl : MonoBehaviour {

  public GalaxyIE galaxyIE;
  public GalaxySimulation galaxySim;
  public GalaxyRenderer galaxyRenderer;

  [Header("Settings")]
  public Settings pathControlEnabled;
  public Settings pathControlDisabled;

  private StarBrightnessMultiplier _starBrightnessMultiplier;

  private void Awake() {
    _starBrightnessMultiplier = new StarBrightnessMultiplier();
    _starBrightnessMultiplier.multiplier = 1;
    galaxyRenderer.startBrightnessMultipliers.Add(_starBrightnessMultiplier);
  }

  private void OnEnable() {
    applySettings(pathControlEnabled);
    galaxyIE.canAct = true;
  }

  private void OnDisable() {
    applySettings(pathControlDisabled);
    galaxyIE.canAct = false;
  }

  private void OnDestroy() {
    if (galaxyRenderer != null) {
      galaxyRenderer.startBrightnessMultipliers.Remove(_starBrightnessMultiplier);
    }
  }

  private void applySettings(Settings settings) {
    if (galaxySim != null) {
      galaxySim.trailColor = settings.trailColor;
    }

    _starBrightnessMultiplier.multiplier = settings.starBrightness;
  }

  [Serializable]
  public struct Settings {
    public Color trailColor;
    public float starBrightness;
  }

  private class StarBrightnessMultiplier : IPropertyMultiplier {
    public float multiplier { get; set; }
  }
}
