using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.Editor
{
    [InitializeOnLoad]
    public class AutoStart
    {
        static AutoStart()
        {
            EditorApplication.update += Update;
            
        }

        public static void Update()
        {
            if (!SessionState.GetBool("FirstInitDone", false))
            {
                SessionState.SetBool("FirstInitDone", true);
                Welcome.ShowWindow();
                EditorApplication.update -= Update;

            }
        }
    }
}

