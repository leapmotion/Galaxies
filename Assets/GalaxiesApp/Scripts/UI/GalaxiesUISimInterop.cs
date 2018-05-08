using UnityEngine;

namespace Leap.Unity.Galaxies {

  public class GalaxiesUISimInterop : MonoBehaviour {

    // TODO: Any necessary inspector links here
    

    public int GetGalaxyCount() {
      throw new System.NotImplementedException();
    }

    public void SetGalaxyCount (int count) {
      throw new System.NotImplementedException();
    }

    public int GetMinGalaxyCount() {
      throw new System.NotImplementedException();
    }

    public int GetMaxGalaxyCount() {
      throw new System.NotImplementedException();
    }

    public void RestartGalaxySimulation() {
      throw new System.NotImplementedException();
    }

    // "Start" and "Stop" buttons were requested, but I am thinking this would just be
    // implemented by setting the simulation speed to 0 or 1.

    /// <summary>
    /// 0 is "stopped", 1 is "full speed".
    /// </summary>
    public void SetSimulationSpeed(float normalizedSpeed) {
      throw new System.NotImplementedException();
    }

    public enum MuhPlaceholderEnum { PleaseReplaceMeWithAGoodEnum };

    /// <summary>
    /// Not sure what the appropriate input type is -- an enum probably, right?
    /// 
    /// (Don't remember if we have a different way of specifying render modes)
    /// </summary>
    public void SetRenderMode(MuhPlaceholderEnum REPLACE_ME) {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// { TRS, Interactive }
    /// Would be nice to have this to try a different control mechanism for the mode,
    /// but okay if it isn't convenient to decouple this from the gesture.
    /// </summary>
    public void SetInteractionMode(MuhPlaceholderEnum REPLACE_ME) {
      throw new System.NotImplementedException();
    }

  }

}