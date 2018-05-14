namespace Leap.Unity.Galaxies {

  public class UIButton_PlaySimulation : UIButtonForGalaxySim {

    public override void OnPress() {
      base.OnPress();

      galaxySimInterop.SetSimulationSpeed(1f);
    }

  }

}
