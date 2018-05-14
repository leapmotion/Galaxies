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

    private class BaseMultiplier : IPropertyMultiplier {
      public float multiplier { get; set; }
    }
    private BaseMultiplier _baseMult;

    private void OnEnable() {
      if (_sim == null) {
        _sim = FindObjectOfType<GalaxySimulation>();
      }

      if (_renderer == null) {
        _renderer = FindObjectOfType<GalaxyRenderer>();
      }

      _baseMult = new BaseMultiplier();
      _baseMult.multiplier = 1;
      _sim.TimestepMultipliers.Add(_baseMult);
    }

    private void OnDisable() {
      _sim.TimestepMultipliers.Remove(_baseMult);
    }

    public int GetGalaxyCount() {
      return _sim.blackHoleCount;
    }

    public void SetGalaxyCount(int count) {
      _sim.blackHoleCount = count;
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

    public float GetSimulationSpeed() {
      return _baseMult.multiplier;
    }
    
    public void SetRenderMode(RenderPreset preset) {
      _renderer.SetPreset(preset);
    }
    
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

  }

}