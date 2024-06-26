using System;
using UnityEngine;
using UnityEngine.Rendering;
#if ENABLE_VR
using UnityEngine.XR;
#endif

namespace SmartifyOS.UI.TranslucentImage
{
/// <summary>
/// Common source of blur for Translucent Images.
/// </summary>
/// <remarks>
/// It is an Image effect that blur the render target of the Camera it attached to, then save the result to a global read-only  Render Texture
/// </remarks>
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Tai Le Assets/Translucent Image Source")]
[HelpURL("https://leloctai.com/asset/translucentimage/docs/articles/customize.html#translucent-image-source")]
public partial class TranslucentImageSource : MonoBehaviour
{
#region Private Field
    [SerializeField]
    BlurConfig blurConfig;

    [SerializeField] [Range(0, 3)]
    [Tooltip("Reduce the size of the screen before processing. Increase will improve performance but create more artifact")]
    int downsample;

    [SerializeField]
    [Tooltip("Choose which part of the screen to blur. Smaller region is faster")]
    Rect blurRegion = new Rect(0, 0, 1, 1);

    [SerializeField]
    [Tooltip("How many time to blur per second. Reduce to increase performance and save battery for slow moving background")]
    float maxUpdateRate = float.PositiveInfinity;

    [SerializeField]
    [Tooltip("Preview the effect fullscreen. Not recommended for runtime use")]
    bool preview;

    [SerializeField]
    [Tooltip("Fill the background where the frame buffer alpha is 0. Useful for VR Underlay and Passthrough, where these areas would otherwise be black")]
    BackgroundFill backgroundFill = new BackgroundFill();

    int        lastDownsample;
    Rect       lastBlurRegion   = new Rect(0, 0, 1, 1);
    Vector2Int lastCamPixelSize = Vector2Int.zero;
    float      lastUpdate;

    IBlurAlgorithm blurAlgorithm;

#pragma warning disable 0108
    Camera camera;
#pragma warning restore 0108
    Material      previewMaterial;
    RenderTexture blurredScreen;
    CommandBuffer cmd;
    bool          isRequested;
#pragma warning disable CS0169
    bool isForOverlayCanvas;
#pragma warning restore CS0169
#endregion


#region Properties
    public BlurConfig BlurConfig
    {
        get { return blurConfig; }
        set
        {
            blurConfig = value;
            InitializeBlurAlgorithm();
        }
    }

    /// <summary>
    /// The rendered image will be shrinked by a factor of 2^{{Downsample}} before bluring to reduce processing time
    /// </summary>
    /// <value>
    /// Must be non-negative. Default to 0
    /// </value>
    public int Downsample
    {
        get { return downsample; }
        set { downsample = Mathf.Max(0, value); }
    }

    /// <summary>
    /// Define the rectangular area on screen that will be blurred.
    /// </summary>
    /// <value>
    /// Between 0 and 1
    /// </value>
    public Rect BlurRegion
    {
        get { return blurRegion; }
        set
        {
            Vector2 min = new Vector2(1 / (float)Cam.pixelWidth, 1 / (float)Cam.pixelHeight);
            blurRegion.x      = Mathf.Clamp(value.x,      0,     1 - min.x);
            blurRegion.y      = Mathf.Clamp(value.y,      0,     1 - min.y);
            blurRegion.width  = Mathf.Clamp(value.width,  min.x, 1 - blurRegion.x);
            blurRegion.height = Mathf.Clamp(value.height, min.y, 1 - blurRegion.y);
        }
    }

    /// <summary>
    /// Maximum number of times to update the blurred image each second
    /// </summary>
    public float MaxUpdateRate
    {
        get => maxUpdateRate;
        set => maxUpdateRate = value;
    }

    /// <summary>
    /// Fill the background where the frame buffer alpha is 0. Useful for VR Underlay and Passthrough, where these areas would otherwise be black
    /// </summary>
    public BackgroundFill BackgroundFill
    {
        get => backgroundFill;
        set => backgroundFill = value;
    }

    /// <summary>
    /// Render the blurred result to the render target
    /// </summary>
    public bool Preview
    {
        get => preview;
        set => preview = value;
    }

    /// <summary>
    /// Result of the image effect. Translucent Image use this as their content (read-only)
    /// </summary>
    public RenderTexture BlurredScreen
    {
        get { return blurredScreen; }
        set { blurredScreen = value; }
    }

    /// <summary>
    /// Set in SRP to provide Cam.rect for overlay cameras
    /// </summary>
    public Rect CamRectOverride { get; set; } = Rect.zero;

    /// <summary>
    /// Blur Region rect is relative to Cam.rect . This is relative to the full screen
    /// </summary>
    public Rect BlurRegionNormalizedScreenSpace
    {
        get
        {
            var camRect = CamRectOverride.width == 0 ? Cam.rect : CamRectOverride;
            camRect.min = Vector2.Max(Vector2.zero, camRect.min);
            camRect.max = Vector2.Min(Vector2.one, camRect.max);

            return new Rect(camRect.position + BlurRegion.position * camRect.size,
                            camRect.size * BlurRegion.size);
        }

        set
        {
            var camRect = CamRectOverride.width == 0 ? Cam.rect : CamRectOverride;
            camRect.min = Vector2.Max(Vector2.zero, camRect.min);
            camRect.max = Vector2.Min(Vector2.one, camRect.max);

            BlurRegion = new Rect((value.position - camRect.position) / camRect.size,
                                  value.size / camRect.size);
        }
    }

