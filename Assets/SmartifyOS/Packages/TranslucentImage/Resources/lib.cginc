#include <UnityCG.cginc>
#include "common.hlsl"

#define SAMPLE_SCREEN_TEX(tex, uv) UNITY_SAMPLE_SCREENSPACE_TEXTURE(tex, UnityStereoTransformScreenSpaceTex(uv))
