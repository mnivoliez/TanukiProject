#ifndef LANTERN_LERP_FUNC
#define LANTERN_LERP_FUNC

#include "noiseSimplex.cginc"

float lerp_lantern(float3 vertexPosition, int numberOfLanterns, float4 centers[5], float distances[5],
    half freq, half speed, half interpolation, half strength, half falloff) {
    float l = distances[0] - length(centers[0].xyz - vertexPosition);
    for(int index = 1; index < numberOfLanterns; ++index) {
        float l_temp = distances[index] - length(centers[index].xyz - vertexPosition);
        //float l_temp = length(centers[index].xyz - vertexPosition) / distances[index];
        l = max(l, l_temp);
    }
    float3 wrldPos = vertexPosition * freq;
    wrldPos.y += _Time.x * speed;

    float ns = snoise(wrldPos);

    return saturate((l + ns * interpolation) * 1/falloff);
}
#endif