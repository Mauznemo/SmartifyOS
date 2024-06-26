using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public static class EditorUtils
    {
        public static void SetSize(this EditorWindow window, float width, float hight)
        {
            window.minSize = new Vector2(width, hight);
            window.maxSize = new Vector2(width, hight);
        }

        public static string GetGraphicsPath()
        {
            return GetSmartifyOSPath() + "Scripts/EditorTools/Resources/Graphics/";
        }

        public static Texture GetIcon(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Texture>(GetGraphicsPath() + "ButtonIcons/" + name + ".png");
        }

        private static string path;

        public static string GetSmartifyOSPath()
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            StackTrace stackTrace = new StackTrace(true);
            StackFrame frame = stackTrace.GetFrame(0);
            string filePath = frame.GetFileName();
            DirectoryInfo dir = new DirectoryInfo(filePath);

            DirectoryInfo fullPath = dir.GetParentDirectory(4);
            DirectoryInfo projectPath = new DirectoryInfo(Application.dataPath).Parent;

            path = fullPath.FullName.Replace(projectPath.FullName, "").Replace("\\", "/") + "/";
            path = path.Substring(1);
            return path;
        }

        public static DirectoryInfo GetParentDirectory(this DirectoryInfo dir, int levelsUp)
        {
            DirectoryInfo currentDir = dir;

            for (int i = 0; i < levelsUp; i++)
            {
                if (currentDir.Parent == null)
                {
                    throw new DirectoryNotFoundException($"Cannot go up {levelsUp} levels from {dir.FullName}");
                }
                currentDir = currentDir.Parent;
            }

            return currentDir;
        }
    }
}


