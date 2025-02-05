using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    [CustomEditor(typeof(ValueThemer))]
    public class ValueThemer_Editor : UnityEditor.Editor
    {
        private ValueStyles valueStyles;
        private int selectedStyleIndex = 0;
        private int lastSelectedStyleIndex = 0;

        ValueThemer valueThemer;

        private void OnEnable()
        {
            valueThemer = (ValueThemer)target;
            valueStyles = ThemeData.GetThemeData().valueStyles;

            if (string.IsNullOrEmpty(valueThemer.styleName))
            {
                selectedStyleIndex = 0;
                lastSelectedStyleIndex = 1;
            }
            else
            {
                selectedStyleIndex = valueStyles.styles.Keys.ToList().IndexOf(valueThemer.styleName);
                lastSelectedStyleIndex = selectedStyleIndex;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty updateColorProperty = serializedObject.FindProperty("updateValue");

            GUILayout.Label("Style to apply", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.Label(ThemeData.GetThemeData().GetValue(valueThemer.styleName).ToString("0.00"), GUILayout.Width(40));

            selectedStyleIndex = EditorGUILayout.Popup(selectedStyleIndex, valueStyles.styles.Keys.ToArray());

            if (selectedStyleIndex != lastSelectedStyleIndex)
            {
                lastSelectedStyleIndex = selectedStyleIndex;
                valueThemer.styleName = valueStyles.styles.Keys.ToArray()[selectedStyleIndex];
                valueThemer.UpdateValue(ThemeData.GetThemeData().GetValue(valueThemer.styleName));
                EditorUtility.SetDirty(valueThemer);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            if (GUILayout.Button("Reload", GUILayout.MaxWidth(100)))
            {
                valueThemer.styleName = valueStyles.styles.Keys.ToArray()[selectedStyleIndex];
                valueThemer.UpdateValue(ThemeData.GetThemeData().GetValue(valueThemer.styleName));
                EditorUtility.SetDirty(valueThemer);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("       Multiplier");
            valueThemer.multiplier = EditorGUILayout.FloatField(valueThemer.multiplier, GUILayout.Width(50));

            GUILayout.Label($"x {ThemeData.GetThemeData().GetValue(valueThemer.styleName).ToString("0.00")} = {(valueThemer.multiplier * ThemeData.GetThemeData().GetValue(valueThemer.styleName)).ToString("0.00")}");

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Update"))
            {
                valueThemer.UpdateValue(ThemeData.GetThemeData().GetValue(valueThemer.styleName));
                EditorUtility.SetDirty(valueThemer);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("Color filed(s) to apply theme to", EditorStyles.boldLabel);


            EditorGUILayout.PropertyField(updateColorProperty);

            // Apply the change and enforce "Editor and Runtime" if needed
            UnityEventBase unityEvent = valueThemer.OnUpdateFloat;
            for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
            {
                if (unityEvent.GetPersistentListenerState(i) != UnityEventCallState.EditorAndRuntime)
                {
                    unityEvent.SetPersistentListenerState(i, UnityEventCallState.EditorAndRuntime);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}