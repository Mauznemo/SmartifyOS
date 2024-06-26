#if UNITY_2019
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage.Editor
{
public class Unity2019ShaderCompilerBugWorkaround : ScriptableObject
{
    [MenuItem("Tools/Translucent Image/Fix Shader Compile Errors")]
    static void ReImport()
    {
        var guids = AssetDatabase.FindAssets("l:TranslucentImageEditorResources lib");
        var path  = AssetDatabase.GUIDToAssetPath(guids[0]);
        var text  = File.ReadAllText(path);
        File.WriteAllText(path, text + "//DELETE ME: Workaround shader not compiling in Unity 2019");
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        File.WriteAllText(path, text);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
}
#endif
