namespace Leap.Unity.Galaxies {

  public class SetTextGraphic_NumberOfGalaxies : SetTextGraphicFromGalaxySim {

    public override string GetCurrentText() {
      return galaxySimInterop.GetGalaxyCount().ToString();
    }

  }

}
