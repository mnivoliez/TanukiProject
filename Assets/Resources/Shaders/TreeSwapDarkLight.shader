
Shader "Custom/Dissolve/TreeCorrupted" {
	Properties {
		[Header(Dark texture)][Space]
		_DarkSizeFull ("Dark Size Full", Float ) = 120
        _DarkStrength ("Dark Strength", Range(1, 20)) = 1.842003
        _DarkBaseTexture01 ("Dark Base Texture 01", 2D) = "white" {}
        _DarkShadowSize ("Dark Shadow Size", Range(0, 1)) = 1
        _DarkShadowColor ("Dark Shadow Color", Color) = (0.9632353,0.9561527,0.9561527,1)
        _DarkGlobalColo ("Dark Teinte", Color) = (0.03443987,0.2389416,0.3602941,1)
        _DarkShadowEffects ("Dark Shadow Effects", Range(1, 5)) = 5
        _DarkThickness ("Dark Thickness", Float ) = 0
        _DarkBaseTexture02 ("Dark Base Texture 02", 2D) = "white" {}
        _DarkAlphaTexture ("Dark Alpha Texture", 2D) = "white" {}

		[Space(10)][Header(Light texture)][Space]
		[NoScaleOffset]
		_LightStrength ("Light Strength", Range(1, 20)) = 1.842003
        _LightShadowSize ("Light Shadow Size", Range(0, 1)) = 1
        _LightShadowColor ("Light Shadow Color", Color) = (0.9632353,0.9561527,0.9561527,1)
        _LightGlobalcolor ("Light Global color", Color) = (0.03443987,0.2389416,0.3602941,1)
        _LightShadowEffects ("Light Shadow Effects", Range(1, 5)) = 5
        _LightThickness ("Light Thickness", Float ) = 0
        _LightBasetexture ("Light Base texture", 2D) = "white" {}
        _LightNormalMap ("Light Normal Map", 2D) = "bump" {}

		[Space(10)][Header(Dissolve)][Space]
		_DissolveColor			("Color", 			Color)			= (0,0,1,1)
		_DissolveInterpolation	("Interpolation", 	Range(0,5))		= 2
		_DissolveStrength		("Strength", 		Range(0,1))		= 0.125
		_DissolveFalloff		("Falloff", 		Range(0.1,2))	= 0.1
		_DissolveOffset			("Offset", 			Range(-1,2))	= 0.25

		[Space(10)][Header(Noise)][Space]
		_NoiseFreq 	("Frequency", Float)	= 0.5
		_NoiseSpeed 	("Speed", Float)		= 1
	}

    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "noiseSimplex.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu
            #pragma target 3.0

            uniform float _LightStrength;
            uniform float _LightShadowSize;
            uniform float4 _LightShadowColor;
            uniform float4 _LightGlobalcolor;
            uniform float _LightShadowEffects;
            uniform float _LightThickness;
            uniform sampler2D _LightBasetexture; uniform float4 _LightBasetexture_ST;
            uniform sampler2D _LightNormalMap; uniform float4 _LightNormalMap_ST;

            uniform float _DarkSizeFull;
            uniform float _DarkStrength;
            uniform sampler2D _DarkBaseTexture01; uniform float4 _DarkBaseTexture01_ST;
            uniform float _DarkShadowSize;
            uniform float4 _DarkShadowColor;
            uniform float4 _DarkGlobalColor;
            uniform float _DarkShadowEffects;
            uniform float _DarkThickness;
            uniform sampler2D _DarkBaseTexture02; uniform float4 _DarkBaseTexture02_ST;
            uniform sampler2D _DarkAlphaTexture; uniform float4 _DarkAlphaTexture_ST;

            uniform half
                _Freq,
                _Speed,
                _Interpolation,
                _Strength,
                _Falloff,
                _Offset;

            uniform const int _numberOfCenters;
            uniform float4 _centers[5];
            uniform float _distances[5];

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float lantern_lerp: TEXCOORD6;
                LIGHTING_COORDS(7,8)
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = normalize(UnityObjectToWorldNormal(v.normal));
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.screenPos = o.pos;

                float l = _distances[0] - length(_centers[0].xyz - o.posWorld.xyz);
                for(int index = 1; index < _numberOfCenters; ++index) {
                    float l_temp = _distances[index] - length(_centers[index].xyz - o.posWorld.xyz);
                    l = max(l, l_temp);
                }

                float3 wrldPos = o.posWorld.xyz * _Freq;
                wrldPos.y += _Time.x * _Speed;

                float ns = snoise(wrldPos);

                float lrp = saturate((l + ns * _Interpolation) * 1/_Falloff);

                o.lantern_lerp = lrp;
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

            float3 dark_color(VertexOutput i, float attenuation, float3 lightDirection, float3 normalDirection, float light_power,float shadow_size_attenuation_biggest, float3 viewDirection): COLOR {
                float4 _AlphaTexture_var = tex2D(_DarkAlphaTexture,TRANSFORM_TEX(i.uv0, _DarkAlphaTexture));
                float node_8652 = (abs(sin(((i.screenPos.rg+(1.0-max(0,dot(normalDirection, viewDirection))))*_DarkSizeFull))).r*_AlphaTexture_var.a);
                float node_5833 = 0.0;
                float node_5452 = (_DarkThickness*light_power);
                float4 _BaseTexture02_var = tex2D(_DarkBaseTexture02,TRANSFORM_TEX(i.uv0, _DarkBaseTexture02));

                float3 finalColor = saturate(
                    (lerp(_BaseTexture02_var.rgb,
                    _DarkShadowColor.rgb,
                    (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1) * shadow_size_attenuation_biggest)) > 0.5 ?
                        (1.0-(1.0-2.0*(lerp(_BaseTexture02_var.rgb,
                            _DarkShadowColor.rgb,
                            (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1)*shadow_size_attenuation_biggest))-0.5))
                            * (1.0-lerp(_DarkGlobalColor
            .rgb,
                                _DarkShadowColor.rgb,
                                (step(node_8652,smoothstep( node_5833, step(node_5452,node_8652), light_power ))
                                * shadow_size_attenuation_biggest
                                * step(node_8652,smoothstep( node_5833, step(node_5452,node_8652), light_power )))))) :
                        (2.0 * lerp(_BaseTexture02_var.rgb,
                            _DarkShadowColor.rgb,
                            (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1)*shadow_size_attenuation_biggest))
                            * lerp(_DarkGlobalColor
            .rgb,
                                _DarkShadowColor.rgb,
                                (step(node_8652,smoothstep( node_5833, step(node_5452,node_8652), light_power ))
                                * shadow_size_attenuation_biggest
                                * step(node_8652,smoothstep( node_5833, step(node_5452,node_8652), light_power ))))) ));

                return finalColor;
            }

            float3 light_color(VertexOutput i, float attenuation, float3 lightDirection,float3 normalDirection, float light_power, float shadow_size_attenuation_biggest,float3 viewDirection ): COLOR {
                float4 _Basetexture_var = tex2D(_LightBasetexture,TRANSFORM_TEX(i.uv0, _LightBasetexture));

                float3 finalColor = saturate(((lerp(_Basetexture_var.rgb,
                    _LightShadowColor.rgb,
                    (floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))*_Basetexture_var.a) > 0.5 ?
                        (1.0-(1.0-2.0*((lerp(_Basetexture_var.rgb,
                            _LightShadowColor.rgb,
                            (floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))
                            *_Basetexture_var.a)-0.5))*(1.0-lerp(_LightGlobalcolor.rgb,
                                _LightShadowColor.rgb,
                                (shadow_size_attenuation_biggest*(0.0*light_power*_LightThickness)))))
                        : (2.0*(lerp(_Basetexture_var.rgb,
                            _LightShadowColor.rgb,
                            (floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))*_Basetexture_var.a)*lerp(_LightGlobalcolor.rgb,
                                _LightShadowColor.rgb,
                                (shadow_size_attenuation_biggest*(0.0*light_power*_LightThickness))))));

                return finalColor;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);

                // get new value base on lerp between light and dark
                float t = 1 - i.lantern_lerp;
                float strength = lerp(_LightStrength, _DarkStrength, t);
                float thickness = lerp(_LightThickness, _DarkThickness, t);

                float shadowEffect = lerp(_LightShadowEffects, _DarkShadowEffects, t);
                float4 shadowColor = lerp(_LightShadowColor, _DarkShadowColor, t);
                float shadowSize = lerp(_LightShadowSize, _DarkShadowSize, t);

                float3 _DarkBaseTexture01_var = UnpackNormal(tex2D(_DarkBaseTexture01,TRANSFORM_TEX(i.uv0, _DarkBaseTexture01)));
                float3 _LightNormalMap_var = UnpackNormal(tex2D(_LightNormalMap,TRANSFORM_TEX(i.uv0, _LightNormalMap)));

                float3 normalLocal = lerp(
                    _DarkBaseTexture01_var.rgb,
                    _LightNormalMap,
                    t
                );

                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals

                float light_power = pow(dot(normalDirection,lightDirection),strength);

                float light_thickness = thickness*light_power;
                float shadow_size_attenuation_biggest = step(shadowSize,attenuation);


                float3 dark = dark_color(i, attenuation, lightDirection, normalDirection, viewDirection);
                float3 light = light_color(i, attenuation, lightDirection, normalDirection, viewDirection);

                float3 color_out = lerp(light, dark, 1-i.lantern_lerp);
                return fixed4(color_out,1);
            }


            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            Cull Off


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "noiseSimplex.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu
            #pragma target 3.0

            uniform float _LightStrength;
            uniform float _LightShadowSize;
            uniform float4 _LightShadowColor;
            uniform float4 _LightGlobalcolor;
            uniform float _LightShadowEffects;
            uniform float _LightThickness;
            uniform sampler2D _LightBasetexture; uniform float4 _LightBasetexture_ST;
            uniform sampler2D _LightNormalMap; uniform float4 _LightNormalMap_ST;

            uniform float _DarkSizeFull;
            uniform float _DarkStrength;
            uniform sampler2D _DarkBaseTexture01; uniform float4 _DarkBaseTexture01_ST;
            uniform float _DarkShadowSize;
            uniform float4 _DarkShadowColor;
            uniform float4 _DarkGlobalColor;
            uniform float _DarkShadowEffects;
            uniform float _DarkThickness;
            uniform sampler2D _DarkBaseTexture02; uniform float4 _DarkBaseTexture02_ST;
            uniform sampler2D _DarkAlphaTexture; uniform float4 _DarkAlphaTexture_ST;

            uniform half
                _Freq,
                _Speed,
                _Interpolation,
                _Strength,
                _Falloff,
                _Offset;

            uniform const int _numberOfCenters;
            uniform float4 _centers[5];
            uniform float _distances[5];

            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float lantern_lerp: TEXCOORD6;
                LIGHTING_COORDS(7,8)
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.screenPos = o.pos;

                float l = _distances[0] - length(_centers[0].xyz - o.posWorld.xyz);
                for(int index = 1; index < _numberOfCenters; ++index) {
                    float l_temp = _distances[index] - length(_centers[index].xyz - o.posWorld.xyz);
                    l = max(l, l_temp);
                }

                float3 wrldPos = o.posWorld.xyz * _Freq;
                wrldPos.y += _Time.x * _Speed;

                float ns = snoise(wrldPos);

                float lrp = saturate((l + ns * _Interpolation) * 1/_Falloff);

                o.lantern_lerp = lrp;

                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

            float3 dark_color(VertexOutput i, float attenuation, float3 lightDirection, float3x3 tangentTransform, float3 viewDirection): COLOR {
                i.screenPos.y *= _ProjectionParams.x;
                float3 _BaseTexture01_var = UnpackNormal(tex2D(_DarkBaseTexture01,TRANSFORM_TEX(i.uv0, _DarkBaseTexture01)));
                float3 normalLocal = _BaseTexture01_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                ////// Lighting:
                float4 _AlphaTexture_var = tex2D(_DarkAlphaTexture,TRANSFORM_TEX(i.uv0, _DarkAlphaTexture));
                float node_8652 = (abs(sin(((i.screenPos.rg+(1.0-max(0,dot(normalDirection, viewDirection))))*_DarkSizeFull))).r*_AlphaTexture_var.a);
                float node_5833 = 0.0;
                float light_power = pow(max(0,dot(normalDirection,lightDirection)),_Strength);
                float node_5452 = (_DarkThickness*light_power);
                float shadow_size_attenuation_biggest = step(_DarkShadowSize,attenuation);
                float4 _BaseTexture02_var = tex2D(_DarkBaseTexture02,TRANSFORM_TEX(i.uv0, _DarkBaseTexture02));
                float3 finalColor =
                saturate((
                    lerp(_BaseTexture02_var.rgb,
                        _DarkShadowColor.rgb,
                        (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1)*shadow_size_attenuation_biggest)) > 0.5 ?
                            (1.0-(1.0-2.0*(lerp(_BaseTexture02_var.rgb,
                                _DarkShadowColor.rgb,
                                (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1)
                                * shadow_size_attenuation_biggest))-0.5))
                                * (1.0-lerp(_DarkGlobalColor
                .rgb,
                                    _DarkShadowColor.rgb,
                                    (step(node_8652, smoothstep( node_5833,
                                        step(node_5452,node_8652),
                                        light_power ))
                                    * shadow_size_attenuation_biggest
                                    * step(node_8652,smoothstep( node_5833,
                                        step(node_5452,node_8652),
                                        light_power ))))))
                            : (2.0*lerp(_BaseTexture02_var.rgb,
                                _DarkShadowColor.rgb,
                                (floor(light_power * _DarkShadowEffects) / (_DarkShadowEffects - 1)
                                * shadow_size_attenuation_biggest))
                                * lerp(_DarkGlobalColor
                .rgb,
                                    _DarkShadowColor.rgb,
                                    (step(node_8652,smoothstep( node_5833,
                                        step(node_5452,node_8652),
                                        light_power ))
                                    * shadow_size_attenuation_biggest
                                    * step(node_8652, smoothstep( node_5833,
                                        step(node_5452,node_8652),
                                        light_power )))))));
                return finalColor;
            }

            float3 light_color(VertexOutput i, float facing: VFACE): COLOR {
               float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_LightNormalMap,TRANSFORM_TEX(i.uv0, _LightNormalMap)));
                float3 normalLocal = _NormalMap_var.rgb;
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                ////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float shadow_size_attenuation_biggest = step(_LightShadowSize,attenuation);
                float light_power = pow(max(0,dot(normalDirection,lightDirection)),_LightStrength);
                float4 _Basetexture_var = tex2D(_LightBasetexture,TRANSFORM_TEX(i.uv0, _LightBasetexture));
                float3 finalColor = saturate(( (lerp(_Basetexture_var.rgb,_LightShadowColor.rgb,(floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))*_Basetexture_var.a) > 0.5 ? (1.0-(1.0-2.0*((lerp(_Basetexture_var.rgb,_LightShadowColor.rgb,(floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))*_Basetexture_var.a)-0.5))*(1.0-lerp(_LightGlobalcolor.rgb,_LightShadowColor.rgb,(shadow_size_attenuation_biggest*(0.0*light_power*_LightThickness))))) : (2.0*(lerp(_Basetexture_var.rgb,_LightShadowColor.rgb,(floor(light_power * _LightShadowEffects) / (_LightShadowEffects - 1)*shadow_size_attenuation_biggest))*_Basetexture_var.a)*lerp(_LightGlobalcolor.rgb,_LightShadowColor.rgb,(shadow_size_attenuation_biggest*(0.0*light_power*_LightThickness)))) ));
                return fixed4(finalColor * 1,0);
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir *= faceSign;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);


                float3 dark = dark_color(i, facing);
                float3 light = light_color(i, facing);

                float3 color_out = lerp(light, dark, 1-i.lantern_lerp);
                return fixed4(color_out * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles n3ds wiiu
            #pragma target 3.0

            struct VertexInput {
                float4 vertex : POSITION;
            };

            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };

            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}