using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Welcome : EditorWindow
    {
        private Texture image;
        private int pageIndex = 0;

        private string licensePath;
        private string licenseContent = "";

        private string buttonText = "Next";

        [MenuItem("SmartifyOS/Welcome")]
        public static void ShowWindow()
        {
            var window = CreateInstance<Welcome>();
            window.titleContent = new GUIContent("Welcome to SmartifyOS");
            window.SetSize(900, 500);
            window.ShowUtility();
        }

        private void OnEnable()
        {
            image = AssetDatabase.LoadAssetAtPath<Texture>(EditorUtils.GetGraphicsPath() + "Welcome/SmartifyOS-welcome.png");
            licensePath = EditorUtils.GetSmartifyOSPath() + "../../LICENSE";
            licenseContent = System.IO.File.ReadAllText(licensePath);

            EditorSceneManager.OpenScene("Assets/Scenes/MainScene.unity");

            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            foreach (var window in windows)
            {
                // Check the window type or title
                if (window.titleContent.text == "Unity Editor Update Check")
                {
                    window.Close();
                    Debug.Log("Closed Unity Editor Update Checker window.");
                }
            }
        }

        private void OnDestroy()
        {
            if (pageIndex == 4)
                return;

            var newWin = CreateInstance<Welcome>(); ;
            newWin.titleContent = new GUIContent("Welcome to SmartifyOS");
            newWin.pageIndex = pageIndex;
            newWin.SetSize(900, 500);
            newWin.ShowUtility();
        }

        private void OnGUI()
        {
            var headingStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 40,
                padding = new RectOffset(20, 0, -30, 0)
            };

            GUILayout.Space(30);
            EditorGUILayout.LabelField("Welcome to SmartifyOS", headingStyle);

            switch (pageIndex)
            {
                case 0:
                    ImagePage();
                    break;
                case 1:
                    Disclaimer();
                    break;
                case 2:
                    License();
                    break;
                case 3:
                    Donation();
                    break;
                case 4:
                    EditorPrefs.SetInt("SmartifyOSWelcome", 1);
                    Close();
                    break;
            }

            EditorGUILayout.BeginHorizontal();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16
            };

            GUILayout.FlexibleSpace();

            if (pageIndex == 3)
            {
                if (GUILayout.Button(" Start with Blank Project", buttonStyle, GUILayout.Width(220), GUILayout.Height(40)))
                {
                    pageIndex++;
                    BlankProject.ShowWindow();
                }
            }

            if (GUILayout.Button(buttonText, buttonStyle, GUILayout.Width(200), GUILayout.Height(40)))
            {
                pageIndex++;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ImagePage()
        {
            var imageStyle = new GUIStyle(EditorStyles.label)
            {
                padding = new RectOffset(0, 0, -25, -24)
            };

            GUILayout.Label(image, imageStyle);
        }

        private Vector2 scrollPosition;

        private void Disclaimer()
        {
            GUILayout.Space(20);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            var headingStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 30,
                padding = new RectOffset(50, 0, -5, -5),
            };

            var subHeadingStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 20,
                padding = new RectOffset(50, 0, -5, -5),
            };

            var textStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                wordWrap = true,
                padding = new RectOffset(50, 50, 0, 0)
            };

            EditorGUILayout.LabelField("Disclaimer", headingStyle);

            GUILayout.Space(20);

            EditorGUILayout.LabelField("This software is currently in the development phase and is intended for developers. It is not yet suitable for general use in vehicles yet.", textStyle);

            GUILayout.Space(20);

            EditorGUILayout.LabelField("Important Considerations:", subHeadingStyle);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("1. Experimental Software: This version is experimental and may contain bugs, incomplete features, and untested code.", textStyle);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("2. Developer Expertise Required: Installation and use of this software require coding skills and a thorough understanding of the system. If you are not a developer or do not have experience in software development (mainly in C# Unity and shell scripting), I strongly advise against using this in your vehicle at the current time.", textStyle);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("3. Risk of Malfunction: The software may cause unexpected behavior, system crashes, or other issues that could affect the performance and safety of your vehicle.", textStyle);

            GUILayout.Space(10);

            EditorGUILayout.LabelField("4. Contribution Encouraged: I welcome contributions from developers to help improve the software. If you are interested in contributing, please refer to the contribution guidelines of the repository.", textStyle);

            GUILayout.Space(20);

            EditorGUILayout.LabelField("By using this software, you acknowledge and agree that you are doing so at your own risk. The developers are not liable for any damage or harm resulting from the use of this software. Always ensure that you have adequate safety measures and backups in place before testing the software in any real-world scenario.", textStyle);

            GUILayout.Space(20);

            EditorGUILayout.LabelField("You can enter the waitlist at smartify-os.com to get an email when the software is ready for general use.", textStyle);

            GUILayout.EndScrollView();
        }


        private Vector2 scrollPosition2;

        private void License()
        {
            GUILayout.Space(20);
            buttonText = "Agree and Continue";
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);
            var textStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                wordWrap = true,
                padding = new RectOffset(50, 50, 0, 0)
            };
            //GUI.enabled = false;
            //EditorGUILayout.TextArea(licenseContent, textStyle);
            //GUI.enabled = true;
            EditorGUILayout.LabelField(licenseContent, textStyle);
            GUILayout.EndScrollView();
        }

        private void Donation()
        {
            buttonText = "Finish";
            GUILayout.FlexibleSpace();

            var headingStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 30,
                padding = new RectOffset(50, 0, -5, -5),
            };

            var textStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 14,
                wordWrap = true,
                padding = new RectOffset(50, 50, 0, 0)
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontStyle = FontStyle.Bold,
                fontSize = 16,
                margin = new RectOffset(50, 50, 0, 0),
            };

            EditorGUILayout.LabelField("Support the project!", headingStyle);

            GUILayout.Space(20);

            EditorGUILayout.LabelField("Become a GitHub Sponsor to support the project", textStyle);

            GUILayout.Space(20);

            if (GUILayout.Button("Become a GitHub Sponsor ❤️", buttonStyle, GUILayout.Height(40)))
            {
                Application.OpenURL("https://github.com/sponsors/Mauznemo");
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Donate on PayPal ❤️", buttonStyle, GUILayout.Height(40)))
            {
                Application.OpenURL("https://www.paypal.com/donate/?hosted_button_id=BSPF2HUZRP7AN");
            }

            GUILayout.FlexibleSpace();
        }
    }
}


