namespace Leap.Unity.Galaxies {

  public class UIButton_PauseSimulation : UIButtonForGalaxySim {

    public override void OnPress() {
      base.OnPress();

      galaxySimInterop.SetSimulationSpeed(0f);
    }

  }

}
