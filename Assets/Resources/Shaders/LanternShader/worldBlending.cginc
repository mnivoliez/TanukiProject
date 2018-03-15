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

uniform half _Freq;
uniform half _Speed;
uniform half _Interpolation;

uniform const int _LanternCount;
uniform float4 _Centers[5];
uniform float _Distances[5];

#if !defined(SHADOWCASTER_PASS) 
	uniform float4 _LightColor0;
	uniform sampler2D _MaskTexture;
	float4 _MaskTexture_ST;
	uniform fixed4 _FirstLColor;
	uniform fixed4 _FirstDColor;
	uniform fixed4 _SecondLColor;
	uniform fixed4 _SecondDColor;
	uniform fixed4 _MaskRColor;
	uniform fixed4 _MaskGColor;
	uniform half _StepCount;
	uniform half _Pow;
#endif

#if defined(FORWARDBASE_PASS)
	uniform half _Offset;
	uniform fixed4 _ColorDisso;
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

fixed4 frag (v2f i) : SV_Target
{
	float len = _Distances[0] - length(_Centers[0].xyz - i.posWorld.xyz);
    for(int id = 1; id < _LanternCount; id++) {
        len = max(len, _Distances[id] - length(_Centers[id].xyz - i.posWorld.xyz));
    }

	float3 wrldPos = i.posWorld.xyz * _Freq;
	wrldPos.y += _Time.x * _Speed;
	float ns = snoise(wrldPos);
	float lrp = 1.0-saturate(len + ns * _Interpolation);

	fixed4 tex1 = tex2D(_FirstTexture, i.uv0);
	fixed4 tex2 = tex2D(_SecondTexture, i.uv0);

	float isMask = 1.0;
	#ifdef ISMASK
		isMask = tex1.a;
	#else
		clip(lerp(tex1.a, tex2.a, lrp)-0.5);
	#endif

	#if !defined(SHADOWCASTER_PASS)
	    float3 attenColor = LIGHT_ATTENUATION(i) * _LightColor0.xyz;

	    float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz, _WorldSpaceLightPos0.w));
	    float nDotL = pow(max(0.0,dot(i.normalDir, lightDir)), _Pow) * attenColor;
	    float directDiff = floor(nDotL * _StepCount) / (_StepCount-0.5);
	    float3 brightDiff = directDiff * lerp(_FirstLColor.rgb,_SecondLColor.rgb, lrp);
	    float3 dimDiff = (1.0 - directDiff) * lerp(_FirstDColor.rgb, _SecondDColor.rgb, lrp);

	    #ifdef FORWARDBASE_PASS
	    	float3 indirectDiff = ShadeSH9(float4(i.normalDir, 1));
	    #endif

		fixed4 mask = 1-tex2D(_MaskTexture, TRANSFORM_TEX(i.uv0, _MaskTexture));
		float rMask = mask.r * _MaskRColor.a * pow(dot(i.normalDir, float3(0,1,0)),3.0);
        float gMask = mask.g * _MaskGColor.a;
		float3 diffCol = lerp(tex1.rgb * (1-rMask) + rMask * _MaskRColor.rgb, tex2.rgb * (1-gMask) + gMask * _MaskGColor.rgb, lrp) * isMask * attenColor;
    #endif

	#if defined(FORWARDBASE_PASS)
		float3 emissive = (1.0-saturate((len-_Offset) + ns * _Interpolation) - lrp) * _ColorDisso.rgb*(_ColorDisso.a*5.0);
		return fixed4((brightDiff + dimDiff + indirectDiff) * diffCol + emissive, 1.0);
	#elif defined(FORWARDADD_PASS)
		return fixed4((brightDiff + dimDiff) * diffCol, 1.0);
	#else
		SHADOW_CASTER_FRAGMENT(i);
	#endif
}		