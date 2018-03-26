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
	#endif

	float2 uv0 : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
};

uniform sampler2D _FirstTexture;
uniform sampler2D _SecondTexture;

float4 _FirstTexture_ST;

#if defined(_LANTERN)
	uniform half _Freq;
	uniform half _Speed;
	uniform half _Interpolation;

	uniform int _LanternCount;
	uniform float4 _Centers[5];
	uniform float _Distances[5];
#endif

#if !defined(SHADOWCASTER_PASS) 
	uniform float4 _LightColor0;
	uniform fixed4 _FirstLColor;
	uniform fixed4 _FirstDColor;

	uniform half _SpecIntensity;
	uniform half _SpecPow;
	uniform half _SpecStep;
	uniform half _RimIntensity;
	uniform half _RimPow;
	uniform half _RimStep;

	uniform half _StepCount;

	#if defined(_LANTERN)
		uniform fixed4 _SecondLColor;
		uniform fixed4 _SecondDColor;
		#if defined(_ISMASK_ON)
			uniform sampler2D _MaskTexture;
			float4 _MaskTexture_ST;
			uniform fixed4 _MaskRColor;
			uniform fixed4 _MaskGColor;
		#endif
	#endif
#endif

#if defined(FORWARDBASE_PASS)
	#if defined(_LANTERN)
		uniform half _Offset;
		uniform fixed4 _ColorDisso;
		#if defined(_ISMASK_ON)
			uniform half _MaskREmi;
			uniform half _MaskGEmi;
		#endif
	#endif
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
	#else
		TRANSFER_SHADOW_CASTER(o);
	#endif

	return o;
}

half4 frag (v2f i) : SV_Target
{
	#if defined(_LANTERN)
		float len = -100;
		if(_LanternCount != 0) {
		    for(int id = 0; id < _LanternCount; id++) {
		        len = max(len, _Distances[id] - length(_Centers[id].xyz - i.posWorld.xyz));
		    }
		}

		float3 wrldPos = i.posWorld.xyz * _Freq;
		wrldPos.y += _Time.x * _Speed;
		float ns = snoise(wrldPos);
		float lrp = 1.0-saturate(len + ns * _Interpolation);
	#endif

	fixed4 tex1 = tex2D(_FirstTexture, TRANSFORM_TEX (i.uv0, _FirstTexture));
	fixed4 tex2 = tex2D(_SecondTexture, TRANSFORM_TEX (i.uv0, _FirstTexture));

	float isMask = 1.0;
	#if defined(_ISALPHA_ON)
		isMask = tex1.a;
	#else
		#if defined(_LANTERN)
			clip(lerp(tex1.a, tex2.a, lrp)-0.5);
		#else
			clip(tex1.a-0.5);
		#endif
	#endif

	#if !defined(SHADOWCASTER_PASS)
	    float3 attenColor = LIGHT_ATTENUATION(i) * _LightColor0.rgb;

	    float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz, _WorldSpaceLightPos0.w));

	    float NdotL = saturate(dot(i.normalDir, lightDir));
	    float3 stepDiff = floor(NdotL * _StepCount) / (_StepCount-0.5);

	    #if defined(_LANTERN)
			float3 brightDiff = stepDiff * lerp(_FirstLColor.rgb,_SecondLColor.rgb, lrp);
		    float3 dimDiff = (1.0 - stepDiff) * lerp(_FirstDColor.rgb, _SecondDColor.rgb, lrp);
		#else
		    float3 brightDiff = stepDiff * _FirstLColor.rgb;
		    float3 dimDiff = (1.0 - stepDiff) * _FirstDColor.rgb;
		#endif
		float3 directDiff = (brightDiff + dimDiff) * attenColor;

		#if defined(_LANTERN)
			#if defined(_ISMASK_ON)
				fixed4 mask = 1-tex2D(_MaskTexture, TRANSFORM_TEX(i.uv0, _MaskTexture));
				float rMask = mask.r * _MaskRColor.a * saturate(pow(dot(i.normalDir, float3(0,1,0)),3.0));
		        float gMask = mask.g * _MaskGColor.a;
			    float3 diffCol = lerp(tex1.rgb * (1-rMask) + rMask * _MaskRColor.rgb, tex2.rgb * (1-gMask) + gMask * _MaskGColor.rgb, lrp);
			#else
				float3 diffCol = lerp(tex1.rgb, tex2.rgb, lrp);
			#endif
		#else
			float3 diffCol = tex1.rgb;
		#endif

		float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	    float3 halfDir = normalize(lightDir + viewDir);
		float3 reflLightDir = reflect(-lightDir, i.normalDir);
		float NdotH = floor(saturate(dot(i.normalDir, halfDir)) * _StepCount*2) / (_StepCount*2-0.5);
		float VdotR = saturate(dot(viewDir, reflLightDir)); 
		float NdotV = saturate(dot(i.normalDir, viewDir));

		float rim = LIGHT_ATTENUATION(i) * pow(1-NdotV, 4) * _RimIntensity * pow(VdotR, _RimPow);

		float3 spec = attenColor * diffCol * _SpecIntensity * pow(NdotH, _SpecPow);
    #endif

	#if defined(FORWARDBASE_PASS)
	    float3 indirectDiff = ShadeSH9(float4(i.normalDir, 1));
		float3 emissive = 0;
		#if defined(_LANTERN)
			emissive += (stepDiff * diffCol.rgb * lerp(_FirstLColor.rgb*_FirstLColor.a, _SecondLColor.rgb*_SecondLColor.a, lrp)*5.0)
				   + (1-stepDiff) * diffCol.rgb * lerp(_FirstDColor.rgb*_FirstDColor.a, _SecondDColor.rgb*_SecondDColor.a, lrp)*5.0;
			emissive += (1.0-saturate((len-_Offset) + ns * _Interpolation) - lrp) * _ColorDisso.rgb*(_ColorDisso.a*100.0);
		    #if defined(_ISMASK_ON)
				emissive += lerp(rMask * _MaskRColor.rgb * (_MaskREmi-1), gMask * _MaskGColor.rgb * (_MaskGEmi-1), lrp)*5.0;
			#endif
		#else
			emissive += (stepDiff * diffCol.rgb * _FirstLColor.rgb*_FirstLColor.a*5.0)
				   + (1-stepDiff) * diffCol.rgb*_FirstDColor.rgb*_FirstDColor.a*5.0;
		#endif
		return half4(((directDiff + indirectDiff + spec + rim) * diffCol + emissive) * isMask, 1.0);
	#elif defined(FORWARDADD_PASS)
		return half4((directDiff * diffCol + attenColor/100) * isMask, 1.0);
	#else
		SHADOW_CASTER_FRAGMENT(i);
	#endif
}