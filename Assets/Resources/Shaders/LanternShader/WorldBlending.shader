Shader "Custom/Dissolve/WorldBlending"
{
	Properties
	{
		[Toggle(ISMASK)] _Mask("Alpha is Mask ?", Float) = 0

		[Header(First Texture (LIGHT))][Space]
		[NoScaleOffset]
		_FirstTexture	("Texture", 		2D)		= "white" {}
		_FirstLColor	("Color Bright",	Color)	= (1,1,1,1)
		_FirstDColor	("Color Dim", 		Color)	= (0,0,0,1)

		[Space(10)][Header(Second Texture (DARK))][Space]
		[NoScaleOffset]
		_SecondTexture	("Texture", 		2D)		= "white" {}
		_SecondLColor	("Color Bright", 	Color)	= (1,1,1,1)
		_SecondDColor	("Color Dim", 		Color)	= (0,0,0,1)

		[Space(10)][Header(Mask)][Space]
		_MaskTexture	("Mask",	 		2D)		= "white" {}
		_MaskRColor		("R Color", 		Color)	= (1,1,1,1)
		_MaskGColor		("G Color", 		Color)	= (0,0,0,1)

		[Space(10)][Header(Toon)][Space]
		_StepCount 	("Step",	Range(0,10))	= 3
		_Pow	 	("Pow",		Range(0,10))	= 1
	}

	SubShader
	{
		Tags {"Queue"="AlphaTest"
            "RenderType"="TransparentCutout"}

	    Cull Off

		Pass
		{
			Name "ForwardBase"
            Tags {"LightMode"="ForwardBase"}

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase_fullshadows
			#pragma shader_feature ISMASK
			#pragma target 3.0

			#define FORWARDBASE_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "worldBlending.cginc"

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
			#pragma multi_compile_fwdadd_fullshadows
			#pragma shader_feature ISMASK

			#define FORWARDADD_PASS
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "AutoLight.cginc"
			#include "worldBlending.cginc"

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
			#pragma shader_feature ISMASK

			#define SHADOWCASTER_PASS

			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"
			#include "worldBlending.cginc"

			ENDCG
		}
	}
}
