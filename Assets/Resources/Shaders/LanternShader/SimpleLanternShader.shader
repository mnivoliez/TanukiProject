// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Lantern/simpleShader" {

    Properties {
		_MainTex        ("Main Albedo (RGB)",   2D)         = "white" {}
        _MainColor		("Main Color", 	        Color)	    = (1,1,1,1)
        _MinTex         ("Min Albedo (RGB)",    2D)         = "white" {}
        _MinColor		("Min Color", 	        Color)	    = (1,1,1,1)
		_MainGlossiness ("Main Smoothness",     Range(0,1)) = 0.5
        _MinGlossiness  ("Min Smoothness",      Range(0,1)) = 0.5
		_MainMetallic   ("Main Metallic",       Range(0,1)) = 0.0
        _MinMetallic    ("Min Metallic",        Range(0,1)) = 0.0

        [Space(10)][Header(Dissolve)][Space]
		//_Color			("Color", 			Color)			= (0,0,1,1)
		_Interpolation	("Interpolation", 	Range(0,5))		= 2
		_Strength		("Strength", 		Range(0,1))		= 0.125
		_Falloff		("Falloff", 		Range(0.1,2))	= 0.1
		_Offset			("Offset", 			Range(-1,2))	= 0.25

        [Space(10)][Header(Noise)][Space]
		_Freq 	        ("Frequency",       Float)	        = 0.5
		_Speed 	        ("Speed",           Float)		    = 1
	}

	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="Transparent"}
        Cull Off
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:fade
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
        #include "lantern_lerp.cginc"


		sampler2D _MainTex;
        sampler2D _MinTex;

        fixed4 _MainColor;
		fixed4 _MinColor;

		struct Input {
			float2 uv_MainTex;
            float2 uv_MinTex;
            float3 worldPos;
		};

		half _MainGlossiness;
        half _MinGlossiness;
		half _MainMetallic;
        half _MinMetallic;

        uniform half _Freq;
		uniform half _Speed;
		uniform half _Interpolation;
		uniform half _Strength;
		uniform half _Falloff;

        uniform const int _numberOfCenters;
		uniform float4 _centers[5];
        uniform float _distances[5];


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
            float l = lerp_lantern(IN.worldPos.xyz, _numberOfCenters, _centers, _distances);

			// Albedo comes from a texture tinted by color
			fixed4 mainAlbedo = tex2D (_MainTex, IN.uv_MainTex) * _MainColor;
            fixed4 minAlbedo = tex2D (_MinTex, IN.uv_MinTex) * _MinColor;

            float3 wrldPos = IN.worldPos.xyz * _Freq;
            wrldPos.y += _Time.x * _Speed;

            float ns = snoise(wrldPos);

            float lrp = saturate((l + ns * _Interpolation) /_Falloff);

            fixed4 c = lerp(mainAlbedo, minAlbedo, 1-lrp);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
            fixed4 metallic = lerp(_MainMetallic, _MinMetallic, lrp);
			o.Metallic = metallic;
            fixed4 glossiness = lerp(_MainGlossiness, _MinGlossiness, lrp);
			o.Smoothness = glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}

	FallBack "Diffuse"
}
