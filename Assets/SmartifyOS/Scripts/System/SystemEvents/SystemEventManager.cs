using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SmartifyOS.SystemEvents
{
    /// <summary>
    /// System event manager for communicating with external processes (eg. python scripts) using files.
    /// </summary>
    public class SystemEventManager
    {
        private static string GetUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        /// <summary>
        /// Modifies the file to trigger a change
        /// </summary>
        /// <param name="eventPath">Path to file, relative to user profile</param>
        /// <param name="content">Content to write in file (to add additional data)</param>
        /// <param name="createIfNotExists">Create the file if it doesn't exist</param>
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

        /// <summary>
        /// Subscribes to file changes
        /// </summary>
        /// <param name="eventPath">Path to file, relative to user profile<</param>
        /// <param name="onEvent">Called when the file changes</param>
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


