using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class CreateFileWithSuffix : EditorWindow
    {
        private string baseName;
        private string suffix;
        private string templateName;
        private string path;

        public static void CreateCustomScript(string baseName, string suffix, string templateName, string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                UnityEngine.Debug.LogWarning("Please select a valid folder in the Project view.");
                return;
            }

            var window = GetWindow<CreateFileWithSuffix>();
            window.titleContent = new GUIContent("Create File With Suffix");
            window.SetSize(400, 200);
            window.ShowUtility();
            window.baseName = baseName;
            window.suffix = suffix;
            window.templateName = templateName;
            window.path = path;
        }

        private void OnGUI()
        {
            GUILayout.Label("Enter Name:");
            baseName = EditorGUILayout.TextField(baseName);

            if (GUILayout.Button("Create Script"))
            {
                string templatePath = ScriptTemplateUtility.GetTemplatesPath() + templateName;

                ScriptTemplateUtility.CreateScriptWithSuffix(templatePath, baseName, suffix, path);
                Close();
            }
        }
    }
}


