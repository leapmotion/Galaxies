using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class UIButton_ResetSimulation : UIButtonForGalaxySim {

    public override void OnPress() {
      GalaxyUIOperations.ResetSimulation();
    }

  }


}