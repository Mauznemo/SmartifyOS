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

        [MenuItem("Window/SmartifyOS/Support the project/Github Sponsor", false)]
        static void SupportTheProject()
        {
            Application.OpenURL("https://github.com/sponsors/Mauznemo");
        }

        [MenuItem("Window/SmartifyOS/Support the project/PayPal", false)]
        static void SupportTheProjectPayPal()
        {
            Application.OpenURL("https://www.paypal.com/donate/?hosted_button_id=BSPF2HUZRP7AN");
        }

        [MenuItem("Window/SmartifyOS/Join Discord", false)]
        static void JoinDiscord()
        {
            Application.OpenURL("https://discord.gg/dYf8zrVUHt");
        }
    }
}

