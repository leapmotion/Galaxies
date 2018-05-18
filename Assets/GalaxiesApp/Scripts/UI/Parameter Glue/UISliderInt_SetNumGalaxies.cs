namespace Leap.Unity.Galaxies {

  public class UISliderInt_SetNumGalaxies : UISliderIntForGalaxySim {

    protected override float GetMinModelValue() {
      return galaxySimInterop.GetMinGalaxyCount();
    }

    protected override float GetMaxModelValue() {
      return galaxySimInterop.GetMaxGalaxyCount();
    }

    protected override float GetModelValue() {
      return galaxySimInterop.GetGalaxyCount();
    }

    protected override void SetModelValue(float sliderValue) {
      galaxySimInterop.SetGalaxyCount((int)sliderValue);
    }

  }

}
