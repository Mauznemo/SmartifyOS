using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SmartifyOS.SystemEvents
{
    public class SystemEventManager
    {
        private static string GetUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public static void CallEvent(string eventPath, string content, bool createIfNotExists = false)
        {
            eventPath = Path.Combine(GetUserPath(), eventPath);

            if (!File.Exists(eventPath))
            {
                if (!createIfNotExists)
                {
                    Debug.LogError("Event not found: " + eventPath);
                    return;
                }
                File.Create(eventPath).Dispose();
                Debug.Log("Created " + eventPath);
            }

            File.WriteAllText(eventPath, content);
        }

        public static void SubscribeToEvent(string eventPath, Action<string> onEvent)
        {
            eventPath = Path.Combine(GetUserPath(), eventPath);
            
            if (!File.Exists(eventPath))
            {
                Debug.LogError("Event not found: " + eventPath);
                return;
            }
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(eventPath),
                Filter = Path.GetFileName(eventPath)
            };

            watcher.Changed += (s, e) => { onEvent(File.ReadAllText(eventPath)); };

            watcher.EnableRaisingEvents = true;
        }
    }
}

