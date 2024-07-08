using UnityEditor;

namespace SmartifyOS.Editor
{
    public class FileCreator
    {
        [MenuItem("Assets/Create/SmartifyOS/Serial Communication Script", false, 80)]
        static void CreateSerialCommunicationScript()
        {
            string templatePath = ScriptTemplateUtility.GetTemplatesPath() + "SerialCommunication.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewSerialCommunicationScript.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/Live Serial Communication Script", false, 80)]
        static void CreateLiveSerialCommunicationScript()
        {
            string templatePath = ScriptTemplateUtility.GetTemplatesPath() + "LiveSerialCommunication.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewLiveSerialCommunicationScript.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/UI Window Script", false, 80)]
        static void CreateUIWindowScript()
        {
            string templatePath = ScriptTemplateUtility.GetTemplatesPath() + "UIWindow.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewUIWindow.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/More/Quick Settings Entry", false, 82)]
        static void CreateQuickSettingsEntryScript()
        {
            string templatePath = ScriptTemplateUtility.GetTemplatesPath() + "QuickSettingsEntry.cs.txt";

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewQuickSettingsEntry.cs");
        }

        [MenuItem("Assets/Create/SmartifyOS/Add Audio Config", false, 80)]
        static void CreateAudioConfigPartialClass()
        {
            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            CreateFileWithSuffix.CreateCustomScript("NewAddition", "_AudioConfig_SO", "AudioConfig_SO.cs.txt", directoryPath);
        }

        [MenuItem("Assets/Create/SmartifyOS/Save System/Add Save Data", false, 80)]
        static void CreateSaveDataPartialClass()
        {
            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            CreateFileWithSuffix.CreateCustomScript("NewAddition", "_SaveData", "SaveData.cs.txt", directoryPath);
        }
    }
}