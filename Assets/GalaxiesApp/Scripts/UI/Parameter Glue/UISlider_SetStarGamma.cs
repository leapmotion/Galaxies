public class UISlider_SetStarGamma : UISlider {

  protected override float GetModelValue() {
    return GalaxyUIOperations.GetStarGamma();
  }

  protected override void SetModelValue(float sliderValue) {
    GalaxyUIOperations.SetStarGamma(slider.normalizedHorizontalValue);
  }
}
