using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SmartifyOS.Editor.Styles
{
    public static class Style
    {
        public static GUIStyle LargeHeading
        {
            get
            {
                var headingStyle = new GUIStyle(EditorStyles.label);
                headingStyle.fontStyle = FontStyle.Bold;
                headingStyle.fontSize = 30;
                headingStyle.wordWrap = true;
                headingStyle.padding = new RectOffset(0, 0, -20, 0);
                return headingStyle;
            }
        }

        public static GUIStyle Heading
        {
            get
            {
                var headingStyle = new GUIStyle(EditorStyles.label);
                headingStyle.fontStyle = FontStyle.Bold;
                headingStyle.fontSize = 20;
                headingStyle.padding = new RectOffset(5, 0, -20, 0);
                return headingStyle;
            }
        }

        public static GUIStyle Button
        {
            get
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = 13;
                return buttonStyle;
            }
        }

        public static GUIStyle LargeButton
        {
            get
            {
                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = 16;
                return buttonStyle;
            }
        }

        public static GUIStyle Popup
        {
            get
            {
                GUIStyle popupStyle = new GUIStyle(EditorStyles.popup);
                popupStyle.fontStyle = FontStyle.Bold;
                popupStyle.fontSize = 13;
                popupStyle.fixedHeight = 30;
                popupStyle.padding = new RectOffset(8, 0, 0, 0);
                return popupStyle;
            }
        }
    }

}

