struct appdata
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 tangent : TANGENT;
	float2 texcoord0 : TEXCOORD0;
	float2 texcoord1 : TEXCOORD1;
	float2 texcoord2 : TEXCOORD2;
};

struct v2f
{
	float2 uv0 : TEXCOORD0;
	float4 posWorld : TEXCOORD1;
	float3 normalDir : TEXCOORD2;
	float4 pos : SV_POSITION;
	LIGHTING_COORDS(3,4)
	float4 projPos : TEXCOORD5;
};

uniform sampler2D _FirstTexture;
uniform sampler2D _SecondTexture;

float4 _FirstTexture_ST;

uniform float4 _LightColor0;
uniform fixed4 _FirstLColor;

#if defined(_LANTERN)
	uniform half _Freq;
	uniform half _Speed;
	uniform half _Interpolation;

	uniform int _LanternCount;
	uniform float4 _Centers[20];
	uniform float _Distances[20];

	uniform fixed4 _SecondLColor;

	#if defined(FORWARDBASE_PASS)
			uniform half _Offset;
			uniform fixed4 _ColorDisso;
	#endif
#endif

uniform sampler2D _CameraDepthTexture;

uniform fixed4 _FirstFoamColor;
uniform fixed4 _SecondFoamColor;

uniform half _Depth;
uniform fixed _EdgeIntensity;

uniform half _WaveDir;
uniform half _WaveSpeed;
uniform half _WaveAmount;
uniform half _WaveHeight;

uniform float _Tess;

uniform sampler2D _NoiseTexture;
uniform half _NoiseIntensity;
uniform float4 _NoiseTexture_ST;

v2f vert (appdata v)
{
	v2f o;

	o.uv0 = v.texcoord0;
	_WaveDir *= 0.0174532925;
	float2 waveDir = float2(cos(_WaveDir), sin(_WaveDir));
	float2 waves = sin((_Time.y * waveDir.x * _WaveSpeed - v.vertex.x * _WaveAmount * abs(waveDir.x))
					+ (_Time.y * waveDir.y * _WaveSpeed - v.vertex.z * _WaveAmount * abs(waveDir.y)));
	float2 panner = (o.uv0 + _Time.x * waveDir * _WaveSpeed);
	float4 noise = tex2Dlod(_NoiseTexture, float4(TRANSFORM_TEX (panner, _NoiseTexture).xy,0,0));
	v.vertex.y += waves * _WaveHeight + ((noise.r-0.5) * _NoiseIntensity);
	o.posWorld = mul(unity_ObjectToWorld, v.vertex);
	o.pos = UnityObjectToClipPos(v.vertex);
	o.normalDir = UnityObjectToWorldNormal(v.normal);
	o.projPos = ComputeScreenPos (o.pos);
    COMPUTE_EYEDEPTH(o.projPos.z);
	TRANSFER_VERTEX_TO_FRAGMENT(o);
	return o;
}

#ifdef UNITY_CAN_COMPILE_TESSELLATION
    struct TessVertex {
        float4 vertex : INTERNALTESSPOS;
        float3 normal : NORMAL;
        float4 tangent : TANGENT;
        float2 texcoord0 : TEXCOORD0;
        float2 texcoord1 : TEXCOORD1;
        float2 texcoord2 : TEXCOORD2;
    };
    struct OutputPatchConstant {
        float edge[3]         : SV_TessFactor;
        float inside          : SV_InsideTessFactor;
        float3 vTangent[4]    : TANGENT;
        float2 vUV[4]         : TEXCOORD;
        float3 vTanUCorner[4] : TANUCORNER;
        float3 vTanVCorner[4] : TANVCORNER;
        float4 vCWts          : TANWEIGHTS;
    };
    TessVertex tessvert (appdata v) {
        TessVertex o;
        o.vertex = v.vertex;
        o.normal = v.normal;
        o.tangent = v.tangent;
        o.texcoord0 = v.texcoord0;
        o.texcoord1 = v.texcoord1;
        o.texcoord2 = v.texcoord2;
        return o;
    }
    float Tessellation(appdata v){
        return _Tess;
    }
    float4 Tessellation(TessVertex v, TessVertex v1, TessVertex v2){
        float tv = Tessellation(v);
        float tv1 = Tessellation(v1);
        float tv2 = Tessellation(v2);
        return float4( tv1+tv2, tv2+tv, tv+tv1, tv+tv1+tv2 ) / float4(2,2,2,3);
    }
    OutputPatchConstant hullconst (InputPatch<TessVertex,3> v) {
        OutputPatchConstant o = (OutputPatchConstant)0;
        float4 ts = Tessellation( v[0], v[1], v[2] );
        o.edge[0] = ts.x;
        o.edge[1] = ts.y;
        o.edge[2] = ts.z;
        o.inside = ts.w;
        return o;
    }
    [domain("tri")]
    [partitioning("fractional_odd")]
    [outputtopology("triangle_cw")]
    [patchconstantfunc("hullconst")]
    [outputcontrolpoints(3)]
    TessVertex hull (InputPatch<TessVertex,3> v, uint id : SV_OutputControlPointID) {
        return v[id];
    }
    [domain("tri")]
    v2f domain (OutputPatchConstant tessFactors, const OutputPatch<TessVertex,3> vi, float3 bary : SV_DomainLocation) {
        appdata v = (appdata)0;
        v.vertex = vi[0].vertex*bary.x + vi[1].vertex*bary.y + vi[2].vertex*bary.z;
        v.normal = vi[0].normal*bary.x + vi[1].normal*bary.y + vi[2].normal*bary.z;
        v.tangent = vi[0].tangent*bary.x + vi[1].tangent*bary.y + vi[2].tangent*bary.z;
        v.texcoord0 = vi[0].texcoord0*bary.x + vi[1].texcoord0*bary.y + vi[2].texcoord0*bary.z;
        v.texcoord1 = vi[0].texcoord1*bary.x + vi[1].texcoord1*bary.y + vi[2].texcoord1*bary.z;
        v2f o = vert(v);
        return o;
    }
