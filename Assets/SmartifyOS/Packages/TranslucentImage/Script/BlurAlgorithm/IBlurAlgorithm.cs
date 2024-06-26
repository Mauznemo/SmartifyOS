using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SmartifyOS.UI.TranslucentImage
{
public enum BlurAlgorithmType
{
    ScalableBlur
}

public enum BackgroundFillMode
{
    None,
    Color
}

[Serializable]
public class BackgroundFill
{
    public BackgroundFillMode mode  = BackgroundFillMode.None;
    public Color              color = Color.white;
}

public interface IBlurAlgorithm
{
    void Init(BlurConfig config, bool isBirp);

    void Blur(
        CommandBuffer          cmd,
        RenderTargetIdentifier src,
        Rect                   srcCropRegion,
        BackgroundFill         backgroundFill,
        RenderTexture          target
    );

    int  GetScratchesCount();
    void GetScratchDescriptor(int index, ref RenderTextureDescriptor descriptor);
    void SetScratch(int           index, RenderTargetIdentifier      value);
}
}
