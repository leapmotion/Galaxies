using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class UIButtonForGalaxySim : UIButton {

    [Header("Galaxy Simulation Interop")]
    public GalaxiesUISimInterop galaxySimInterop;

    protected override void Reset() {
      base.Reset();

      if (galaxySimInterop == null) {
        galaxySimInterop = FindObjectOfType<GalaxiesUISimInterop>();
      }
    }

    protected virtual void Start() {
      if (galaxySimInterop == null) {
        galaxySimInterop = GetComponent<GalaxiesUISimInterop>();
      }
    }

  }

  public abstract class UISliderForGalaxySim : UISlider {

    [Header("Galaxy Simulation Interop")]
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
