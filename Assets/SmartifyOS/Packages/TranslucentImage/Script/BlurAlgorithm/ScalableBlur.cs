using UnityEngine;
using UnityEngine.Rendering;

namespace SmartifyOS.UI.TranslucentImage
{
public class ScalableBlur : IBlurAlgorithm
{
    const int BLUR_PASS      = 0;
    const int CROP_BLUR_PASS = 1;

    readonly RenderTargetIdentifier[] scratches = new RenderTargetIdentifier[14];

    bool               isBirp;
    Material           material;
    ScalableBlurConfig config;

    Material Material
    {
        get
        {
            if (material == null)
                Material = new Material(Shader.Find(isBirp
                                                        ? "Hidden/EfficientBlur"
                                                        : "Hidden/EfficientBlur_UniversalRP"));

            return material;
        }
        set => material = value;
    }

    public void Init(BlurConfig config, bool isBirp)
    {
        this.isBirp = isBirp;
        this.config = (ScalableBlurConfig)config;
    }

    public void Blur(
        CommandBuffer          cmd,
        RenderTargetIdentifier src,
        Rect                   srcCropRegion,
        BackgroundFill         backgroundFill,
        RenderTexture          target
    )
    {
        float radius = ScaleWithResolution(config.Radius,
                                           target.width * srcCropRegion.width,
                                           target.height * srcCropRegion.height);
        ConfigMaterial(radius, srcCropRegion.ToMinMaxVector(), backgroundFill);

        int stepCount = Mathf.Clamp(config.Iteration * 2 - 1, 1, scratches.Length * 2 - 1);

        if(stepCount > 1)
            cmd.BlitCustom(src, scratches[0], Material, CROP_BLUR_PASS, isBirp);

        var depth    = Mathf.Min(config.Iteration - 1, scratches.Length - 1);
        for (var i = 1; i < stepCount; i++)
        {
            var fromIdx = SimplePingPong(i - 1, depth);
            var toIdx   = SimplePingPong(i,     depth);
            cmd.BlitCustom(scratches[fromIdx], scratches[toIdx], Material, 0, isBirp);
        }

        cmd.BlitCustom(stepCount > 1 ? scratches[0] : src,
                       target,
                       Material,
                       stepCount > 1 ? BLUR_PASS : CROP_BLUR_PASS,
                       isBirp);
    }

    public int GetScratchesCount()
    {
        return Mathf.Min(config.Iteration, scratches.Length);
    }

    public void GetScratchDescriptor(int index, ref RenderTextureDescriptor descriptor)
    {
        if (index == 0)
        {
            int firstDownsampleFactor = config.Iteration > 0 ? 1 : 0;
            descriptor.width  >>= firstDownsampleFactor;
            descriptor.height >>= firstDownsampleFactor;
        }
        else
        {
            descriptor.width  >>= 1;
            descriptor.height >>= 1;
        }
        if (descriptor.width <= 0) descriptor.width   = 1;
        if (descriptor.height <= 0) descriptor.height = 1;
    }

    public void SetScratch(int index, RenderTargetIdentifier value)
    {
        scratches[index] = value;
    }

    protected void ConfigMaterial(float radius, Vector4 cropRegion, BackgroundFill backgroundFill)
    {
        switch (backgroundFill.mode)
        {
        case BackgroundFillMode.None:
            Material.EnableKeyword("BACKGROUND_FILL_NONE");
            Material.DisableKeyword("BACKGROUND_FILL_COLOR");
            break;
        case BackgroundFillMode.Color:
            Material.EnableKeyword("BACKGROUND_FILL_COLOR");
            Material.DisableKeyword("BACKGROUND_FILL_NONE");
            Material.SetColor(ShaderId.BACKGROUND_COLOR, backgroundFill.color);
            break;
        }
        Material.SetFloat(ShaderId.RADIUS, radius);
        Material.SetVector(ShaderId.CROP_REGION, cropRegion);
    }

    ///<summary>
    /// Relative blur size to maintain same look across multiple resolution
    /// </summary>
    float ScaleWithResolution(float baseRadius, float width, float height)
    {
        float scaleFactor = Mathf.Min(width, height) / 1080f;
        scaleFactor = Mathf.Clamp(scaleFactor, .5f, 2f); //too much variation cause artifact
        return baseRadius * scaleFactor;
    }

    public static int SimplePingPong(int t, int max)
    {
        if (t > max)
            return 2 * max - t;
        return t;
    }
}
}
