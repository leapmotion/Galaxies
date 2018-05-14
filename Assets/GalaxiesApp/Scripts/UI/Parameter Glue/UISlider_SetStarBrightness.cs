namespace Leap.Unity.Galaxies {

  public class UISlider_SetStarBrightness : UISlider {

    protected override float GetModelValue() {
      return GalaxyUIOperations.GetStarBrightness();
    }

    protected override void SetModelValue(float sliderValue) {
      GalaxyUIOperations.SetStarBrightness(slider.normalizedHorizontalValue);
    }
  }

}
