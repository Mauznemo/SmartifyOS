#if UNITY_2020_3_OR_NEWER
#define HAS_CONFIGINPUT
#endif

#if UNITY_2022_3_OR_NEWER
#define HAS_DOUBLEBUFFER_BOTH
#endif

#if UNITY_2023_3_OR_NEWER
#define HAS_RENDERGRAPH
#endif

#if URP12_OR_NEWER
#define HAS_DOUBLEBUFFER_UNIVERSAL_RENDERER
#define HAS_RENDERORDER
#else
#define STATIC_AFTERPOSTTEX
#endif


using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Scripting.APIUpdating;

namespace SmartifyOS.UI.TranslucentImage.URP
{
enum RendererType
{
    Universal,
    Renderer2D
}

[MovedFrom("LeTai.Asset.TranslucentImage.LWRP")]
public partial class TranslucentImageBlurRenderPass : ScriptableRenderPass
{
    internal struct PassData
    {
        public TranslucentImageSource blurSource;
        public IBlurAlgorithm         blurAlgorithm;
        public Vector2Int             camPixelSize;
        public bool                   shouldUpdateBlur;
        public bool                   isPreviewing;
    }

    internal struct SRPassData
    {
#if !HAS_DOUBLEBUFFER_BOTH
        public RendererType           rendererType;
        public RenderTargetIdentifier cameraColorTarget;
#if HAS_RENDERORDER
        public TranslucentImageBlurSource.RenderOrder renderOrder;
#endif
#endif
        public bool canvasDisappearWorkaround;
    }

    public readonly struct PreviewExecutionData
    {
        public readonly TranslucentImageSource blurSource;
        public readonly RenderTargetIdentifier previewTarget;
        public readonly Material               previewMaterial;

        public PreviewExecutionData(
            TranslucentImageSource blurSource,
            RenderTargetIdentifier previewTarget,
            Material               previewMaterial
        )
        {
            this.blurSource      = blurSource;
            this.previewTarget   = previewTarget;
            this.previewMaterial = previewMaterial;
        }
    }

    private const string PROFILER_TAG = "Translucent Image Source";

#if STATIC_AFTERPOSTTEX
    readonly RenderTargetIdentifier afterPostprocessTexture;
#endif

#if HAS_DOUBLEBUFFER_UNIVERSAL_RENDERER
    readonly URPRendererInternal urpRendererInternal;
#endif

    PassData   currentPassData;
    SRPassData currentSRPassData;
    Material   previewMaterial;

    public Material PreviewMaterial
    {
        get
        {
            if (!previewMaterial)
                previewMaterial = CoreUtils.CreateEngineMaterial("Hidden/FillCrop_UniversalRP");

            return previewMaterial;
        }
    }

    internal TranslucentImageBlurRenderPass(



#if HAS_DOUBLEBUFFER_UNIVERSAL_RENDERER
        URPRendererInternal urpRendererInternal
#endif
    )
    {
#if STATIC_AFTERPOSTTEX
        afterPostprocessTexture = new RenderTargetIdentifier(Shader.PropertyToID("_AfterPostProcessTexture"), 0, CubemapFace.Unknown, -1);
#endif

#if HAS_DOUBLEBUFFER_UNIVERSAL_RENDERER
        this.urpRendererInternal = urpRendererInternal;
#endif

        RenderGraphInit();
    }

#if !HAS_DOUBLEBUFFER_BOTH
    RenderTargetIdentifier GetAfterPostColor()
    {
#if STATIC_AFTERPOSTTEX
        return afterPostprocessTexture;
#else
        return urpRendererInternal.GetAfterPostColor();
#endif
    }
#endif

    ~TranslucentImageBlurRenderPass()
    {
        CoreUtils.Destroy(previewMaterial);
        RenderGraphDispose();
    }

    internal void SetupSRP(SRPassData srPassData)
    {
        currentSRPassData = srPassData;
    }


    internal void Setup(PassData passData)
    {
        currentPassData = passData;
#if HAS_CONFIGINPUT
        ConfigureInput(ScriptableRenderPassInput.Color);
#endif
    }

#if HAS_RENDERGRAPH
    [Obsolete("This rendering path is for compatibility mode only (when Render Graph is disabled). Use Render Graph API instead.", false)]
#endif
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        var                    cmd = CommandBufferPool.Get(PROFILER_TAG);
        RenderTargetIdentifier sourceTex;

#if !HAS_DOUBLEBUFFER_BOTH
        var isPostProcessEnabled = renderingData.cameraData.postProcessEnabled;
        void SetSource2DRenderer()
        {
            bool useAfterPostTex = isPostProcessEnabled;
#if HAS_RENDERORDER
            useAfterPostTex &= currentSRPassData.renderOrder == TranslucentImageBlurSource.RenderOrder.AfterPostProcessing;
#endif
            sourceTex = useAfterPostTex
                            ? GetAfterPostColor()
                            : currentSRPassData.cameraColorTarget;
        }
#endif

#if HAS_DOUBLEBUFFER_BOTH
        sourceTex = urpRendererInternal.GetBackBuffer();
#elif HAS_DOUBLEBUFFER_UNIVERSAL_RENDERER
        if (currentSRPassData.rendererType == RendererType.Universal)
        {
            sourceTex = urpRendererInternal.GetBackBuffer();
        }
        else
        {
            SetSource2DRenderer();
        }
#else
        SetSource2DRenderer();
#endif

        var  blurSource        = currentPassData.blurSource;
        bool shouldResetTarget = currentSRPassData.canvasDisappearWorkaround && renderingData.cameraData.resolveFinalTarget;

        if (currentPassData.shouldUpdateBlur)
        {
            blurSource.OnBeforeBlur(currentPassData.camPixelSize);
            var blurExecData = new BlurExecutor.BlurExecutionData(sourceTex,
                                                                  blurSource,
                                                                  currentPassData.blurAlgorithm);
            BlurExecutor.ExecuteBlurWithTempTextures(cmd, ref blurExecData);

            if (shouldResetTarget)
                CoreUtils.SetRenderTarget(cmd, BuiltinRenderTextureType.CameraTarget);
        }


        if (currentPassData.isPreviewing)
        {
            var previewTarget = shouldResetTarget ? BuiltinRenderTextureType.CameraTarget : sourceTex;
            var previewExecData = new PreviewExecutionData(blurSource,
                                                           previewTarget,
                                                           PreviewMaterial);
            ExecutePreview(cmd, ref previewExecData);
        }


        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public static void ExecutePreview(CommandBuffer cmd, ref PreviewExecutionData data)
    {
        var blurSource = data.blurSource;

        data.previewMaterial.SetVector(ShaderId.CROP_REGION, blurSource.BlurRegion.ToMinMaxVector());
        cmd.BlitCustom(blurSource.BlurredScreen, data.previewTarget, data.previewMaterial, 0);
    }
}
}
