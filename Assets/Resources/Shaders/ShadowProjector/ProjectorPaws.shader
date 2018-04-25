Shader "Custom/Projector/Paws" {
	Properties {
		_ShadowTex ("Texture", 2D) = "white" {}
		_ShadowColor ("Color", Color) = (1,1,1,1)
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		Pass {
			Name "PASS"
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
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				return o;
			}

			sampler2D _ShadowTex;
			fixed4 _ShadowColor;
			fixed _PawProgress;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				return 1-(1-shadow) * (1-_ShadowColor*25*_ShadowColor.a)*_PawProgress;
			}
			ENDCG
		}
	}
}
