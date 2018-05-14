using UnityEngine;

public abstract class UISliderInt : UISlider {

  protected override void initialize() {
    base.initialize();

    if (slider != null) {
      slider.minHorizontalValue = GetMinValue();
      slider.maxHorizontalValue = GetMaxValue();
      slider.horizontalSteps = (GetMaxValue() - GetMinValue());
    }
  }

  public abstract int GetMinValue();

  public abstract int GetMaxValue();

  protected override float filterSliderValue(float sliderValue) {
    return Mathf.RoundToInt(sliderValue);
  }

}
