// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Lantern/complexShader" {

    Properties {
        [Header(Main_Texture)][Space]
        _MainStrength               ("Strength",                Range(1, 20))   = 1.842003
        _MainLightColor             ("Light Color",             Color)          = (0.9632353,0.9561527,0.9561527,1)
        _MainShadowcolor            ("Shadow color",            Color)          = (0.03443987,0.2389416,0.3602941,1)
        _MainThickness              ("Thickness",               Float )         = 0
        _MainTexture                ("Texture",                 2D)             = "white" {}
        _MainAuxTexture             ("Aux Texture",             2D)             = "white" {}


        [Space(10)][Header(Minor_Texture)][Space]
        _MinStrength                ("Strength",                Range(1, 20))   = 1.842003
        _MinLightColor              ("Light Color",             Color)          = (0.9632353,0.9561527,0.9561527,1)
        _MinShadowColor             ("Shadow color",            Color)          = (0.03443987,0.2389416,0.3602941,1)
        _MinThickness               ("Thickness",               Float )         = 0
        _MinTexture                 ("Texture",                 2D)             = "white" {}
        _MinAuxTexture              ("Aux Texture",             2D)             = "white" {}

        [Space(10)][Header(Common)][Space]
        _ShadowSize              ("Shadow Size",            Range(0, 1))    = 1
        _ShadowEffects           ("Shadow Effects",         Range(1, 5))    = 5
        [HideInInspector]_Cutoff ("Aux cutoff",             Range(0,1))     = 0.5

        [Space(10)][Header(Dissolve)][Space]
		_Interpolation	("Interpolation", 	Range(0,5))		= 2
		_Strength		("Strength", 		Range(0,1))		= 0.125
		_Falloff		("Falloff", 		Range(0.1,2))	= 0.1
		_Offset			("Offset", 			Range(-1,2))	= 0.25

        [Space(10)][Header(Noise)][Space]
		_Freq 	        ("Frequency",       Float)	        = 0.5
		_Speed 	        ("Speed",           Float)		    = 1
	}

	SubShader {
        Pass {
            Tags {"LightMode"="ForwardBase"}
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members lanterns_intensity)
            #pragma exclude_renderers d3d11
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #include "lantern_utilities.cginc"

            // compile shader into multiple variants, with and without shadows
            // (we don't care about any lightmaps yet, so skip these variants)
            #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
            // shadow helper functions and macros
            #include "AutoLight.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1) // put shadows data into TEXCOORD1
                fixed3 diff : COLOR0;
                fixed3 ambient : COLOR1;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diff = nl * _LightColor0.rgb;
                o.ambient = ShadeSH9(half4(worldNormal,1));
                // compute shadows data
                TRANSFER_SHADOW(o)
                return o;
            }

            uniform float _MainStrength;
            uniform float _MainShadowSize;
            uniform float4 _MainLightColor;
            uniform float4 _MainShadowColor;
            uniform float _MainShadowEffects;
            uniform float _MainThickness;
            uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
            uniform sampler2D _MainAuxTexture; uniform float4 _MainAuxTexture_ST;

            uniform float _Cutoff;

            uniform float _MinStrength;
            uniform float _MinShadowSize;
            uniform float4 _MinLightColor;
            uniform float4 _MinShadowColor;
            uniform float _MinShadowEffects;
            uniform float _MinThickness;
            uniform sampler2D _MinTexture; uniform float4 _MinTexture_ST;
            uniform sampler2D _MinAuxTexture; uniform float4 _MinAuxTexture_ST;


            uniform const int _numberOfCenters;
            uniform float4 _centers[5];
            uniform float _distances[5];

            uniform half _Freq;
            uniform half _Speed;
            uniform half _Interpolation;
            uniform half _Strength;
            uniform half _Falloff;



            fixed4 frag (v2f i) : SV_Target
            {
                float intensity = lanterns_intensity(i.pos, _numberOfCenters, _centers, _distances);

                float4 mainTexture = tex2D(_MainTexture,
                    TRANSFORM_TEX(i.uv, _MainTexture));
                float3 mainNormalLocal = mainTexture.rgb;

                float4 minTexture = tex2D(_MinTexture,
                    TRANSFORM_TEX(i.uv, _MinTexture));
                float3 minNormalLocal = minTexture.rgb;

                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                float4 mainAuxTexture = tex2D(_MainAuxTexture,TRANSFORM_TEX(i.uv, _MainAuxTexture));

                float4 minAuxTexture = tex2D(_MinAuxTexture,TRANSFORM_TEX(i.uv, _MinAuxTexture));

                float4 col = lerp(_MainShadowColor, _MinShadowColor, intensity);

                // compute shadow attenuation (1.0 = fully lit, 0.0 = fully shadowed)
                fixed shadow = SHADOW_ATTENUATION(i);
                // darken light's illumination with shadow, keep ambient intact
                fixed3 lighting = i.diff * shadow + i.ambient;
                //col.rgb *= lighting;
                return col;
            }
            ENDCG
        }

        // shadow casting support
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"

    }

		/*Tags { "RenderType"="TransparentCutout" "Queue"="Transparent"}
        Cull Off
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows Aux:fade vertex:vert
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
        #include "lanterns_utilities.cginc"

        uniform float _MainStrength;
        uniform float _MainShadowSize;
        uniform float4 _MainLightColor;
        uniform float4 _MainShadowColor;
        uniform float _MainShadowEffects;
        uniform float _MainThickness;
        uniform sampler2D _MainTexture; uniform float4 _MainTexture_ST;
        uniform sampler2D _MainAuxTexture; uniform float4 _MainAuxTexture_ST;

        uniform float _Cutoff;

        uniform float _MinStrength;
        uniform float _MinShadowSize;
        uniform float4 _MinLightColor;
        uniform float4 _MinShadowColor;
        uniform float _MinShadowEffects;
        uniform float _MinThickness;
        uniform sampler2D _MinTexture; uniform float4 _MinTexture_ST;
        uniform sampler2D _MinAuxTexture; uniform float4 _MinAuxTexture_ST;


		struct Input {
			float2 uv_MainTexture;
            float2 uv_MinTexture;
            float2 uv_MainAuxTexture;
            float2 uv_MinAuxTexture;
            float3 worldPos;
		};

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

            float3 wrldPos = IN.worldPos.xyz * _Freq;
            wrldPos.y += _Time.x * _Speed;

            float ns = snoise(wrldPos);

            float lrp = saturate((l + ns * _Interpolation) /_Falloff);

            float4 mainTexture = tex2D(_MainTexture,
                TRANSFORM_TEX(IN.uv_MainTexture, _MainTexture));
            float3 mainNormalLocal = mainTexture.rgb;

            float4 minTexture = tex2D(_MinTexture,
                TRANSFORM_TEX(IN.uv_MinTexture, _MinTexture));
            float3 minNormalLocal = minTexture.rgb;

            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

            float4 mainAuxTexture = tex2D(_MainAuxTexture,TRANSFORM_TEX(IN.uv_MainAuxTexture, _MainAuxTexture));

            float4 minAuxTexture = tex2D(_MinAuxTexture,TRANSFORM_TEX(IN.uv_MinAuxTexture, _MinAuxTexture));

		}
		ENDCG
	}*/

	FallBack "Diffuse"
}
