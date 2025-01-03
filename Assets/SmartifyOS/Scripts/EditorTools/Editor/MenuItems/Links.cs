using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Links
    {
        [MenuItem("SmartifyOS/Report Issue", false, 201)]
        static void ReportIssue()
        {
            Application.OpenURL("https://github.com/Mauznemo/SmartifyOS/issues");
        }

        [MenuItem("SmartifyOS/Support the project/Github Sponsor", false, 203)]
        static void SupportTheProject()
        {
            Application.OpenURL("https://github.com/sponsors/Mauznemo");
        }

        [MenuItem("SmartifyOS/Support the project/PayPal", false, 202)]
        static void SupportTheProjectPayPal()
        {
            Application.OpenURL("https://www.paypal.com/donate/?hosted_button_id=BSPF2HUZRP7AN");
        }

        [MenuItem("SmartifyOS/Join Discord", false, 204)]
        static void JoinDiscord()
        {
            Application.OpenURL("https://discord.gg/dYf8zrVUHt");
        }

        [MenuItem("SmartifyOS/Documentation", false, 200)]
        static void Documentation()
        {
            Application.OpenURL("https://docs.smartify-os.com/docs/intro");
        }
    }
}

