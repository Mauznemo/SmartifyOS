using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Welcome : EditorWindow
    {
        private Texture image;

        [MenuItem("Window/SmartifyOS/Welcome")]
        public static void ShowWindow()
        {
            var window = GetWindow<Welcome>("Welcome");
            window.SetSize(900, 500);
            window.ShowModal();
        }

        private void OnEnable()
        {
            image = AssetDatabase.LoadAssetAtPath<Texture>(EditorUtils.GetGraphicsPath() + "Welcome/SmartifyOS-welcome.png");
        }

        private void OnGUI()
        {
            var headingStyle = new GUIStyle(EditorStyles.label);
            headingStyle.fontStyle = FontStyle.Bold;
            headingStyle.fontSize = 40;
            headingStyle.padding = new RectOffset(20, 0, -30, 0);

            GUILayout.Space(30);
            EditorGUILayout.LabelField("Welcome to SmartifyOS", headingStyle);

            var imageStyle = new GUIStyle(EditorStyles.label);

            imageStyle.padding = new RectOffset(0, 0, -25, -24);

            GUILayout.Label(image, imageStyle);


            EditorGUILayout.BeginHorizontal();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 16;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Next", buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {

            }

            EditorGUILayout.EndHorizontal();
        }
    }
}


