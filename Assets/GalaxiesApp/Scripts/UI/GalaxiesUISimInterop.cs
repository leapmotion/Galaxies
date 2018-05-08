using System;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class GalaxiesUISimInterop : MonoBehaviour {

    [SerializeField]
    private GalaxySimulation _sim;

    [SerializeField]
    private GalaxyRenderer _renderer;

    [SerializeField]
    private GameObject _trsPinchAnchor;

    [SerializeField]
    private GameObject _moveGalaxiesAnchor;

    private GameObject _currActiveAnchor;
    private BaseMultiplier _baseMult;

    private void OnEnable() {
      if (_sim == null) {
        _sim = FindObjectOfType<GalaxySimulation>();
      }

      if (_renderer == null) {
        _renderer = FindObjectOfType<GalaxyRenderer>();
      }

      _baseMult = new BaseMultiplier();
      _sim.TimestepMultipliers.Add(_baseMult);
    }

    private void OnDisable() {
      _sim.TimestepMultipliers.Remove(_baseMult);
    }

    public int GetGalaxyCount() {
      throw new System.NotImplementedException();
    }

    public void SetGalaxyCount(int count) {
      throw new System.NotImplementedException();
    }

    public int GetMinGalaxyCount() {
      return 1;
    }

    public int GetMaxGalaxyCount() {
      return 10;
    }

    public void RestartGalaxySimulation() {
      _sim.ResetSimulation();
    }

    // "Start" and "Stop" buttons were requested, but I am thinking this would just be
    // implemented by setting the simulation speed to 0 or 1.

    /// <summary>
    /// 0 is "stopped", 1 is "full speed".
    /// </summary>
    public void SetSimulationSpeed(float normalizedSpeed) {
      _baseMult.multiplier = normalizedSpeed;
    }

    public enum MuhPlaceholderEnum { PleaseReplaceMeWithAGoodEnum };

    /// <summary>
    /// Not sure what the appropriate input type is -- an enum probably, right?
    /// 
    /// (Don't remember if we have a different way of specifying render modes)
    /// </summary>
    public void SetRenderMode(RenderPreset preset) {
      _renderer.SetPreset(preset);
    }

    /// <summary>
    /// { TRS, Interactive }
    /// Would be nice to have this to try a different control mechanism for the mode,
    /// but okay if it isn't convenient to decouple this from the gesture.
    /// </summary>
    public void SetInteractionMode(InteractionMode interactionMode) {
      if (_currActiveAnchor != null) {
        _currActiveAnchor.SetActive(false);
      }

      switch (interactionMode) {
        case InteractionMode.TRS_Pinch:
          _currActiveAnchor = _trsPinchAnchor;
          break;
        case InteractionMode.MoveGalaxies:
          _currActiveAnchor = _moveGalaxiesAnchor;
          break;
      }

      _currActiveAnchor.SetActive(true);
    }

    public enum InteractionMode {
      TRS_Pinch = 0,
      MoveGalaxies = 10
    }

    private class BaseMultiplier : IPropertyMultiplier {
      public float multiplier { get; set; }
    }

    [Serializable]
    public class SwitchGroup {
      [SerializeField]
      private GameObject _anchor;

      [SerializeField]
      private GrabSwitch _a, _b;

      public void Enable(LeapRTS rts) {
        if (_anchor != null) {
          _anchor.SetActive(true);
        }

        if (_a != null) {
          rts._switchA = _a;
        }

        if (_b != null) {
          rts._switchB = _b;
        }
      }

      public void Disable() {
        if (_anchor != null) {
          _anchor.SetActive(false);
        }
      }
    }
  }

}