Shader "Hidden/TerrainEngine/Details/Vertexlit" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {  }
    }
    SubShader {
        Tags { "RenderType"="LanternGrass" "IgnoreProjector"="True"}
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert
        #include "noiseSimplex.cginc"

        sampler2D _MainTex;

        struct Input {
            fixed4 color : COLOR;
            float3 worldPos;
        };

        uniform half _Freq;
        uniform half _Speed;
        uniform half _Interpolation;
        uniform half _Offset;
        uniform fixed4 _ColorDisso;

        uniform int _LanternCount;
        uniform float4 _CentersLantern[20];
        uniform float _DistancesLantern[20];

        void surf (Input IN, inout SurfaceOutput o) {
            float len = -100;
            for(int id = 0; id < _LanternCount; id++) {
                len = max(len, _DistancesLantern[id] - length(_CentersLantern[id].xyz - IN.worldPos.xyz));
            }

            float3 wrldPos = IN.worldPos.xyz * _Freq;
            wrldPos.y += _Time.x * _Speed;
            float ns = snoise(wrldPos);
            float lrp = 1.0-saturate(len + ns * _Interpolation);

            o.Albedo = IN.color;
            o.Emission = (1.0-saturate((len-_Offset) + ns * _Interpolation) - lrp) * _ColorDisso.rgb*(_ColorDisso.a*25.0);
            clip(lerp(IN.color.a,1-IN.color.a,lrp)-0.5);
        }

        ENDCG
    }
    SubShader {
        Pass
        {
            Tags{ "LightMode" = "ShadowCaster" "RenderType" = "LanternGrass" "IgnoreProjector"="True"}
            Offset 1,1
            Cull Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
            #include "UnityCG.cginc"
            #include "noiseSimplex.cginc"

            struct appdata
            {
                float3 uv : TEXCOORD0;
                float3 pos : POSITION;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata IN)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.uv = IN.uv;
                o.pos = UnityObjectToClipPos(IN.pos);
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                clip(-1);
                return fixed4(0,0,0,0);
            }

            ENDCG
        }
    }

    Fallback "VertexLit"
}
