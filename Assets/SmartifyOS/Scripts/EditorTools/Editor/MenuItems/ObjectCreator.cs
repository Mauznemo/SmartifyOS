using UnityEditor;
using UnityEngine;
using SmartifyOS.UI.TranslucentImage;

namespace SmartifyOS.Editor
{
    public static class ObjectCreator
    {
        [MenuItem("GameObject/UI/SmartifyOS/Blurred Image", false)]
        public static void CreateBlurredImage(MenuCommand menuCommand)
        {
            var component = CreateObjectWithComponent<BlurredImage>(menuCommand, "Blurred Image");
            component.color = new Color(75, 75, 75);
        }

        [MenuItem("GameObject/UI/SmartifyOS/Button/Icon Button", false)]
        public static void CreateIconButton(MenuCommand menuCommand)
        {

        }

        [MenuItem("GameObject/UI/SmartifyOS/Button/Button", false)]
        public static void CreateButton(MenuCommand menuCommand)
        {

        }

        [MenuItem("GameObject/UI/SmartifyOS/Input Field", false)]
        public static void CreateInputField(MenuCommand menuCommand)
        {

        }

        [MenuItem("GameObject/UI/SmartifyOS/Toggle", false)]
        public static void CreateToggle(MenuCommand menuCommand)
        {

        }

        [MenuItem("GameObject/UI/SmartifyOS/More/Quick Settings/Toggle", false, 1180)]
        public static void CreateQsToggle(MenuCommand menuCommand)
        {
            CreateObjectFromPrefab(menuCommand, "Prefabs/UI/QuickSettings/ToggleButton", "QsToggle");
        }

        [MenuItem("GameObject/UI/SmartifyOS/More/Quick Settings/Button", false, 1180)]
        public static void CreateQsButton(MenuCommand menuCommand)
        {
            CreateObjectFromPrefab(menuCommand, "Prefabs/UI/QuickSettings/Button", "QsButton");
        }

        [MenuItem("GameObject/UI/SmartifyOS/Window", false)]
        public static void CreateWindow(MenuCommand menuCommand)
        {
            WindowCreator.ShowWindow(menuCommand.context as GameObject);
        }

        private static T CreateObjectWithComponent<T>(MenuCommand menuCommand, string name) where T : Component
        {
            GameObject customObject = new GameObject(name);

            var component = customObject.AddComponent<T>();

            GameObjectUtility.SetParentAndAlign(customObject, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(customObject, "Create " + customObject.name);

            Selection.activeObject = customObject;

            return component;
        }

        private static void CreateObjectFromPrefab(MenuCommand menuCommand, string prefabPath, string name)
        {
            //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError("Prefab not found at path: " + prefabPath);
                return;
            }

            GameObject prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            PrefabUtility.UnpackPrefabInstance(prefabInstance, PrefabUnpackMode.Completely, InteractionMode.UserAction);

            prefabInstance.name = name;

            GameObjectUtility.SetParentAndAlign(prefabInstance, menuCommand.context as GameObject);

            Undo.RegisterCreatedObjectUndo(prefabInstance, "Create " + prefabInstance.name);

            Selection.activeObject = prefabInstance;
        }

    }
}
