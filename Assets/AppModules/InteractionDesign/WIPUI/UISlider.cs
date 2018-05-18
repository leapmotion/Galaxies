using Leap.Unity.Interaction;
using UnityEngine;

public abstract class UISlider : UIButton {

  [Header("UI Slider")]

  public InteractionSlider slider;

  protected override void initialize() {
    base.initialize();

    if (button != null && button is InteractionSlider) {
      slider = button as InteractionSlider;
    }

    if (slider != null) {
      button = slider;
    }
  }

  public float value {
    get {
      return slider.HorizontalSliderValue;
    }
    set {
      slider.HorizontalSliderValue = value;
      slider.HorizontalSliderValue = filterSliderValue(slider.HorizontalSliderValue);
      onSlideEvent(slider.HorizontalSliderValue);
    }
  }

  protected override void OnEnable() {
    base.OnEnable();

    slider.HorizontalSlideEvent -= onSlideEvent;
    slider.HorizontalSlideEvent += onSlideEvent;

    slider.OnUnpress -= onUnpress;
    slider.OnUnpress += onUnpress;

    slider.OnContactEnd -= onContactEnd;
    slider.OnContactEnd += onContactEnd;
  }

  protected override void OnDisable() {
    base.OnDisable();

    if (slider != null) {
      slider.HorizontalSlideEvent -= onSlideEvent;
      slider.OnUnpress -= onUnpress;
      slider.OnContactEnd -= onContactEnd;
    }
  }

  private bool _firstUpdate = true;
  protected virtual void Update() {
    if (_firstUpdate) {
      refreshSimValue();
      refreshMinMaxValues();

      _firstUpdate = false;
    }

    if (_timeSinceLastContactEnd <= _contactEndWait) {
      _timeSinceLastContactEnd += Time.deltaTime;

      if (_timeSinceLastContactEnd > _contactEndWait) {
        SetModelValue(value);
      }
    }
    else {
      refreshSimValue();
    }
  }

  private float _timeSinceLastContactEnd = 100f;
  private float _contactEndWait = 0.5f;
  /// <summary>
  /// Sliders send contact end pretty often because the collision checks are imperfect--
  /// arguably this is a bug in the Interaction Engine, as a workaround for now we wait
  /// for contact end callbacks to stop happening for half a second before assuming 
  /// contact has "truly" ended, and then we update the simulation with the slider's
  /// value.
  /// </summary>
  private void onContactEnd() {
    _timeSinceLastContactEnd = 0f;
  }

  private void onSlideEvent(float value) {
    value = filterSliderValue(value);
  }

  private void onUnpress() {
    SetModelValue(value);
  }

  /// <summary>
  /// Called whenever the user manipulates the slider via script or controller; use this
  /// to enforce constraints e.g. slider value snapping or integer rounding.
  /// </summary>
  protected virtual float filterSliderValue(float sliderValue) {
    return sliderValue;
  }

  /// <summary>
  /// Sets the data model value this slider controls with the argument value, which
  /// reflects the current value of this slider.
  /// </summary>
  protected abstract void SetModelValue(float sliderValue);

  /// <summary>
  /// Returns the current value at the data model -- not the value of this slider. This
  /// is used e.g. to initialize the slider to match the system value it will manipulate.
  /// </summary>
  protected abstract float GetModelValue();

  /// <summary>
  /// Optionally override this method to cause the slider to initialize itself using
  /// minimum value from the data model.
  /// </summary>
  protected virtual float GetMinModelValue() {
    return slider.minHorizontalValue;
  }

  /// <summary>
  /// Optionally override this method to cause the slider to initialize itself using
  /// maximum value from the data model.
  /// </summary>
  protected virtual float GetMaxModelValue() {
    return slider.maxHorizontalValue;
  }

  /// <summary>
  /// Moves this slider to match the data model's current value.
  /// </summary>
  private void refreshSimValue() {
    float sliderValue = slider.HorizontalSliderValue;
    float simValue = GetModelValue();
    if (sliderValue != simValue) {
      slider.HorizontalSliderValue = simValue;
    }
  }

  private void refreshMinMaxValues() {
    slider.minHorizontalValue = GetMinModelValue();
    slider.maxHorizontalValue = GetMaxModelValue();
  }

}
