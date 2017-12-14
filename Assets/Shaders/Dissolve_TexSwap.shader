Shader "Custom/Dissolve/World/TextureSwap" {
	Properties {
		[Header(First Texture)][Space]
		[NoScaleOffset]
		_FirstTexture	("Texture", 	2D)		= "white" {}
		_FirstColor		("Color", 	Color)	= (1,1,1,1)

		[Space(10)][Header(Second Texture)][Space]
		[NoScaleOffset]
		_SecondTexture	("Texture", 	2D)		= "white" {}
		_SecondColor	("Color", 	Color)	= (0,0,0,1)

		[Space(10)][Header(Dissolve)][Space]
		_Color			("Color", 			Color)			= (0,0,1,1)
		_Distance		("Distance", 		Float)			= 10
		_Interpolation	("Interpolation", 	Range(0,5))		= 2
		_Strength		("Strength", 		Range(0,1))		= 0.125
		_Falloff		("Falloff", 		Range(0.1,2))	= 0.1
		_Offset			("Offset", 			Range(-1,2))	= 0.25
		_Center			("Center", 			Vector)			= (0,0,0,0)

		[Space(10)][Header(Noise)][Space]
		_Freq 	("Frequency", Float)	= 0.5
		_Speed 	("Speed", Float)		= 1
	}

	SubShader {
		Tags {"RenderType" = "Opaque"}
		LOD 200

		CGPROGRAM

		#pragma surface surf Standard addshadow
		#pragma target 3.0

		#include "noiseSimplex.cginc"

		uniform sampler2D 
			_FirstTexture,
			_SecondTexture
		;

		uniform fixed4
			_FirstColor,
			_SecondColor,
			_Color
		;

		uniform half
			_Freq,
			_Speed,
			_Distance,
			_Interpolation,
			_Strength,
			_Falloff,
			_Offset
		;

		uniform float4 _Center;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {

			float l = _Distance - length(_Center.xyz - IN.worldPos.xyz);

			float3 wrldPos = IN.worldPos.xyz * _Freq;
			wrldPos.y += _Time.x * _Speed;

			float ns = snoise(wrldPos);
			
			float lrp = saturate((l + ns * _Interpolation) * 1/_Falloff);

			fixed4 tex1 = tex2D(_FirstTexture, IN.uv_MainTex);
			fixed4 tex2 = tex2D(_SecondTexture, IN.uv_MainTex);
			o.Albedo = lerp(tex1.rgb * _FirstColor.rgb, tex2.rgb * _SecondColor.rgb, 1-lrp);

			o.Emission = lrp * saturate(1-(l-_Offset)) * _Color.rgb * saturate(ns*_Strength*10+_Strength*0.5f * (1/_Falloff));
			o.Smoothness = 0;
			o.Metallic = 0;

		}

		ENDCG
	}

	Fallback "Diffuse"
}