using UnityEditor;

namespace SmartifyOS.Editor
{
    public class FileCreator
    {
        [MenuItem("Assets/Create/SmartifyOS/Serial Communication Script", false, 80)]
        static void CreateSerialCommunicationScript()
        {
            string templatePath = GetTemplatesPath() + "SerialCommunication.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewSerialCommunicationScript.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/Live Serial Communication Script", false, 80)]
        static void CreateLiveSerialCommunicationScript()
        {
            string templatePath = GetTemplatesPath() + "LiveSerialCommunication.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewLiveSerialCommunicationScript.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/UI Window Script", false, 80)]
        static void CreateUIWindowScript()
        {
            string templatePath = GetTemplatesPath() + "UIWindow.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewUIWindow.cs");
        }

        private static string GetTemplatesPath()
        {
            return EditorUtils.GetSmartifyOSPath() + "Scripts/EditorTools/Editor/MenuItems/ScriptTemplates/";
        }
    }
}