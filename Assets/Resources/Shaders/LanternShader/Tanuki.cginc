	struct appdata
{
	float4 vertex : POSITION;
	#if !defined(SHADOWCASTER_PASS)
		float3 normal : NORMAL;
	#endif
	float2 uv : TEXCOORD0;
};

struct v2f
{
	#if defined(SHADOWCASTER_PASS)
		V2F_SHADOW_CASTER;
	#else
		float3 normalDir : TEXCOORD2;
		float4 pos : SV_POSITION;
		LIGHTING_COORDS(3,4)
		UNITY_FOG_COORDS(5)
	#endif

	float2 uv0 : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
};

uniform sampler2D _FirstTexture;

float4 _FirstTexture_ST;

#if !defined(SHADOWCASTER_PASS)
	uniform float4 _LightColor0;
	uniform fixed4 _FirstLColor;
	uniform fixed4 _FirstDColor;

	uniform fixed4 _EmissiveColor;
	uniform fixed _EmissiveIntensity;
	uniform half _EmissiveSpeed;
	uniform half _EmissiveStrength;

	uniform half4 _InvincibilityColor;

	uniform half _SpecIntensity;
	uniform half _SpecPow;
	uniform half _SpecStep;
	uniform half _RimIntensity;
	uniform half _RimPow;
	uniform half _RimStep;

	uniform half _StepCount;
#endif

v2f vert (appdata v)
{
	v2f o;

	o.uv0 = v.uv;
	o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);

	#if !defined(SHADOWCASTER_PASS)
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		TRANSFER_VERTEX_TO_FRAGMENT(o);
		UNITY_TRANSFER_FOG(o,o.pos);
	#else
		TRANSFER_SHADOW_CASTER(o);
	#endif

	return o;
}

half4 frag (v2f i) : SV_Target
{
	fixed4 tex = tex2D(_FirstTexture, TRANSFORM_TEX (i.uv0, _FirstTexture));

	#if !defined(SHADOWCASTER_PASS)
	    float3 attenColor = LIGHT_ATTENUATION(i) * _LightColor0.rgb;

	    float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz, _WorldSpaceLightPos0.w));

	    float NdotL = saturate(dot(i.normalDir, lightDir));
	    float3 stepDiff = floor(NdotL * _StepCount) / (_StepCount-0.5);

		float3 brightDiff = stepDiff * _FirstLColor.rgb;
		float3 dimDiff = (1.0 - stepDiff) * _FirstDColor.rgb;

		float3 directDiff = (brightDiff + dimDiff) * attenColor;

		float mask = tex.a * _EmissiveColor.a * _EmissiveIntensity;
		float3 diffCol = tex.rgb;

		#if defined(FORWARDBASE_PASS)
			float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
			float3 halfDir = normalize(lightDir + viewDir);
			float3 reflLightDir = reflect(-lightDir, i.normalDir);
			float NdotH = floor(saturate(dot(i.normalDir, halfDir)) * _StepCount*2) / (_StepCount*2-0.5);
			float VdotR = saturate(dot(viewDir, reflLightDir));
			float NdotV = saturate(dot(i.normalDir, viewDir));

			float rim = LIGHT_ATTENUATION(i) * pow(1-NdotV, 4) * _RimIntensity * pow(VdotR, _RimPow);

			float3 spec = attenColor * diffCol * _SpecIntensity * pow(NdotH, _SpecPow);
		#endif
    #endif

	#if defined(FORWARDBASE_PASS)
	    float3 indirectDiff = ShadeSH9(float4(i.normalDir, 1));
		half3 emissive = (sin(_Time.y*_EmissiveSpeed)/(1/_EmissiveStrength)+(1-_EmissiveStrength)) * tex.a * _EmissiveColor.rgb * _EmissiveColor.a *25 * _EmissiveIntensity;
		half3 worldNoise = snoise(i.posWorld.xyz - half3(0,_Time.y*1.5,0));
		emissive += (sin(_Time.y*10)/4+.75) * (worldNoise*.5+.5) * _InvincibilityColor.rgb * _InvincibilityColor.a;
		emissive += (stepDiff * diffCol.rgb * _FirstLColor.rgb*_FirstLColor.a*25.0)
				+ (1-stepDiff) * diffCol.rgb * _FirstDColor.rgb*_FirstDColor.a*25.0;
		half4 finalCol = half4(((directDiff + indirectDiff + spec + rim) * diffCol + emissive), 1.0);
		UNITY_APPLY_FOG(i.fogCoord, finalCol);
		return finalCol;
	#elif defined(FORWARDADD_PASS)
		half4 finalCol = half4((directDiff * diffCol + attenColor/100), 1.0);
		UNITY_APPLY_FOG_COLOR(i.fogCoord, finalCol, fixed4(0,0,0,0));
		return finalCol;
	#else
		SHADOW_CASTER_FRAGMENT(i);
	#endif
}