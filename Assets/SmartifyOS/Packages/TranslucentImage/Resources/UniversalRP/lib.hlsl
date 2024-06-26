#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "../common.hlsl"

#define USE_PROCEDURAL_QUAD (SHADER_TARGET >= 30 && !SHADER_API_GLES)

struct minimalVertexInput
{
#if USE_PROCEDURAL_QUAD
    uint vertexID  : SV_VertexID;
#else
    float4 position : POSITION;
#endif
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct minimalVertexOutput
{
    float4 position : POSITION;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};

void GetProceduralQuad(in uint vertexID, out float4 positionCS, out float2 uv)
{
    positionCS = GetQuadVertexPosition(vertexID);
    positionCS.xy = positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f);
    uv = GetQuadTexCoord(vertexID); // * _ScaleBias.xy + _ScaleBias.zw;
}

float2 VertexToUV(float2 vertex)
{
    float2 texcoord = (vertex + 1.0) * 0.5; // triangle vert to uv
#if UNITY_UV_STARTS_AT_TOP
    texcoord = texcoord * float2(1.0, -1.0) + float2(0.0, 1.0);
#endif
    return texcoord;
}

#if UNITY_VERSION < 600000
SAMPLER(sampler_LinearClamp);
#endif
#define SAMPLE_SCREEN_TEX(tex, uv) SAMPLE_TEXTURE2D_X(tex, sampler_LinearClamp, UnityStereoTransformScreenSpaceTex(uv))
