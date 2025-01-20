using System;
using System.IO;
using SmartifyOS.Editor.Theming;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class WindowCreator : EditorWindow
    {
        public static void ShowWindow(bool fullscreen)
        {
            var window = CreateInstance<WindowCreator>();
            window.titleContent = new GUIContent("Create Window");
            window.ShowUtility();
            window.SetSize(400, 300);
            window.fullscreen = fullscreen;
        }

        private void OnEnable()
        {
            parent = GameObject.Find("Canvas");
            if (Directory.Exists("Assets/Scripts/UI/Windows/"))
            {
                defaultPath = "Assets/Scripts/UI/Windows/";
            }
            LoadPrefabs();
        }

        private GameObject parent;
        private GameObject windowPrefab;
        private GameObject fullscreenWindowPrefab;
        private string windowTitle = "New Window";
        private Vector2Int size = new Vector2Int(400, 300);
        private bool fullscreen = false;
        private bool createScript = true;

        private string defaultPath = "Assets";
        private string path = "";

        private GameObject window;

        private void OnGUI()
        {
            if (createScript && isWaiting)
            {
                WaitForDelay();

                var style = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold
                };

                GUILayout.FlexibleSpace();
                GUILayout.Label("Creating window and assigning script...", style);
                GUILayout.Label("Please do nothing until this window closes.", style);
                GUILayout.FlexibleSpace();
                return;
            }

            GUILayout.Space(10);

            if (parent == null)
            {
                parent = UnityEditor.Selection.activeGameObject;
            }

            parent = (GameObject)EditorGUILayout.ObjectField("Parent GameObject", parent, typeof(GameObject), true);

            GUILayout.Space(10);

            GUILayout.Label("Window Tile:");
            windowTitle = EditorGUILayout.TextField(windowTitle);

            GUILayout.Space(10);

            fullscreen = GUILayout.Toggle(fullscreen, new GUIContent("Fullscreen", "Should the window be fullscreen"));
            GUI.enabled = !fullscreen;
            size = EditorGUILayout.Vector2IntField("Window Size:", size);
            GUI.enabled = true;

            GUILayout.Space(10);

            createScript = GUILayout.Toggle(createScript, new GUIContent("Create Script", "Also create and assign a new script to the window"));

            GUI.enabled = createScript;
            GUILayout.BeginHorizontal();
            string relativePath = GetRelativePath(path, Application.dataPath);
            GUILayout.Label(new GUIContent($"Script Path: {relativePath.TrimLength(40)}", $"Path: {relativePath}"));
            if (GUILayout.Button("Browse"))
            {
                path = EditorUtility.SaveFilePanel(
                    "Save new script",      // Title of the dialog
                    defaultPath,                   // Default directory
                    $"{windowTitle.ToPascalCase()}.cs",         // Default file name
                    "cs"                 // File extension
                );
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;

            GUILayout.FlexibleSpace();

            GUI.enabled = createScript ? path != "" : true;
            if (GUILayout.Button("Create", GUILayout.Height(40)))
            {
                if (fullscreen)
                {
                    window = CreateFullscreenWindowFromPrefab(windowTitle);
                }
                else
                {
                    window = CreateWindowFromPrefab(windowTitle);
                }

                if (window.TryGetComponent(out ColorThemer colorThemer))
                {
                    colorThemer.UpdateColor(ThemeData.GetThemeData().GetColor(colorThemer.styleName));
                    EditorUtility.SetDirty(colorThemer);
                }

                if (window.TryGetComponent(out ValueThemer valueThemer))
                {
                    valueThemer.UpdateValue(ThemeData.GetThemeData().GetValue(valueThemer.styleName));
                    EditorUtility.SetDirty(valueThemer);
                }

                if (createScript)
                {
                    ScriptTemplateUtility.CreateScript($"{ScriptTemplateUtility.GetTemplatesPath()}/UIWindow.cs.txt", windowTitle.ToPascalCase(), path);

                    ScheduleDelayedAction();

                    AssetDatabase.Refresh();
                }

                if (!createScript)
                    Close();
            }
            GUI.enabled = true;
        }

        private string GetRelativePath(string fullPath, string basePath)
        {
            if (fullPath.StartsWith(basePath))
            {
                return "Assets" + fullPath.Substring(basePath.Length).Replace("\\", "/");
            }
            return fullPath;
        }

        private void LoadPrefabs()
        {
            windowPrefab = Resources.Load<GameObject>("Prefabs/UI/WindowPrefab");

            fullscreenWindowPrefab = Resources.Load<GameObject>("Prefabs/UI/FullscreenWindowPrefab");

            if (windowPrefab == null || fullscreenWindowPrefab == null)
            {
                Debug.LogError("Failed to load prefabs");
            }
        }

        private double startTime;
        private bool isWaiting;

        private void ScheduleDelayedAction()
        {
            startTime = EditorApplication.timeSinceStartup;
            isWaiting = true;
        }

        private void WaitForDelay()
        {
            if (EditorApplication.timeSinceStartup - startTime >= 2)
            {
                if (!AssetDatabase.AssetPathExists(GetRelativePath(path, Application.dataPath)))
                {
                    ScheduleDelayedAction();
                    return;
                }

                isWaiting = false;

                AddScriptComponent(window, path);

                Close();
            }
        }

        private void AddScriptComponent(GameObject gameObject, string filePath)
        {
            // Load the script as a MonoScript
            MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(GetRelativePath(filePath, Application.dataPath));

            if (monoScript == null)
            {
                Debug.LogError("Script not found at the specified path.");
                return;
            }

            // Get the class type from the MonoScript
            System.Type scriptType = monoScript.GetClass();

            if (scriptType == null)
            {
                Debug.LogError("Unable to get class type from the script. Ensure the script compiles and contains a valid MonoBehaviour class.");
                return;
            }

            // Add the script as a component
            gameObject.AddComponent(scriptType);
        }

        private GameObject CreateFullscreenWindowFromPrefab(string name)
        {
            if (parent == null)
            {
                throw new Exception("Parent GameObject is not assigned.");
            }

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(fullscreenWindowPrefab);


            PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            prefabInstance.name = name.ToPascalCase();

            GameObjectUtility.SetParentAndAlign(prefabInstance, parent);

            var g = GameObject.Find("------ Put nothing under here! ------"); //TODO: Find a better way to find the object
            prefabInstance.transform.SetSiblingIndex(g.transform.GetSiblingIndex());

            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create " + prefabInstance.name);

            Selection.activeObject = prefabInstance;

            GetTitleText(prefabInstance).text = name;
            return prefabInstance;
        }

        private GameObject CreateWindowFromPrefab(string name)
        {
            if (parent == null)
            {
                throw new Exception("Parent GameObject is not assigned.");
            }

            if (size.x < 10 || size.y < 10)
            {
                throw new Exception("This window is too small!");
            }

            if (size.x > 10_000 || size.y > 10_000)
            {
                throw new Exception("This window is too big!");
            }

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(windowPrefab);


            PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            prefabInstance.name = name.ToPascalCase();

            GameObjectUtility.SetParentAndAlign(prefabInstance, parent);

            var g = GameObject.Find("------ Put nothing under here! ------"); //TODO: Find a better way to find the object
            prefabInstance.transform.SetSiblingIndex(g.transform.GetSiblingIndex());

            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create " + prefabInstance.name);

            Selection.activeObject = prefabInstance;

            SetSize(prefabInstance);

            GetTitleText(prefabInstance).text = name;
            return prefabInstance;
        }

        private void SetSize(GameObject window)
        {
            RectTransform rectTransform = (RectTransform)window.transform;
            rectTransform.sizeDelta = size;
        }

        private TMP_Text GetTitleText(GameObject parent)
        {
            return parent.GetComponentInChildren<TMP_Text>();
        }

    }
}
