Shader "Hidden/EfficientBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BackgroundColor ("_BackgroundColor", Color) = (0,0,0,0)
    }

    CGINCLUDE
    #include "UnityCG.cginc"
    #include "lib.cginc"

    UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
    float4 _MainTex_TexelSize;

    uniform half _Radius;

    BlurVertexOutput vert(appdata_img v)
    {
        BlurVertexOutput o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(BlurVertexOutput, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

        o.vertex = UnityObjectToClipPos(v.vertex);
        o.texcoord = GetGatherTexcoord(v.texcoord, _MainTex_TexelSize, _Radius);

        return o;
    }
    ENDCG

    SubShader
    {
        Cull Off ZWrite Off ZTest Always Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(BlurVertexOutput i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i)
                half4 o = GATHER(_MainTex, i.texcoord);
                return o;
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            //Crop before blur
            #pragma vertex vertCrop
            #pragma fragment frag
            #pragma multi_compile_local BACKGROUND_FILL_NONE BACKGROUND_FILL_COLOR

            float4 _CropRegion;
            half3  _BackgroundColor;

            BlurVertexOutput vertCrop(appdata_img v)
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
            ENDCG
        }
    }

    FallBack Off
}
