using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SmartifyOS.UI.TranslucentImage.URP.Editor
{
[InitializeOnLoad]
public class RendererFeatureChecker : EditorWindow
{
    static RendererFeatureChecker()
    {
        EditorApplication.update += DoCheck;
    }

    static void DoCheck()
    {
        EditorApplication.update -= DoCheck;

        var pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (pipelineAsset == null)
            return;

        var rendererData = (ScriptableRendererData)
            typeof(UniversalRenderPipelineAsset)
               .GetProperty("scriptableRendererData", BindingFlags.Instance | BindingFlags.NonPublic)
              ?.GetValue(pipelineAsset);

        if (rendererData == null)
            return;

        var haveFeature = rendererData.rendererFeatures.OfType<TranslucentImageBlurSource>().Any();

        if (haveFeature)
            return;

        const float width  = 400f;
        const float height = 200f;
        var window = GetWindowWithRect<RendererFeatureChecker>(
            new Rect(
                (Screen.width - width) / 2f,
                (Screen.height - height) / 2f,
                width, height
            ),
            true,
            "Translucent Image",
            true
        );

        window.rendererData = rendererData;
    }

    ScriptableRendererData rendererData;

    void OnGUI()
    {
        GUILayout.Label("Missing Renderer Feature", EditorStyles.largeLabel);

        EditorGUILayout.Space();

        GUILayout.Label(
            "Translucent Image needs a renderer feature added to the active Renderer Asset. Do you want to add it now?",
            EditorStyles.wordWrappedLabel
        );

        if (GUILayout.Button("More info", EditorStyles.linkLabel))
            Application.OpenURL("https://leloctai.com/asset/translucentimage/docs/articles/universalrp.html");

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Select Current Renderer Asset"))
        {
            EditorGUIUtility.PingObject(rendererData);
            Selection.activeObject = rendererData;
            Close();
        }
    }
}
}
