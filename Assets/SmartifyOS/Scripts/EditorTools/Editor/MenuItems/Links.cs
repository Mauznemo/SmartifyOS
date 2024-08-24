using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Links
    {
        [MenuItem("Window/SmartifyOS/Report Issue", false)]
        static void ReportIssue()
        {
            Application.OpenURL("https://github.com/Mauznemo/SmartifyOS/issues");
        }

        [MenuItem("Window/SmartifyOS/Support the project", false)]
        static void SupportTheProject()
        {
            Application.OpenURL("https://github.com/sponsors/Mauznemo");
        }

        [MenuItem("Window/SmartifyOS/Join Discord", false)]
        static void JoinDiscord()
        {
            Application.OpenURL("https://discord.gg/dYf8zrVUHt");
        }
    }
}

