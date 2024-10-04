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

        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Converters = { new ColorConverter() }
        };

        private static SaveData saveData;

        /// <summary>
        /// Saves the <see cref="SaveData" to file. DON'T CALL THIS AFTER EVERY CHANGE!/>
        /// </summary>
        public static void Save()
        {
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented, settings);

            Debug.Log(json);

            //Temporary fix for sometimes saving null
            if (json.Trim() == "null")
            {
                return;
            }

            File.WriteAllText(Application.persistentDataPath + "/" + SAVE_NAME, json);
        }

        /// <summary>
        /// Removes the save file
        /// </summary>
        public static void Remove()
        {

            if (Exists(SAVE_NAME))
            {
                File.Delete(Application.persistentDataPath + "/" + SAVE_NAME);
            }
        }

        /// <summary>
        /// Loads the save file
        /// </summary>
        /// <returns>Current SaveData (can directly be modified and will save to will when the application closes)</returns>
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
                return JsonConvert.DeserializeObject<SaveData>(fileContent, settings);
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


