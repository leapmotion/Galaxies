namespace Leap.Unity.Galaxies {

  public class SetTextGraphic_SimulationSpeed : SetTextGraphicFromGalaxySim {

    public override string GetCurrentText() {
      return galaxySimInterop.GetSimulationSpeed().ToString("F1");
    }

  }

}
