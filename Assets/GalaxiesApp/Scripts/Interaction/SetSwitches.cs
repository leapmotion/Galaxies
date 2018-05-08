using Leap.Unity;
using UnityEngine;

public class SetSwitches : MonoBehaviour {

  [SerializeField]
  private LeapRTS _rts;

  [SerializeField]
  private GrabSwitch _switchA;

  [SerializeField]
  private GrabSwitch _switchB;

  private void OnEnable() {
    _rts._switchA = _switchA;
    _rts._switchB = _switchB;
  }
}
