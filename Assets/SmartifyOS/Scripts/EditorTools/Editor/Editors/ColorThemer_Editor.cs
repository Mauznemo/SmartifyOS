using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    [CustomEditor(typeof(ColorThemer))]
    public class ColorThemer_Editor : UnityEditor.Editor
    {
        private Texture2D colorTexture;
        private ColorStyles colorStyles;
        private int selectedStyleIndex = 0;
        private int lastSelectedStyleIndex = 0;

        private ColorThemer colorThemer;

        private void OnEnable()
        {
            colorThemer = (ColorThemer)target;

            CreateColorTexture();
            colorStyles = ThemeData.GetThemeData().colorStyles;

            if (string.IsNullOrEmpty(colorThemer.styleName))
            {
                selectedStyleIndex = 0;
                lastSelectedStyleIndex = 1;
            }
            else
            {
                selectedStyleIndex = colorStyles.styles.Keys.ToList().IndexOf(colorThemer.styleName);
                lastSelectedStyleIndex = selectedStyleIndex;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty updateColorProperty = serializedObject.FindProperty("updateColor");

            GUILayout.Label("Style to apply", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUI.color = ThemeData.GetThemeData().GetColor(colorThemer.styleName);
            GUILayout.Label(colorTexture, GUILayout.Width(20), GUILayout.Height(20));
            GUI.color = Color.white;

            selectedStyleIndex = EditorGUILayout.Popup(selectedStyleIndex, colorStyles.styles.Keys.ToArray());

            if (selectedStyleIndex != lastSelectedStyleIndex)
            {
                lastSelectedStyleIndex = selectedStyleIndex;
                colorThemer.styleName = colorStyles.styles.Keys.ToArray()[selectedStyleIndex];
                colorThemer.UpdateValue(ThemeData.GetThemeData().GetColor(colorThemer.styleName));
                EditorUtility.SetDirty(colorThemer);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Reload", GUILayout.MaxWidth(100)))
            {
                colorThemer.styleName = colorStyles.styles.Keys.ToArray()[selectedStyleIndex];
                colorThemer.UpdateValue(ThemeData.GetThemeData().GetColor(colorThemer.styleName));
                EditorUtility.SetDirty(colorThemer);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Color filed(s) to apply theme to", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(updateColorProperty);


            // Apply the change and enforce "Editor and Runtime" if needed
            UnityEventBase unityEvent = colorThemer.OnUpdateColor;
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentListenerState(i) != UnityEventCallState.EditorAndRuntime)
                {
                    unityEvent.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateColorTexture()
        {
            // Initialize a larger texture
            colorTexture = new Texture2D(20, 20, TextureFormat.RGBA32, false);
            colorTexture.wrapMode = TextureWrapMode.Clamp;

            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    colorTexture.SetPixel(x, y, Color.white);
                }
            }

            // Apply the changes to the texture
            colorTexture.Apply();
        }
    }
}
