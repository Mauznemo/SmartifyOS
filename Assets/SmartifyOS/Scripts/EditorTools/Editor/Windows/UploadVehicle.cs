using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using SmartifyOS.Editor.Styles;

namespace SmartifyOS.Editor
{
    public class UploadVehicle : EditorWindow
    {
        [MenuItem("Window/SmartifyOS/Upload Vehicle")]
        public static void ShowWindow()
        {
            var window = GetWindow<UploadVehicle>("Upload Vehicle");
            window.Show();
            window.minSize = new Vector2(400, 350);
        }

        private string brand;
        private string model;
        private string variant;

        private bool accepted;

        private void OnGUI()
        {
            Title();

            GUILayout.Space(30);

            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontStyle = FontStyle.Bold;
            textFieldStyle.fontSize = 13;
            textFieldStyle.padding = new RectOffset(10, 5, 7, 5);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = 13;
            //textFieldStyle.padding = new RectOffset(10, 5, 7, 5);

            EditorGUILayout.LabelField("Brand (e.g. Mazda)", labelStyle);
            brand = EditorGUILayout.TextField(brand, textFieldStyle, GUILayout.Height(30));

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Model (e.g. MX-5)", labelStyle);
            model = EditorGUILayout.TextField(model, textFieldStyle, GUILayout.Height(30));

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Variant (e.g. NA)", labelStyle);
            variant = EditorGUILayout.TextField(variant, textFieldStyle, GUILayout.Height(30));

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                accepted = EditorGUILayout.ToggleLeft("Accept the User Content Submission Agreement", accepted, GUILayout.Width(300));
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                GUI.enabled = accepted && !string.IsNullOrEmpty(brand) && !string.IsNullOrEmpty(model) && !string.IsNullOrEmpty(variant);
                if (GUILayout.Button("Upload", Style.LargeButton, GUILayout.MaxWidth(400), GUILayout.Height(40)))
                {
                    UploadFile(new FileInfo("X:/projects/Python/calculator.py"));
                }
                GUI.enabled = true;
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();

                if (LinkLabel(new GUIContent("User Content Submission Agreement")))
                {
                    UserContentSubmissionAgreement.ShowWindow();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndHorizontal();

        }

        private void Title()
        {
      
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Upload your own vehicle!", Style.Heading);

            var descriptionStyle = new GUIStyle(EditorStyles.label);
            descriptionStyle.padding = new RectOffset(5, 0, 0, 0);
            descriptionStyle.wordWrap = true;

            EditorGUILayout.LabelField("Upload your own vehicle 3D model for other people to use.", descriptionStyle);
        }

        private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            GUIStyle LinkStyle = new GUIStyle(EditorStyles.label);
            LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
            LinkStyle.stretchWidth = false;

            var position = GUILayoutUtility.GetRect(label, LinkStyle, options);

            Handles.BeginGUI();
            Handles.color = LinkStyle.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);

            return GUI.Button(position, label, LinkStyle);
        }

        private void UploadFile(FileInfo fileInfo)
        {
            string url = "http://localhost:5173/api/upload-file"; // Replace with your actual API endpoint

            // Create a UnityWebRequest
            Networking.POST(url, CreateForm(fileInfo), (success, message) =>
            {
                if (success)
                {
                    Debug.Log(message);
                }
                else
                {
                    Debug.LogError(message);
                }
            });

        }

        private WWWForm CreateForm(FileInfo fileInfo)
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", File.ReadAllBytes(fileInfo.FullName), fileInfo.Name);
            form.AddField("brand", brand);
            form.AddField("model", model);
            form.AddField("variant", variant);

            return form;
        }
    }
}


