Shader "Custom/Lantern/WorldBlending"
{
	Properties
	{
		[KeywordEnum(Simple, Lantern)] _Mode("Blend Mode", Float) = 0

		[KeywordEnum(Alpha, Emissive, Mask)] _AlphaMode("Alpha is :", Float) = 0
		_AlphaModeEmiL	("", Color)			= (0,0,0,0)
		_AlphaModeEmiD	("", Color)			= (0,0,0,0)

		_FirstTexture	("", 2D)			= "white" {}
		_FirstLColor	("", Color)			= (1,1,1,0)
		_FirstDColor	("", Color)			= (0,0,0,0)

		_SecondTexture	("", 2D)			= "white" {}
		_SecondLColor	("", Color)			= (1,1,1,0)
		_SecondDColor	("", Color)			= (0,0,0,0)

		[PowerSlider(2.0)] 
		_SpecIntensity	("", Range(0,30))	= 0
		[PowerSlider(2.0)] 
		_SpecPow		("", Range(0.01,20))= 1
		[PowerSlider(2.0)] 
		_RimIntensity	("", Range(0,50))	= 0
		[PowerSlider(2.0)] 
		_RimPow			("", Range(0.01,20))= 1

		_StepCount		("", Range(0,20))	= 3

		[Toggle] _IsMask("", Float) = 0

		_MaskTexture	("", 2D)			= "white" {}
		_MaskRColor		("", Color)			= (1,1,1,1)
		[PowerSlider(3.0)] 
		_MaskREmi		("", Range(1,5))	= 1
		_MaskGColor		("", Color)			= (0,0,0,1)
		[PowerSlider(3.0)] 
		_MaskGEmi		("", Range(1,5))	= 1

	}

	SubShader
	{
		Tags {"Queue"="AlphaTest" "RenderType"="Lantern"}

	    Cull Off

		Pass
		{
			Name "ForwardBase"
            Tags {"LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fwdbase_fullshadows
			#pragma multi_compile_fog
			#pragma shader_feature _SIMPLE _LANTERN
			#pragma shader_feature _ALPHAMODE _EMISSIVEMODE _MASKMODE
			#pragma shader_feature _ISMASK_ON

			#define FORWARDBASE_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "WorldBlending.cginc"

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
			#pragma target 3.0
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog
			#pragma shader_feature _SIMPLE _LANTERN
			#pragma shader_feature _ALPHAMODE _EMISSIVEMODE _MASKMODE
			#pragma shader_feature _ISMASK_ON

			#define FORWARDADD_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "WorldBlending.cginc"

			ENDCG
		}
		Pass
		{
			Name "ShadowCaster"
            Tags {"LightMode"="ShadowCaster"}
            Offset 1,1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma shader_feature _SIMPLE _LANTERN
			#pragma shader_feature _ALPHAMODE

			#define SHADOWCASTER_PASS

			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "WorldBlending.cginc"

			ENDCG
		}
	}
	CustomEditor "LanternShaderGUI"
}