    /// <summary>
    /// The Camera attached to the same GameObject. Cached in field 'camera'
    /// </summary>
    internal Camera Cam
    {
        get { return camera ? camera : camera = GetComponent<Camera>(); }
    }

    /// <summary>
    /// Minimum time in second to wait before refresh the blurred image.
    /// If maxUpdateRate non-positive then just stop updating
    /// </summary>
    float MinUpdateCycle
    {
        get { return (MaxUpdateRate > 0) ? (1f / MaxUpdateRate) : float.PositiveInfinity; }
    }
#endregion


#if UNITY_EDITOR

    protected virtual void OnEnable()
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Start();
        }
    }

    protected virtual void OnGUI()
    {
        if (!Preview) return;
        if (UnityEditor.Selection.activeGameObject != gameObject) return;

        var curBlurRegionNSS = BlurRegionNormalizedScreenSpace;
        var newBlurRegionNSS = ResizableScreenRect.Draw(curBlurRegionNSS);

        if (newBlurRegionNSS != curBlurRegionNSS)
        {
            UnityEditor.Undo.RecordObject(this, "Change Blur Region");
            BlurRegionNormalizedScreenSpace = newBlurRegionNSS;
        }

        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
    }
#endif

    protected virtual void Start()
    {
        previewMaterial = new Material(Shader.Find("Hidden/FillCrop"));

        InitializeBlurAlgorithm();
        CreateNewBlurredScreen(Vector2Int.RoundToInt(Cam.pixelRect.size));

        lastDownsample = Downsample;
    }

    void OnDestroy()
    {
        if (BlurredScreen)
            BlurredScreen.Release();
    }

    void InitializeBlurAlgorithm()
    {
        switch (blurConfig)
        {
        case ScalableBlurConfig _:
            blurAlgorithm = new ScalableBlur();
            break;
        default:
            blurAlgorithm = new ScalableBlur();
            break;
        }
    }

    protected virtual void CreateNewBlurredScreen(Vector2Int camPixelSize)
    {
        if (BlurredScreen)
            BlurredScreen.Release();

#if ENABLE_VR
        if (XRSettings.enabled)
        {
            BlurredScreen        = new RenderTexture(XRSettings.eyeTextureDesc);
            BlurredScreen.width  = Mathf.RoundToInt(BlurredScreen.width * BlurRegion.width) >> Downsample;
            BlurredScreen.height = Mathf.RoundToInt(BlurredScreen.height * BlurRegion.height) >> Downsample;
            BlurredScreen.depth  = 0;
        }
        else
#endif
        {
            BlurredScreen = new RenderTexture(Mathf.RoundToInt(camPixelSize.x * BlurRegion.width) >> Downsample,
                                              Mathf.RoundToInt(camPixelSize.y * BlurRegion.height) >> Downsample, 0);
        }

        BlurredScreen.antiAliasing = 1;
        BlurredScreen.useMipMap    = false;

        BlurredScreen.name       = $"{gameObject.name} Translucent Image Source";
        BlurredScreen.filterMode = FilterMode.Bilinear;

        BlurredScreen.Create();
    }

    TextureDimension lastEyeTexDim;

    public void OnBeforeBlur(Vector2Int camPixelSize)
    {
        if (
            BlurredScreen == null
         || !BlurredScreen.IsCreated()
         || Downsample != lastDownsample
         || !BlurRegion.Approximately(lastBlurRegion)
         || camPixelSize != lastCamPixelSize
#if ENABLE_VR
         || XRSettings.deviceEyeTextureDimension != lastEyeTexDim
#endif
        )
        {
            CreateNewBlurredScreen(camPixelSize);
            lastDownsample   = Downsample;
            lastBlurRegion   = BlurRegion;
            lastCamPixelSize = camPixelSize;
#if ENABLE_VR
            lastEyeTexDim = XRSettings.deviceEyeTextureDimension;
#endif
        }

        lastUpdate = GetTrueCurrentTime();
    }


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (cmd == null)
        {
            cmd      = new CommandBuffer();
            cmd.name = "Translucent Image Source";
        }

        if (blurAlgorithm != null && BlurConfig != null)
        {
            if (ShouldUpdateBlur())
            {
                cmd.Clear();

                OnBeforeBlur(Vector2Int.RoundToInt(Cam.pixelRect.size));
                blurAlgorithm.Init(BlurConfig, true);
                var blurExecData = new BlurExecutor.BlurExecutionData(source,
                                                                      this,
                                                                      blurAlgorithm);
                BlurExecutor.ExecuteBlurWithTempTextures(cmd, ref blurExecData);

                Graphics.ExecuteCommandBuffer(cmd);
            }

            if (Preview)
            {
                previewMaterial.SetVector(ShaderId.CROP_REGION, BlurRegion.ToMinMaxVector());
                Graphics.Blit(BlurredScreen, destination, previewMaterial);
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }
        else
        {
            Graphics.Blit(source, destination);
        }


        isRequested = false;
    }

    public void Request()
    {
        isRequested = true;
    }

    public bool ShouldUpdateBlur()
    {
        if (!enabled)
            return false;

        if (!Preview && !isRequested)
            return false;

        float now    = GetTrueCurrentTime();
        bool  should = now - lastUpdate >= MinUpdateCycle;

        return should;
    }

    private static float GetTrueCurrentTime()
    {
#if UNITY_EDITOR
        return (float)UnityEditor.EditorApplication.timeSinceStartup;
#else
        return Time.unscaledTime;
#endif
    }
}
}
