using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI
{
    [CustomEditor(typeof(BaseUIWindow), true)]
    public class BaseUIWindow_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Get the target object
            BaseUIWindow window = (BaseUIWindow)target;

            GUILayout.BeginHorizontal();
            if (window.transform.localScale == Vector3.zero)
            {
                if (GUILayout.Button("Show Window"))
                {
                    window.Show();
                }
            }
            else
            {
                if (GUILayout.Button("Hide Window"))
                {
                    window.Hide();
                }
            }

            if (GUILayout.Button(new GUIContent("Remove", "Remove window and delete its script file"), GUILayout.Width(60)))
            {
                window.RemoveAndDeleteFile();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            DrawDefaultInspector();
        }
    }
}