#endif


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

	_WaveDir *= 0.0174532925;
	float2 waveDir = float2(cos(_WaveDir), sin(_WaveDir));
	float2 panner = (i.uv0 + _Time.x * waveDir * _WaveSpeed);
	fixed4 tex1 = tex2D(_FirstTexture, TRANSFORM_TEX (panner, _FirstTexture));
	fixed4 tex2 = tex2D(_SecondTexture, TRANSFORM_TEX (panner, _FirstTexture));

    float3 attenColor = LIGHT_ATTENUATION(i) * _LightColor0.rgb;
    float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz, _WorldSpaceLightPos0.w));

    float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
	float partZ = max(0,i.projPos.z - _ProjectionParams.g);

	float VdotN = saturate(dot(viewDirection,i.normalDir));

	float edgeDetect = saturate(saturate((sceneZ-partZ)/_Depth) * 2 * VdotN + 0.5);
	float edgeRemap = (edgeDetect - 0.5) * 2;
	float invEdgeIntensity = pow(1-edgeRemap, 5.0);

    float NdotL = max(0.0,dot(i.normalDir, lightDir));
    float3 directDiff = NdotL * attenColor;

    half3 firstWaterColor = _FirstLColor * (1-(tex1 + invEdgeIntensity)) + (tex1 + invEdgeIntensity) * _FirstFoamColor;
	#if defined(_LANTERN)
    	half3 secondWaterColor = _SecondLColor * (1-(tex2 + invEdgeIntensity)) + (tex2 + invEdgeIntensity) * _SecondFoamColor;
		float3 diffCol = saturate(lerp(firstWaterColor, secondWaterColor, lrp));
		fixed opacity = saturate(invEdgeIntensity * _EdgeIntensity + edgeRemap * lerp(_FirstLColor.a, _SecondLColor.a, lrp));
	#else
    	float3 diffCol = firstWaterColor;
		fixed opacity = saturate(invEdgeIntensity * _EdgeIntensity + edgeRemap * _FirstLColor.a);
	#endif

	#if defined(FORWARDBASE_PASS)
	    float3 indirectDiff = ShadeSH9(float4(i.normalDir, 1));
		float3 emissive = 0;
		#if defined(_LANTERN)
			emissive += (1.0-saturate((len-_Offset) + ns * _Interpolation) - lrp) * _ColorDisso.rgb*(_ColorDisso.a*100.0);
			emissive += lerp((tex1+invEdgeIntensity) * _FirstFoamColor*_FirstFoamColor.a, (tex2+invEdgeIntensity) * _SecondFoamColor*_SecondFoamColor.a, lrp) *100.0;
		#else
			emissive += (tex1 + invEdgeIntensity) * _FirstFoamColor*_FirstFoamColor.a*100.0;
		#endif
		return half4((directDiff + indirectDiff) * diffCol + emissive, opacity);
	#else
		return half4(directDiff * diffCol, opacity);
	#endif
}