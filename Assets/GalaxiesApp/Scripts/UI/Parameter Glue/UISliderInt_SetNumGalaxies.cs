public class UISliderInt_SetNumGalaxies : UISliderInt {

  public override int GetMaxValue() {
    return GalaxyUIOperations.GetMaxNumGalaxies();
  }

  public override int GetMinValue() {
    return GalaxyUIOperations.GetMinNumGalaxies();
  }

  protected override float GetModelValue() {
    return GalaxyUIOperations.GetNumGalaxies();
  }

  protected override void SetModelValue(float sliderValue) {
    GalaxyUIOperations.SetNumGalaxies((int)sliderValue);
  }

}
