#define GATHER(tex, locations)  \
    o =  SAMPLE_SCREEN_TEX(tex, locations.xw) * 1.0h / 4.0h; \
    o += SAMPLE_SCREEN_TEX(tex, locations.zw) * 1.0h / 4.0h; \
    o += SAMPLE_SCREEN_TEX(tex, locations.xy) * 1.0h / 4.0h; \
    o += SAMPLE_SCREEN_TEX(tex, locations.zy) * 1.0h / 4.0h; \


#if defined(UNITY_SINGLE_PASS_STEREO)
float4 StereoAdjustedTexelSize(float4 texelSize)
{
    texelSize.x = texelSize.x * 2.0; // texelSize.x = 1/w. For a double-wide texture, the true resolution is given by 2/w.
    texelSize.z = texelSize.z * 0.5; // texelSize.z = w. For a double-wide texture, the true size of the eye texture is given by w/2.
    return texelSize;
}
#else
float4 StereoAdjustedTexelSize(float4 texelSize)
{
    return texelSize;
}
#endif

float4 GetGatherTexcoord(float2 texcoord, float4 texelSize, half radius)
{
    half4 offset = half2(-0.5h, 0.5h).xxyy; //-x, -y, x, y
    offset *= StereoAdjustedTexelSize(texelSize).xyxy;
    offset *= radius;
    return texcoord.xyxy + offset;
}

float2 getNewUV(float2 oldUV, float4 cropRegion)
{
    return lerp(cropRegion.xy, cropRegion.zw, oldUV);
}

float2 getCroppedCoord(float2 screenCoord, float4 cropRegion)
{
    return (screenCoord - cropRegion.xy) / (cropRegion.zw - cropRegion.xy);
}

struct BlurVertexOutput
{
    float4 vertex : SV_POSITION;
    float4 texcoord : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
