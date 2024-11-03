using System.IO;
using SmartifyOS.Editor.Styles;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class BlankProject : EditorWindow
    {
        private bool removeSceneObjects = true;
        private bool removeScriptFiles = false;

        private bool understood = false;

        private bool sceneCleared = false;
        private bool filesCleared = false;

        private BlankProjectReferences blankProjectReferences;

        private void OnEnable()
        {
            blankProjectReferences = GameObject.Find("BlankProjectReferences").GetComponent<BlankProjectReferences>();

            sceneCleared = IsSceneCleared();
            filesCleared = AreFilesCleared();
        }

        [MenuItem("SmartifyOS/Create blank Project")]
        public static void ShowWindow()
        {
            var window = GetWindow<BlankProject>("Blank Project");
            window.minSize = new Vector2(500, 300);
        }

        public void OnGUI()
        {
            GUILayout.Label($"Create a blank project", EditorStyles.boldLabel);
            GUI.color = Color.gray;
            GUILayout.Label($"Please have a look at the included Mazda Miata NA example and try understanding it before removing it from the project! (You can also modify the example instead of starting from scratch)", EditorStyles.wordWrappedLabel);
            GUI.color = Color.white;

            GUILayout.Space(20);

            if (blankProjectReferences == null)
            {
                GUI.color = Color.red;
                GUILayout.Label($"Error: Missing Blank Project References", EditorStyles.boldLabel);
                GUI.color = Color.white;
                return;
            }

            GUILayout.Label("What you want to remove from the project?");

            GUI.enabled = !sceneCleared;
            removeSceneObjects = GUILayout.Toggle(removeSceneObjects, new GUIContent("Remove Scene Objects", "All objects that are specific for the included example (Mazda Miata NA) will be removed from the scene."));

            GUI.enabled = !filesCleared;
            removeScriptFiles = GUILayout.Toggle(removeScriptFiles, new GUIContent("Remove Script Files", "All script files that are specific for the included example (Mazda Miata NA) will be removed."));
            GUI.enabled = true;

            GUILayout.Space(20);

            GUI.color = Color.red;
            GUILayout.Label($"If you modified any example file or object it will still be removed!", EditorStyles.boldLabel);
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            understood = GUILayout.Toggle(understood, "I read and understand the red text");

            GUI.enabled = understood;

            if (GUILayout.Button("Remove", Style.Button, GUILayout.Height(30)))
            {
                RemoveSceneObjects();
                RemoveScriptFiles();
            }
        }

        private void RemoveSceneObjects()
        {
            if (!removeSceneObjects)
                return;

            if (sceneCleared)
                return;

            foreach (GameObject obj in blankProjectReferences.objectsToRemove)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }

            sceneCleared = true;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        private void RemoveScriptFiles()
        {
            if (!removeScriptFiles)
                return;

            if (filesCleared)
                return;

            File.Move("Assets/Scripts/UI/UIManager.cs", "Assets/UIManager.cs.bak");
            File.Move("Assets/Scripts/UI/InfoDisplay.cs", "Assets/InfoDisplay.cs.bak");

            DeleteFilesInFolder("Assets/Scripts");
            DeleteFilesInFolder("Assets/ScriptableObjects");

            if (!Directory.Exists("Assets/Scripts/UI/"))
                Directory.CreateDirectory("Assets/Scripts/UI/");

            File.Move("Assets/UIManager.cs.bak", "Assets/Scripts/UI/UIManager.cs");
            File.Move("Assets/InfoDisplay.cs.bak", "Assets/Scripts/UI/InfoDisplay.cs");

            ReplaceTextInFile("Assets/SmartifyOS/Scripts/System/UI/Components/MediaPlayer/Bluetooth/BluetoothPlayer.cs", @"if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Hide(true);
                allowAutoOpen = false;
            }", "//Auto removed by blank project creator");

            ReplaceTextInFile("Assets/SmartifyOS/Scripts/System/UI/Components/MediaPlayer/Bluetooth/BluetoothPlayer.cs", @"if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Show();
                allowAutoOpen = true;
            }", "//Auto removed by blank project creator");

            ReplaceTextInFile("Assets/SmartifyOS/Scripts/System/UI/Components/MediaPlayer/Files/FilePlayer.cs", @"if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Hide(true);
            }", "//Auto removed by blank project creator");

            ReplaceTextInFile("Assets/SmartifyOS/Scripts/System/UI/Components/MediaPlayer/Files/FilePlayer.cs", @"if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Show();
            }", "//Auto removed by blank project creator");

            filesCleared = true;

            AssetDatabase.Refresh();
        }

        private void DeleteFilesInFolder(string path)
        {
            if (!removeScriptFiles)
                return;

            if (filesCleared)
                return;

            if (Directory.Exists(path))
            {
                // Delete all files in the Scripts folder
                foreach (string file in Directory.GetFiles(path))
                {
                    File.Delete(file);
                    File.Delete(file + ".meta");
                }

                // Delete all subdirectories and their contents in the Scripts folder
                foreach (string dir in Directory.GetDirectories(path))
                {
                    Directory.Delete(dir, true);
                    File.Delete(dir + ".meta"); // Delete the meta file for the directory
                }

                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogWarning($"{path} folder does not exist.");
            }
        }

        private bool AreFilesCleared()
        {
            if (File.Exists("Assets/Scripts/StatisticsManager.cs"))
                return false;

            if (File.Exists("Assets/Scripts/Arduinos/MainController.cs"))
                return false;

            if (File.Exists("Assets/Scripts/Arduinos/MainController.cs"))
                return false;

            if (File.Exists("Assets/Scripts/Arduinos/MainController.cs"))
                return false;

            return true;
        }

        private bool IsSceneCleared()
        {
            foreach (GameObject obj in blankProjectReferences.objectsToRemove)
            {
                if (obj != null)
                    return false;
            }
            return true;
        }

        private void ReplaceTextInFile(string path, string search, string replace)
        {
            if (File.Exists(path))
            {
                string content = File.ReadAllText(path);
                if (content.Contains(search))
                {
                    content = content.Replace(search, replace);
                    File.WriteAllText(path, content);
                }
                else
                {
                    Debug.LogWarning($"Search text '{search}' not found in {path}");
                }
            }
            else
            {
                Debug.LogError($"File not found at path: {path}");
            }
        }
    }

}

