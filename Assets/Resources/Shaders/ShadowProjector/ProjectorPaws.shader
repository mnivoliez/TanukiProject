Shader "Custom/Projector/Paws" {
	Properties {
		_ShadowTex ("Texture", 2D) = "white" {}
		_FalloffTex ("FallOff", 2D) = "white" {}
		_ShadowColor ("Color", Color) = (1,1,1,1)
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				return o;
			}

			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			fixed4 _ShadowColor;
			fixed _PawProgress;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed falloff = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).a;
				fixed res = lerp(1, shadow, falloff);
				return 1-(1-res) * (1-_ShadowColor*25*_ShadowColor.a)/**_PawProgress*/;
			}
			ENDCG
		}
	}
}
