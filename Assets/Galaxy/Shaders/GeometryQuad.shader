Shader "Galaxy/Stars/Quad"{
  Properties {
    _MainTex     ("Quad Texture", 2D)  = "white" {}
    _Noise       ("Noise",        2D)  = "white" {}
    _NoiseOffset ("Noise Offset", Int) = 0
  }
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

    Cull Off
    Blend One One
    ZWrite Off

		Pass {
			CGPROGRAM
      #pragma target 5.0
			#pragma vertex vert
      #pragma geometry geom
			#pragma fragment frag

      #pragma multi_compile _ USE_RAMP
      #pragma multi_compile _ BY_SPEED BY_DIRECTION BY_ACCEL BY_BLACK_HOLE
      #pragma multi_compile _ INTERPOLATION_CROSSES_TEX_BOUNDARY

			#include "UnityCG.cginc"

      struct appdata {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
      };

      struct v2g {
        float4 pos : POSITION;
        float3 toCam : TEXCOORD1;
        float4 color : COLOR;
      };

			struct g2f {
        float4 pos : SV_POSITION;
        float4 uv : TEXCOORD0;
        float4 color : COLOR;
			};

      sampler2D _MainTex;

      sampler2D _Ramp;
      sampler2D _Noise;

      sampler2D _CurrPosition;
      sampler2D _PrevPosition;
      sampler2D _LastPosition;

      float _CurrInterpolation;
      float _PrevInterpolation;

      float4x4 _ToWorldMat;
      float _Scale;
      float _Bright;
      float _Size;
      uint _NoiseOffset;

      float _PreScalar;
      float _PostScalar;

      v2g vert(uint id : SV_VertexID) {
        float4 uv;
        uv.x = (id / 512) / 512.0;
        uv.y = (id % 512) / 512.0;
        uv.z = 0;
        uv.w = 0;

        float4 currPosition, prevPosition;

#ifdef INTERPOLATION_CROSSES_TEX_BOUNDARY
        {
          float4 currTex = tex2Dlod(_CurrPosition, uv);
          float4 prevTex = tex2Dlod(_PrevPosition, uv);
          float4 lastTex = tex2Dlod(_LastPosition, uv);

          currPosition = lerp(prevTex, currTex, _CurrInterpolation);
          prevPosition = lerp(lastTex, prevTex, _PrevInterpolation);
        }
#else
        {
          float4 currTex = tex2Dlod(_CurrPosition, uv);
          float4 prevTex = tex2Dlod(_PrevPosition, uv);

          currPosition = lerp(prevTex, currTex, _CurrInterpolation);
          prevPosition = lerp(prevTex, currTex, _PrevInterpolation);
        }
#endif

        float4 worldPosition = mul(_ToWorldMat, float4(currPosition.xyz, 1));

        //uint id2 = (id + _NoiseOffset) % (32 * 32);
        //uv.x = (id2 / 32) / 32.0;
        //uv.y = (id2 % 32) / 32.0;
        //uv.z = 0;
        //uv.w = 0;
        //worldPosition.xyz += tex2Dlod(_Noise, uv) * _Size;

        float distToCamera = length(_WorldSpaceCameraPos - worldPosition);

        float screenSpaceWidth = _Scale * _Size * 400 / distToCamera;
        float brightness = _Bright * screenSpaceWidth * screenSpaceWidth;

        v2g o;
        o.pos = worldPosition;
        o.toCam = _WorldSpaceCameraPos - worldPosition;
        o.color = _PreScalar;

#if BY_SPEED
        o.color = _PreScalar * length(prevPosition - currPosition);
#endif

#if BY_DIRECTION
        float3 delta = abs(prevPosition - currPosition);
        float maxC = max(max(delta.x, delta.y), delta.z);

        delta /= maxC;
        delta = pow(delta, 2);
        delta *= min(1, maxC);

        o.color = float4(delta * _PreScalar, 1);
#endif

#if BY_ACCEL
        //TODO: accel not always there :(
        //float4 vel0 = currPosition - prevPosition;
        //float4 vel1 = prevPosition - lastPosition;
        //o.color = _PreScalar * length(vel0 - vel1);
        o.color = float4(1, 0, 0, 1);
#endif

#if BY_BLACK_HOLE
        o.color = currPosition.w;
#endif

#if USE_RAMP
        float2 uv2 = o.color.rg;
        o.color = tex2Dlod(_Ramp, float4(uv2, 0, 0));
#endif

        o.color *= saturate(brightness * _PostScalar);

        return o;
      }

      [maxvertexcount(4)]
      void geom(point v2g input[1], inout TriangleStream<g2f> triStream) {
        g2f o;
        o.color = input[0].color;

        float distToCam = length(input[0].toCam.xyz);
        o.color = o.color * distToCam;

        float3 toCam = input[0].toCam.xyz;
        float3 up = float3(0, 1, 0);

        float4 quadRight = float4(normalize(cross(toCam, up)) * _Size * _Scale, 0);
        float4 quadUp = float4(normalize(cross(toCam, quadRight)) * _Size * _Scale, 0);

        o.pos = mul(UNITY_MATRIX_VP, input[0].pos + quadRight - quadUp);
        o.uv = float4(1, 0, 0, 0);
        triStream.Append(o);

        o.pos = mul(UNITY_MATRIX_VP, input[0].pos - quadRight - quadUp);
        o.uv = float4(0, 0, 0, 0);
        triStream.Append(o);

        o.pos = mul(UNITY_MATRIX_VP, input[0].pos + quadRight + quadUp);
        o.uv = float4(1, 1, 0, 0);
        triStream.Append(o);

        o.pos = mul(UNITY_MATRIX_VP, input[0].pos - quadRight + quadUp);
        o.uv = float4(0, 1, 0, 0);
        triStream.Append(o);
      }
			
			fixed4 frag (g2f i) : SV_Target {
        return tex2D(_MainTex, i.uv) * i.color;
			}
			ENDCG
		}
	}
}
