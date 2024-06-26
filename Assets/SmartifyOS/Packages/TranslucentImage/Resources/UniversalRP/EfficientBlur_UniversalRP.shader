Shader "Hidden/EfficientBlur_UniversalRP"
{
    Properties
    {
        _BackgroundColor ("_BackgroundColor", Color) = (0,0,0,0)
    }

    HLSLINCLUDE
    #pragma target 3.0
    //HLSLcc is not used by default on gles
    #pragma prefer_hlslcc gles
    //SRP don't support dx9
    #pragma exclude_renderers d3d11_9x

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "./lib.hlsl"

    TEXTURE2D_X(_MainTex);
    SAMPLER(sampler_MainTex);
    uniform half4 _MainTex_TexelSize;
    uniform half  _Radius;

    BlurVertexOutput vert(minimalVertexInput v)
    {
        BlurVertexOutput o;
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

        o.vertex = half4(pos.xy, 0.0, 1.0);
        o.texcoord = GetGatherTexcoord(uv, _MainTex_TexelSize, _Radius);

        return o;
    }
    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(BlurVertexOutput i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 o = GATHER(_MainTex, i.texcoord);
                return o;
            }
            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM
            //Crop before blur
            #pragma vertex vertCrop
            #pragma fragment frag
            #pragma multi_compile_local BACKGROUND_FILL_NONE BACKGROUND_FILL_COLOR

            float4 _CropRegion;
            half3 _BackgroundColor;

            BlurVertexOutput vertCrop(minimalVertexInput v)
            {
                BlurVertexOutput o = vert(v);

                o.texcoord.xy = getNewUV(o.texcoord.xy, _CropRegion);
                o.texcoord.zw = getNewUV(o.texcoord.zw, _CropRegion);

                return o;
            }

            half4 frag(BlurVertexOutput i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                half4 o = GATHER(_MainTex, i.texcoord);

                #if BACKGROUND_FILL_COLOR
                o.rgb = lerp(_BackgroundColor, o.rgb, o.a);
                o.a = 1.0h;
                #endif

                return o;
            }

            // v2f vert(appdata v)
            // {
            //     v2f o;
            //     UNITY_SETUP_INSTANCE_ID(v);
            //     UNITY_INITIALIZE_OUTPUT(v2f, o);
            //     UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            //     o.vertex = UnityObjectToClipPos(v.vertex);
            //     o.uv = v.uv;
            //
            //     o.viewDir = mul(unity_CameraInvProjection, o.vertex).xyz;
            //     #if UNITY_UV_STARTS_AT_TOP
            //     o.viewDir.y = -o.viewDir.y;
            //     #endif
            //     o.viewDir.z = -o.viewDir.z;
            //     o.viewDir = mul(unity_CameraToWorld, o.viewDir.xyzz).xyz;
            //     return o;
            // }
            //
            // samplerCUBE _EnvTex;
            // float4      _EnvTex_HDR;
            //
            // half4 frag(v2f i) : SV_Target
            // {
            //     UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
            //     half4 col = SAMPLE_SCREEN_TEX(_MainTex, i.uv);
            //     half4 envData = texCUBE(_EnvTex, normalize(i.viewDir));
            //     half3 env = DecodeHDR(envData, _EnvTex_HDR);
            //     col.rgb *= col.a;
            //     col.rgb = col.rgb + env * (1 - col.a);
            //     col.a = 1;
            //     return col;
            // }
            ENDHLSL
        }
    }

    FallBack Off
}
