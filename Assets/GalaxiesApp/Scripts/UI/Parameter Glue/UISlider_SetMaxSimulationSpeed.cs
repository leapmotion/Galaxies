using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class UISlider_SetMaxSimulationSpeed : UISliderForGalaxySim, IPropertyMultiplier {

    public GalaxySimulation simulation;

    public float multiplier {
      get {
        return slider.normalizedHorizontalValue;
      }
    }

    protected override void OnEnable() {
      base.OnEnable();
      if (simulation == null) {
        Debug.LogError("UISlider SetMaxSimulationSpeed requires a simulation to be " +
                       "connected!");
        enabled = false;
        return;
      }

      simulation.TimestepMultipliers.Add(this);
    }

    protected override void OnDisable() {
      base.OnDisable();

      if (simulation != null) {
        simulation.TimestepMultipliers.Remove(this);
      }
    }

    protected override void SetModelValue(float sliderValue) {
      galaxySimInterop.SetSimulationSpeed(sliderValue);
    }

    protected override float GetModelValue() {
      return galaxySimInterop.GetSimulationSpeed();
    }
  }

}
