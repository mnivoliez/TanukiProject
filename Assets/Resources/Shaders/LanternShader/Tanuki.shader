Shader "Custom/Lantern/Tanuki"
{
	Properties
	{
		_FirstTexture	("", 2D)			= "white" {}
		_FirstLColor	("", Color)			= (1,1,1,0)
		_FirstDColor	("", Color)			= (0,0,0,0)

		_EmissiveColor		("", Color)		= (1,1,1,1)
		_EmissiveIntensity	("", Range(0,1))= 1

		[PowerSlider(2.0)] 
		_SpecIntensity	("", Range(0,30))	= 0
		[PowerSlider(2.0)] 
		_SpecPow		("", Range(0.01,20))= 1
		[PowerSlider(2.0)] 
		_RimIntensity	("", Range(0,50))	= 0
		[PowerSlider(2.0)] 
		_RimPow			("", Range(0.01,20))= 1

		_StepCount		("", Range(0,20))	= 3
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

			#define FORWARDBASE_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "Tanuki.cginc"

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

			#define FORWARDADD_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "Tanuki.cginc"

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

			#define SHADOWCASTER_PASS

			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "Tanuki.cginc"

			ENDCG
		}
	}
	CustomEditor "LanternShaderGUI"
}
