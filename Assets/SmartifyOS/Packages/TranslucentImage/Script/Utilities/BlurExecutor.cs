using UnityEngine;
using UnityEngine.Rendering;

namespace SmartifyOS.UI.TranslucentImage
{
public static class BlurExecutor
{
    static readonly int[] TEMP_RT = new int[14];

    static BlurExecutor()
    {
        for (var i = 0; i < TEMP_RT.Length; i++)
        {
            TEMP_RT[i] = Shader.PropertyToID($"TI_intermediate_rt_{i}");
        }
    }

    public readonly struct BlurExecutionData
    {
        public readonly RenderTargetIdentifier sourceTex;
        public readonly TranslucentImageSource blurSource;
        public readonly IBlurAlgorithm         blurAlgorithm;

        public BlurExecutionData(
            RenderTargetIdentifier sourceTex,
            TranslucentImageSource blurSource,
            IBlurAlgorithm         blurAlgorithm
        )
        {
            this.sourceTex     = sourceTex;
            this.blurSource    = blurSource;
            this.blurAlgorithm = blurAlgorithm;
        }
    }

    public static void ExecuteBlurWithTempTextures(CommandBuffer cmd, ref BlurExecutionData data)
    {
        var scratchesCount = data.blurAlgorithm.GetScratchesCount();

        var desc = data.blurSource.BlurredScreen.descriptor;
        desc.msaaSamples     = 1;
        desc.useMipMap       = false;
        desc.depthBufferBits = 0;

        for (int i = 0; i < scratchesCount; i++)
        {
            data.blurAlgorithm.GetScratchDescriptor(i, ref desc);
            cmd.GetTemporaryRT(TEMP_RT[i], desc, FilterMode.Bilinear);
            data.blurAlgorithm.SetScratch(i, TEMP_RT[i]);
        }

        {
            ExecuteBlur(cmd, ref data);
        }

        for (int i = 0; i < scratchesCount; i++)
            cmd.ReleaseTemporaryRT(TEMP_RT[i]);
    }

    public static void ExecuteBlur(CommandBuffer cmd, ref BlurExecutionData data)
    {
        var blurSource    = data.blurSource;
        var blurredScreen = blurSource.BlurredScreen;
        var blurRegion    = blurSource.BlurRegion;

        data.blurAlgorithm.Blur(cmd,
                                data.sourceTex,
                                blurRegion,
                                blurSource.BackgroundFill,
                                blurredScreen);
    }
}
}
