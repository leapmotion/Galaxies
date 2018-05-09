using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(HeatmapRenderer), PostProcessEvent.BeforeStack, "Custom/Heatmap")]
public class Heatmap : PostProcessEffectSettings {
  public FloatParameter preScalar = new FloatParameter() { value = 1.0f };
  public TextureParameter gradient = new TextureParameter();
}

public class HeatmapRenderer : PostProcessEffectRenderer<Heatmap> {

  public override void Render(PostProcessRenderContext context) {
    var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Heatmap"));
    sheet.properties.SetTexture("_Ramp", settings.gradient);
    sheet.properties.SetFloat("_PreScalar", settings.preScalar);
    context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
  }
}