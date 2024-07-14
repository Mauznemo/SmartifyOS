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

        if(window.transform.localScale == Vector3.zero)
        {
            if(GUILayout.Button("Show Window"))
            {
                window.Show();
            }
        }
        else
        {
            if(GUILayout.Button("Hide Window"))
            {
                window.Hide();
            }
        }
    
        GUILayout.Space(10);

        DrawDefaultInspector();
    }
    }
}
