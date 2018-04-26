Shader "Custom/Projector/ProjectorShadow" {
	Properties {}
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
			#pragma multi_compile_fog
			#include "UnityCG.cginc"

			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				float4 pos : SV_POSITION;
			};

			uniform float4x4 unity_Projector;
			uniform float4x4 unity_ProjectorClip;

			uniform float _Offset;

			v2f vert(float4 vertex : POSITION)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul (unity_Projector, vertex);
				//o.uvShadow.z = mul( unity_ProjectorClip, vertex);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				return o;
			}

			uniform sampler2D _ShadowTex;
			uniform fixed4 _ShadowColor;
			uniform sampler2D _FalloffTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed shadow = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				fixed falloff = tex2Dproj (_FalloffTex, UNITY_PROJ_COORD(i.uvFalloff)).a;
				fixed res = lerp(1, shadow, falloff);
				return 1 - (1-res) * (1-(i.uvFalloff.x*_Offset)) * (1-_ShadowColor);
			}
			ENDCG
		}
	}
}
