Shader "Custom/Lantern/WorldBlendingWater"
{
	Properties
	{
		[KeywordEnum(Simple, Lantern)] _Mode("Blend Mode", Float) = 0

		_FirstTexture	("", 2D)		= "white" {}
		_FirstLColor	("", Color)		= (0,0,1,1)
		_FirstFoamColor	("", Color)		= (1,1,1,0.2)

		_SecondTexture	("", 2D)		= "white" {}
		_SecondLColor	("", Color)		= (1,0,0,1)
		_SecondFoamColor("", Color)		= (0,0,0,0.2)

		_WaveDir 		("", Range(-360,360))	= 0
		_WaveSpeed 		("", Range(0,20))		= 1
		[PowerSlider(1.5)]
		_WaveAmount 	("", Range(0,20))	= 5
		[PowerSlider(2.0)]
		_WaveHeight 	("", Range(0,2))	= 0.25

		_Depth			("", Range(0,20))	= 10
		_EdgeIntensity 	("", Range(0,1))	= 0.5

		[IntRange]
		_Tessellation 	("", Range(1,3))	= 1
		_Tess 			("", Float)			= 0

		_NoiseTexture	("", 2D)			= "white" {}
		_NoiseIntensity	("", Range(0,10))	= 1
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent"}

        Cull Off

		Pass
		{
			Name "ForwardBase"
            Tags {"LightMode"="ForwardBase"}

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

			CGPROGRAM

			#pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_fwdbase
			#pragma shader_feature _SIMPLE _LANTERN

			#define FORWARDBASE_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "WorldBlendingWater.cginc"

			ENDCG
		}
		Pass
		{
			Name "ForwardAdd"
            Tags {"LightMode"="ForwardAdd"}

            Blend One One
            ZWrite Off

			CGPROGRAM
			#pragma hull hull
            #pragma domain domain
            #pragma vertex tessvert
            #pragma fragment frag
			#pragma target 5.0
			#pragma multi_compile_fwdadd
			#pragma shader_feature _SIMPLE _LANTERN

			#define FORWARDADD_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "WorldBlendingWater.cginc"

			ENDCG
		}
	}
	CustomEditor "LanternShaderGUI"
}
