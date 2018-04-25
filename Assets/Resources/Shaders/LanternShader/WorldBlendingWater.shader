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

		_TextureScale	("", Float)		= 1

		_WaveSpeed 		("", Range(0,20))		= 1
		_WaveAmount 	("", Float)		= 5
		[PowerSlider(2.0)]
		_WaveHeight 	("", Range(0,2))	= 0.25

		_Depth			("", Range(0,20))	= 10
		_EdgeIntensity 	("", Range(0,1))	= 0.5

		_NoiseScale		("", Range(0,20))	= 1
		_NoiseIntensity	("", Range(0,2))	= 1

		_StepCount	("", Range(0,20))	= 10

		_DistortStrength ("", Range(0,5))	= 1
	}

	SubShader
	{
		Tags {"Queue"="Transparent"
		"RenderType"="Transparent"
		"IgnoreProjector"="True"}

        Cull Off

        GrabPass {"_BackgroundTexture"}

        Pass
        {
			Name "GrabPass"

			Zwrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
			#define GRAB_PASS
            
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "WorldBlendingWater.cginc"

			ENDCG
    	}

		Pass
		{
			Name "ForwardBase"
            Tags {"LightMode"="ForwardBase"}

            Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
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

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fwdadd
			#pragma multi_compile_fog
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
