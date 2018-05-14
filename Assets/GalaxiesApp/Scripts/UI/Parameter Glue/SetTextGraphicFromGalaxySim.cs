using Leap.Unity.WIPUI;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public abstract class SetTextGraphicFromGalaxySim : SetTextGraphicBase {

    public GalaxiesUISimInterop galaxySimInterop;

    protected override void Reset() {
      base.Reset();

      if (galaxySimInterop == null) {
        galaxySimInterop = FindObjectOfType<GalaxiesUISimInterop>();
      }
    }

    protected virtual void Start() {
      if (galaxySimInterop == null) {
        galaxySimInterop = FindObjectOfType<GalaxiesUISimInterop>();
      }
    }

  }

}
