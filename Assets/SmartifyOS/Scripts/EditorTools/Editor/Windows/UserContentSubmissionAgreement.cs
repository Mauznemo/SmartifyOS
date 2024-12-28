using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class UserContentSubmissionAgreement : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = CreateInstance<UserContentSubmissionAgreement>();
            window.titleContent = new GUIContent("User Content Submission Agreement");
            window.SetSize(700, 400);
            window.ShowModalUtility();

        }

        private void OnGUI()
        {
            var headingStyle = new GUIStyle(EditorStyles.label);
            headingStyle.fontStyle = FontStyle.Bold;
            headingStyle.fontSize = 20;
            headingStyle.padding = new RectOffset(5, 0, -20, 0);

            GUILayout.Space(30);
            EditorGUILayout.LabelField("User Content Submission Agreement", headingStyle);

            var descriptionStyle = new GUIStyle(EditorStyles.label);
            descriptionStyle.padding = new RectOffset(5, 0, 0, 0);
            descriptionStyle.wordWrap = true;

            EditorGUILayout.LabelField(@"
By uploading any 3D model to SmartifyOS, you agree to the following terms and conditions:

    1. Ownership and Rights: You affirm that you are the creator of the content or have obtained all necessary rights.

    2. Representations and Warranties: You represent that the content does not infringe any third party's rights.

    3. Indemnification: You agree to indemnify SmartifyOS against any claims related to the content.

    4. Content Removal: SmartifyOS may remove content at its discretion without notice.

    5. Modifications: SmartifyOS may modify these terms at any time.

By uploading content, you acknowledge and agree to these terms.

", descriptionStyle);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 16;

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close", buttonStyle, GUILayout.MaxWidth(200), GUILayout.Height(40)))
                {
                    Close();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}

