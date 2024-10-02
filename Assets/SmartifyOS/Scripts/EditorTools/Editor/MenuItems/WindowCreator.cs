using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class WindowCreator : EditorWindow
    {
        [MenuItem("SmartifyOS/Create Window")]
        public static void ShowWindow()
        {
            var window = GetWindow<WindowCreator>("Create Window");
            window.Show();
            window.SetSize(400, 200);
        }

        public static void ShowWindow(GameObject parent)
        {
            var window = GetWindow<WindowCreator>("Create Window");
            window.Show();
            window.SetSize(400, 200);
        }

        private void OnEnable()
        {
            LoadPrefab();
        }

        private GameObject parent;
        private GameObject loadedPrefab;
        private string windowTitle = "New Window";
        private Vector2Int size = new Vector2Int(400, 300);

        private void OnGUI()
        {
            GUILayout.Space(10);

            if (parent == null)
            {
                parent = UnityEditor.Selection.activeGameObject;
            }

            parent = (GameObject)EditorGUILayout.ObjectField("Parent GameObject", parent, typeof(GameObject), true);

            GUILayout.Space(10);

            GUILayout.Label("Window Tile:");
            windowTitle = EditorGUILayout.TextField(windowTitle);

            size = EditorGUILayout.Vector2IntField("Window Size:", size);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Create", GUILayout.Height(40)))
            {
                CreateObjectFromPrefab(windowTitle);
            }
        }

        private void LoadPrefab()
        {
            loadedPrefab = Resources.Load<GameObject>("Prefabs/UI/WindowPrefab");

            if (loadedPrefab == null)
            {
                Debug.LogError("Failed to load prefab (WindowPrefab)");
            }
        }


        private void CreateObjectFromPrefab(string name)
        {
            if (parent == null)
            {
                Debug.LogError("Parent GameObject is not assigned.");
                return;
            }

            if (size.x < 10 || size.y < 10)
            {
                Debug.LogError("This window is too small!");
                return;
            }

            if (size.x > 10_000 || size.y > 10_000)
            {
                Debug.LogError("This window is too big!");
                return;
            }

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(loadedPrefab);


            PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            prefabInstance.name = name.ToPascalCase();

            GameObjectUtility.SetParentAndAlign(prefabInstance, parent);

            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create " + prefabInstance.name);

            Selection.activeObject = prefabInstance;

            SetSize(prefabInstance);

            GetTitleText(prefabInstance).text = name;
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
