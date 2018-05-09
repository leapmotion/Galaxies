Shader "Hidden/Custom/Heatmap" {
  HLSLINCLUDE

  #include "PostProcessing/Shaders/StdLib.hlsl"

  TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
  TEXTURE2D_SAMPLER2D(_Ramp, sampler_Ramp);
  float _PreScalar;

  float4 Frag(VaryingsDefault i) : SV_Target {
    float4 value = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
    float4 color = SAMPLE_TEXTURE2D(_Ramp, sampler_Ramp, float2(_PreScalar * value.r, 0));
    return color;
  }

  ENDHLSL

  SubShader {
    Cull Off ZWrite Off ZTest Always

    Pass {
      HLSLPROGRAM
      #pragma vertex VertDefault
      #pragma fragment Frag
      ENDHLSL
    }
  }
}