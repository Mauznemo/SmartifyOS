using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage.URP.Editor
{
[CustomEditor(typeof(TranslucentImageBlurSource))]
public class TranslucentImageBlurSourceEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var feature = (TranslucentImageBlurSource)target;
        if (feature.rendererType == RendererType.Renderer2D)
        {
            var ver = Version.Parse(Regex.Replace(Application.unityVersion, @"[^\d.]", "."));
            if (
                ver >= new Version(2023, 1, 7) ||
                (ver.Major == 2022 && ver >= new Version(2022, 3, 7))
            )
                EditorGUILayout.HelpBox("The 2D Renderer is sometimes buggy with custom renderer features at the moment.\n\n" +
                                        "If you're encountering problems, check if the built-in \"Full Screen Pass Renderer Feature\" is functioning correctly in the same setup. If it does not, Translucent Image is unlikely to be able to work either.\n\n" +
                                        "Also, make sure you're on the latest Unity patch release for your minor version", MessageType.Warning, true);
            else
                EditorGUILayout.HelpBox("This version of the 2D Renderer includes a bug that prevents Translucent Image from working correctly in many cases. (UUM-14400)", MessageType.Error, true);
        }
    }
}
}
