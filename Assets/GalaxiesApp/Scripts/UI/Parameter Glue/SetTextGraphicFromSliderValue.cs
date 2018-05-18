using Leap.Unity.Interaction;
using Leap.Unity.WIPUI;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class SetTextGraphicFromSliderValue : SetTextGraphicBase {

    public InteractionSlider slider;
    public string formatArg = "F2";

    public override string GetCurrentText() {
      return slider.HorizontalSliderValue.ToString(formatArg);
    }

    protected override void Reset() {
      base.Reset();

      if (slider == null) {
        slider = GetComponentInParent<InteractionSlider>();
      }
    }

    protected virtual void Start() {
      if (slider == null) {
        slider = GetComponentInParent<InteractionSlider>();
      }
    }



  }

}
