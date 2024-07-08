using System.IO;
using UnityEditor;

namespace SmartifyOS.Editor
{
    public class ScriptTemplateUtility
    {
        public static void CreateScriptWithSuffix(string templatePath, string baseName, string suffix, string path)
        {
            string templateText = File.ReadAllText(templatePath);

            baseName = baseName.ToPascalCase();
            string fileNameWithSuffix = baseName + suffix + ".cs";

            string fileContent = templateText.Replace("#SCRIPTNAME#", baseName);

            string filePath = Path.Combine(path, fileNameWithSuffix);

            File.WriteAllText(filePath, fileContent);
            AssetDatabase.Refresh();
        }

        public static string GetTemplatesPath()
        {
            return EditorUtils.GetSmartifyOSPath() + "Scripts/EditorTools/Editor/MenuItems/ScriptTemplates/";
        }
    }

}

