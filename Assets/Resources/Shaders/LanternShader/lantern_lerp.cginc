#ifndef LANTERN_LERP_FUNC
#define LANTERN_LERP_FUNC

#include "noiseSimplex.cginc"

// should return scale between 1 and 0, 1 if close to lantern, 0 otherwise
float lerp_lantern(float3 vertexPosition, int numberOfLanterns, float4 centers[5], float distances[5]) {
    float l;
    bool is_set = false;
    //float l = distances[0] - length(centers[0].xyz - vertexPosition);
    //float l = abs(length(centers[0].xyz - vertexPosition)) / distances[0];
    int index = 0;
    while(index < numberOfLanterns) {
        float l_temp = length(centers[index].xyz - vertexPosition) / distances[index];
        if (is_set) {
            l = min(l, l_temp);
        } else {
            l = l_temp;
            is_set = true;
        }
        ++index;
    }

    /*l = min(l, 1);
    l = max(l, 0);

    float result;
    if (is_set) {
        result = 1 - l;
    } else {
        result = 0;
    }
    return result;*/
    return l;
}
#endif