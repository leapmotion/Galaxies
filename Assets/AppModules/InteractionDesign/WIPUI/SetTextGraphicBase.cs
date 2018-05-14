using Leap.Unity.Attributes;
using Leap.Unity.GraphicalRenderer;
using UnityEngine;

namespace Leap.Unity.WIPUI {

  public abstract class SetTextGraphicBase : MonoBehaviour {

    public LeapTextGraphic textGraphic;

    public string prefix = "";
    public string postfix = "";

    protected virtual void Reset() {
      if (textGraphic == null) textGraphic = GetComponent<LeapTextGraphic>();
    }

    protected virtual void Update() {
      if (textGraphic != null) {
        if (string.IsNullOrEmpty(prefix) && string.IsNullOrEmpty(postfix)) {
          textGraphic.text = GetCurrentText();
        }
        else {
          textGraphic.text = prefix + GetCurrentText() + postfix;
        }
      }
    }

    public abstract string GetCurrentText();

  }

}