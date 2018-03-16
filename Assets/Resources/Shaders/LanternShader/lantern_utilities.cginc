#ifndef LANTERN_LERP_FUNC
#define LANTERN_LERP_FUNC

// should return scale between 1 and 0, 1 if close to lantern, 0 otherwise
float lanterns_intensity(float3 vertexPosition, int numberOfLanterns, float4 centers[5], float distances[5]) {
    // it's the intensity of the lanterns' effect on this vertex pixel, cummulative
    float intensity = 0.0;
    for(int i = 0; i < numberOfLanterns; ++i) {
        float distance_point_lantern = distance(centers[i].xyz, vertexPosition);
        float lantern_intensity = 1 - distance_point_lantern / distances[i];
        lantern_intensity = clamp(lantern_intensity, 0, 1);
        intensity += lantern_intensity;
    }

    // the intensity is between 0 and 1, so clamp
    intensity = clamp(intensity, 0, 1);
    return intensity;
}
#endif