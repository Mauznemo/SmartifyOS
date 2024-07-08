using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace SmartifyOS.SaveSystem
{
    public class SaveManager
    {
        private const string SAVE_NAME = "settings.json";

        private static SaveData saveData;

        public static void Save()
        {
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);

            File.WriteAllText(Application.persistentDataPath + "/" + SAVE_NAME, json);
        }

        public static void Remove()
        {

            if (Exists(SAVE_NAME))
            {
                File.Delete(Application.persistentDataPath + "/" + SAVE_NAME);
            }
        }

        public static SaveData Load()
        {
            if (saveData == null)
            {
                saveData = LoadFile();
            }

            return saveData;
        }

        private static SaveData LoadFile()
        {
            if (Exists(SAVE_NAME))
            {
                var fileContent = File.ReadAllText(Application.persistentDataPath + "/" + SAVE_NAME);
                return JsonConvert.DeserializeObject<SaveData>(fileContent);
            }
            else
            {
                return new SaveData();
            }
        }

        private static bool Exists(string fileName)
        {
            return File.Exists(Application.persistentDataPath + "/" + fileName);
        }
    }
}


