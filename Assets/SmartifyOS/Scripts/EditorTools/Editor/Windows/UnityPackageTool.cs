using UnityEditor;
using UnityEngine;
using System.IO;

public class UnityPackageTool : EditorWindow
{
    private string createPackagePath = "Assets";
    private string unpackPackagePath = "Assets";
    private string unityPackagePath = "";

    [MenuItem("Window/SOS Dev Tools/Unity Package Tool")]
    public static void ShowWindow()
    {
        GetWindow<UnityPackageTool>("Unity Package Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create .unitypackage", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Source Path:", GUILayout.Width(100));
        createPackagePath = EditorGUILayout.TextField(createPackagePath);
        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    createPackagePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please select a folder inside the Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Create Unity Package"))
        {
            CreateUnityPackage(createPackagePath);
        }

        GUILayout.Space(20);

        GUILayout.Label("Unpack .unitypackage", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Package Path:", GUILayout.Width(100));
        unityPackagePath = EditorGUILayout.TextField(unityPackagePath);
        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            unityPackagePath = EditorUtility.OpenFilePanel("Select Unity Package", "", "unitypackage");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Destination Path:", GUILayout.Width(100));
        unpackPackagePath = EditorGUILayout.TextField(unpackPackagePath);
        if (GUILayout.Button("Select", GUILayout.Width(70)))
        {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Folder", "Assets", "");
            if (!string.IsNullOrEmpty(selectedPath))
            {
                if (selectedPath.StartsWith(Application.dataPath))
                {
                    unpackPackagePath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
                else
                {
                    EditorUtility.DisplayDialog("Invalid Path", "Please select a folder inside the Assets directory.", "OK");
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Unpack Unity Package"))
        {
            UnpackUnityPackage(unityPackagePath, unpackPackagePath);
        }
    }

    private void CreateUnityPackage(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", "Invalid source path.", "OK");
            return;
        }

        string packageName = Path.GetFileName(path.TrimEnd('/')) + ".unitypackage";
        string savePath = EditorUtility.SaveFilePanel("Save Unity Package", "", packageName, "unitypackage");

        if (!string.IsNullOrEmpty(packageName))
        {
            AssetDatabase.ExportPackage(path, packageName, ExportPackageOptions.Recurse);
            EditorUtility.DisplayDialog("Success", "Unity package created successfully!", "OK");
        }
    }

    private void UnpackUnityPackage(string packagePath, string destinationPath)
    {
        if (string.IsNullOrEmpty(packagePath) || !File.Exists(packagePath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid package path.", "OK");
            return;
        }

        if (string.IsNullOrEmpty(destinationPath) || !Directory.Exists(destinationPath))
        {
            EditorUtility.DisplayDialog("Error", "Invalid destination path.", "OK");
            return;
        }

        AssetDatabase.ImportPackage(packagePath, false);
        EditorUtility.DisplayDialog("Success", "Unity package unpacked successfully!", "OK");
    }
}
