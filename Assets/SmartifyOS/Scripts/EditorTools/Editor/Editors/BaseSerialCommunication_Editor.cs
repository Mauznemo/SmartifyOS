using System;
using System.Collections;
using System.Collections.Generic;
using SmartifyOS.Editor.Styles;
using Style = SmartifyOS.Editor.Styles;
using SmartifyOS.SerialCommunication;
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI
{
    [CustomEditor(typeof(BaseSerialCommunication), true)]
    public class BaseSerialCommunication_Editor : UnityEditor.Editor
    {
        private string message;
        private string history;
        BaseSerialCommunication script;
        private bool lastPlayModeState;

        public override void OnInspectorGUI()
        {

            script = (BaseSerialCommunication)target;

            if (script == null)
                return;

            CheckForPlayModeChange();

            var headingStyle = new GUIStyle(Style.Style.Heading)
            {
                padding = new RectOffset(0, 0, 0, 0)
            };
            var descStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };
            if (script.emulationMode)
            {
                GUILayout.Label("Serial Emulation", headingStyle);
                GUILayout.Label("You are the serial device (ex. Arduino). And receive messages sent to the serial device.", descStyle);
                GUILayout.Space(10);
            }


            ShowModeState(script);


            if (script.emulationMode)
            {
                GUILayout.Space(10);
                if (!Application.isPlaying)
                {
                    GUILayout.Label("Please enter playmode to send messages to the script", descStyle);
                }
                else
                {
                    ShowChatWindow();
                }
            }

            GUILayout.Space(10);
            DrawDefaultInspector();
        }

        private void OnResponseReceived(string obj)
        {
            history += "Received: " + obj + "\n";
        }

        private void CheckForPlayModeChange()
        {
            if (lastPlayModeState != Application.isPlaying)
            {
                if (Application.isPlaying)
                {
                    script.emulationResponse += OnResponseReceived;
                }
                else
                {
                    script.emulationResponse -= OnResponseReceived;
                }
                lastPlayModeState = Application.isPlaying;
            }
        }

        private void ShowChatWindow()
        {
            GUILayout.Label(history);

            message = EditorGUILayout.TextField("Message", message);

            GUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Send Message"))
                {
                    script.Received(message);
                    //history += "Sent: " + message + "\n";
                }
                if (GUILayout.Button("Clear History"))
                {
                    history = "";
                }
            }
            GUILayout.EndHorizontal();

        }

        private void ShowModeState(BaseSerialCommunication script)
        {
            if (script.emulationMode)
            {
                GUI.color = Color.green;
                GUILayout.Label("Emulation Mode Active");
                GUI.color = Color.white;

                if (GUILayout.Button("Deactivate Emulation Mode"))
                {
                    script.emulationMode = false;
                    EditorUtility.SetDirty(target);

                }
            }
            else
            {
                if (GUILayout.Button("Activate Serial Emulation Mode"))
                {
                    script.emulationMode = true;
                    EditorUtility.SetDirty(target);
                }
            }
        }

    }
}
