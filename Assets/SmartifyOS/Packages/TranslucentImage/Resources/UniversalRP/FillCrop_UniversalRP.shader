Shader "Hidden/FillCrop_UniversalRP"
{
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma target 3.0
            //HLSLcc is not used by default on gles
            #pragma prefer_hlslcc gles
            //SRP don't support dx9
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "./lib.hlsl"

            minimalVertexOutput vert(minimalVertexInput v)
            {
                minimalVertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                float4 pos;
                float2 uv;

#if USE_PROCEDURAL_QUAD
                GetProceduralQuad(v.vertexID, pos, uv);
#else
                pos = v.position;
                uv = VertexToUV(v.position.xy);
#endif

                o.position = half4(pos.xy, 0.0, 1.0);
                o.texcoord = uv;
                return o;
            }

            TEXTURE2D_X(_MainTex);
            float4 _CropRegion;

            half4 frag(minimalVertexOutput v) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(v)
                return SAMPLE_SCREEN_TEX(_MainTex, getCroppedCoord(v.texcoord, _CropRegion));
            }
            ENDHLSL
        }
    }
}
