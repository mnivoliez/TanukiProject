// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Lantern/complexShader" {

    Properties {
        [Header(First Texture (LIGHT))][Space]
		[NoScaleOffset]
		_FirstTexture	("Texture", 		2D)		= "white" {}
		_FirstLColor	("Color Bright",	Color)	= (1,1,1,1)
		_FirstDColor	("Color Dim", 		Color)	= (1,1,1,1)

		[Space(10)][Header(Second Texture (DARK))][Space]
		[NoScaleOffset]
		_SecondTexture	("Texture", 		2D)		= "white" {}
		_SecondLColor	("Color Bright", 	Color)	= (0,0,0,1)
		_SecondDColor	("Color Dim", 		Color)	= (0,0,0,1)

		[Space(10)][Header(Dissolve)][Space]
		_Color			("Color", 			Color)			= (0,0,1,1)
		_Interpolation	("Interpolation", 	Range(0,5))		= 2
		_Strength		("Strength", 		Range(0,1))		= 0.125
		_Falloff		("Falloff", 		Range(0.1,2))	= 0.1
		_Offset			("Offset", 			Range(-1,2))	= 0.25

		[Space(10)][Header(Noise)][Space]
		_Freq 	("Frequency", Float)	= 0.5
		_Speed 	("Speed", Float)		= 1

		[Space(10)][Header(Toon)][Space]
		_StepCount 	("Step",	Range(0,10))	= 3
		_Pow	 	("Pow",		Range(0,10))	= 1
	}

	SubShader {
		Tags {"RenderType"="Opaque"}
		LOD 100

		Pass{
			Name "ForwardBase"
            Tags {"LightMode"="ForwardBase"}

            Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase_fullshadows

			#define UNITY_PASS_FORWARDBASE

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "noiseSimplex.cginc"
            #include "lantern_utilities.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				LIGHTING_COORDS(3,4)
			};

			uniform sampler2D
				_FirstTexture,
				_SecondTexture
			;

			uniform fixed4
				_FirstLColor,
				_FirstDColor,
				_SecondLColor,
				_SecondDColor,
				_Color
			;

			uniform half
                _Freq,
				_Speed,
				_Interpolation,
				_Strength,
				_Falloff,
				_Offset,
				_StepCount,
				_Pow
			;

			uniform const int _numberOfCenters;
		    uniform float4 _centers[5];
            uniform float _distances[5];

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.uv;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float l = lanterns_intensity(i.posWorld, _numberOfCenters, _centers, _distances);

				float3 wrldPos = i.posWorld.xyz * _Freq;
				wrldPos.y += _Time.x * _Speed;

				float ns = snoise(wrldPos);

				float lrp = 1 - l; //saturate((l + ns * _Interpolation) * 1/_Falloff);

				fixed4 tex1 = tex2D(_FirstTexture, i.uv0);
				fixed4 tex2 = tex2D(_SecondTexture, i.uv0);

				float atten = LIGHT_ATTENUATION(i);
                float3 attenColor = atten * _LightColor0.xyz;
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float nDotL = pow(max(0.0,dot(i.normalDir, lightDir)), _Pow) * attenColor;
                float light = (floor(nDotL * _StepCount) / (_StepCount-0.5));
                float3 diff = light * lerp(_FirstLColor.rgb,_SecondLColor.rgb, lrp);
                float3 invDiff = (1.0 - light) * lerp(_FirstDColor.rgb, _SecondDColor.rgb, lrp);

				float4 col = float4(lerp(tex1.rgb, tex2.rgb, lrp) * attenColor, 1.0);
				float4 emissive = float4(lrp * saturate(1.0-(l-_Offset)) * col.rgb * saturate(ns*_Strength*10+_Strength*0.5f * (1.0/_Falloff)), 1);
				col.rgb *= diff + invDiff;

				clip(lerp(tex1.a, tex2.a, lrp)-0.5);
				return saturate(emissive + col);
			}
			ENDCG
		}
		Pass
		{
			Name "ForwardAdd"
            Tags {"LightMode"="ForwardAdd"}

            Cull Off
            Blend One One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

			#define UNITY_PASS_FORWARDADD

			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			#include "noiseSimplex.cginc"
            #include "lantern_utilities.cginc"
			#include "AutoLight.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float3 normalDir : TEXCOORD2;
				LIGHTING_COORDS(3,4)
			};

			uniform sampler2D
				_FirstTexture,
				_SecondTexture
			;

			uniform fixed4
				_FirstLColor,
				_FirstDColor,
				_SecondLColor,
				_SecondDColor,
				_Color
			;

			uniform half
				_Freq,
				_Speed,
				_Interpolation,
				_Strength,
				_Falloff,
				_Offset,
				_StepCount,
				_Pow
			;

			uniform const int _numberOfCenters;
		    uniform float4 _centers[5];
            uniform float _distances[5];

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.uv;
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o)
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float l = lanterns_intensity(i.posWorld, _numberOfCenters, _centers, _distances);

				float3 wrldPos = i.posWorld.xyz * _Freq;
				wrldPos.y += _Time.x * _Speed;

				float ns = snoise(wrldPos);

				float lrp = saturate((l + ns * _Interpolation) * 1/_Falloff);

				fixed4 tex1 = tex2D(_FirstTexture, i.uv0);
				fixed4 tex2 = tex2D(_SecondTexture, i.uv0);

				float atten = LIGHT_ATTENUATION(i);
                float3 attenColor = atten * _LightColor0.xyz;
                float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float nDotL = pow(max(0.0,dot(i.normalDir, lightDir)), _Pow) * attenColor;
                float light = (floor(nDotL * _StepCount) / (_StepCount-0.5));
                float3 diff = light * lerp(_FirstLColor.rgb,_SecondLColor.rgb, lrp);
                float3 invDiff = (1.0 - light) * lerp(_FirstDColor.rgb, _SecondDColor.rgb, lrp);

				float4 col = float4(lerp(tex1.rgb, tex2.rgb, 1.0-lrp) * attenColor, 1.0);
				col.rgb *= diff + invDiff;

				clip(lerp(tex1.a, tex2.a, lrp)-0.5);
				return saturate(col);
			}
			ENDCG
		}

		Pass
		{
			Name "ShadowCaster"
            Tags {"LightMode"="ShadowCaster"
            		"Queue"="AlphaTest"
            		"RenderType"="TransparentCutout"}
            Zwrite On ZTest LEqual Cull Off
            Offset 1,1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct v2f{
				V2F_SHADOW_CASTER;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o);
				return o;
			}

			float4 frag (v2f i) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i);
			}
			ENDCG
		}
	}
}
