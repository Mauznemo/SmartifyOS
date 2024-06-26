using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage
{
public static class ShaderId
{
    public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
    public static readonly int RADIUS   = Shader.PropertyToID("_Radius");
    public static readonly int COLOR    = Shader.PropertyToID("_Color");
    // public static readonly int ENV_TEX      = Shader.PropertyToID("_EnvTex");
    public static readonly int BACKGROUND_COLOR = Shader.PropertyToID("_BackgroundColor");
    public static readonly int CROP_REGION      = Shader.PropertyToID("_CropRegion");
}
}
