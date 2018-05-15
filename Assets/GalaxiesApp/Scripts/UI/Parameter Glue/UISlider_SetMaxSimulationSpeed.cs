using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class UISlider_SetMaxSimulationSpeed : UISliderForGalaxySim, IPropertyMultiplier {

    public float multiplier {
      get {
        return slider.normalizedHorizontalValue;
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
