using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace SmartifyOS.UI.TranslucentImage.Editor
{
class ScenceGizmoAutoDisable : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (!importedAssets.Any(p => p.Contains("TranslucentImage")))
            return;

        var structAnnotation = Type.GetType("UnityEditor.Annotation, UnityEditor");
        if (structAnnotation == null) return;

        var fieldClassId     = structAnnotation.GetField("classID");
        var fieldScriptClass = structAnnotation.GetField("scriptClass");
        var fieldFlags       = structAnnotation.GetField("flags");
        var fieldIconEnabled = structAnnotation.GetField("iconEnabled");

        Type classAnnotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
        if (classAnnotationUtility == null) return;

        var methodGetAnnotations = classAnnotationUtility.GetMethod("GetAnnotations", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodGetAnnotations == null) return;
        var methodSetIconEnabled = classAnnotationUtility.GetMethod("SetIconEnabled", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodSetIconEnabled == null) return;

        Array annotations = (Array)methodGetAnnotations.Invoke(null, null);
        foreach (var a in annotations)
        {
            string scriptClass = (string)fieldScriptClass.GetValue(a);

            // built in types
            if (string.IsNullOrEmpty(scriptClass)) continue;

            int classId     = (int)fieldClassId.GetValue(a);
            int flags       = (int)fieldFlags.GetValue(a);
            int iconEnabled = (int)fieldIconEnabled.GetValue(a);

            const int maskHasIcon = 1;
            bool      hasIconFlag = (flags & maskHasIcon) == maskHasIcon;

            if (hasIconFlag
             && iconEnabled != 0
             && scriptClass.Contains("TranslucentImage"))
            {
                methodSetIconEnabled.Invoke(null, new object[] { classId, scriptClass, 0 });
            }
        }
    }
}
}
