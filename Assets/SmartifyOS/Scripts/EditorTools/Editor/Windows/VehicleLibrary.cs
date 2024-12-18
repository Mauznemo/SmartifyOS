﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.Buffers.Text;
using Newtonsoft.Json.Linq;
using System;

namespace SmartifyOS.Editor
{
    public class VehicleLibrary : EditorWindow
    {
        [MenuItem("SmartifyOS/Vehicle Library", false, 100)]
        public static void ShowWindow()
        {
            var window = GetWindow<VehicleLibrary>("Vehicle Library");
            //window.SetSize(900, 500);
            window.Show();
        }


        private Texture uploadIcon;
        private Texture searchIcon;
        private Texture likeIcon;
        private Texture likedIcon;
        private Texture downloadIcon;
        private Texture importIcon;

        private Vehicle[] vehicleArray;

        private string infoText;

        private void OnEnable()
        {
            uploadIcon = EditorUtils.GetIcon("UploadIcon");
            searchIcon = EditorUtils.GetIcon("SearchIcon");
            likeIcon = EditorUtils.GetIcon("LikeIcon");
            likedIcon = EditorUtils.GetIcon("LikedIcon");
            downloadIcon = EditorUtils.GetIcon("DownloadIcon");
            importIcon = EditorUtils.GetIcon("importIcon");

            LoadVehicles();
        }

        private void LoadVehicles(string search = "")
        {
            infoText = "Loading...";
            string url = "http://localhost:5173/api/get-vehicles";
            Networking.GET(url, search, (success, message) =>
            {
                if (success)
                {
                    Debug.Log(message);
                    dynamic dataArray = JsonConvert.DeserializeObject<dynamic>(message);

                    JObject dataObject = dataArray as JObject;

                    if (dataObject != null && dataObject.ContainsKey("error"))
                    {
                        string error = dataObject["error"].ToString();
                        vehicleArray = null;
                        switch (error)
                        {
                            case "no-vehicles-found":
                                infoText = "No Vehicles Found";
                                return;
                            case "brand-not-found":
                                infoText = "Please enter a brand or nothing";
                                return;
                        }
                        return;
                    }
                    vehicleArray = ConvertData(dataArray);
                    Repaint();
                }
                else
                {
                    Debug.LogError(message);
                    infoText = message;
                    Repaint();
                }
            });
        }

        private Vehicle[] ConvertData(dynamic data)
        {
            List<Vehicle> list = new List<Vehicle>();

            foreach (dynamic item in data)
            {
                string base64 = item.vehicle.image;
                int base64Index = base64.IndexOf("base64,") + 7;
                string base64Data = base64.Substring(base64Index);

                byte[] imageBytes = System.Convert.FromBase64String(base64Data);

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(imageBytes);

                list.Add(new Vehicle
                {
                    brand = item.vehicle.brand,
                    model = item.vehicle.model,
                    variant = item.vehicle.variant,
                    image = texture,
                });
            }

            return list.ToArray();
        }

        public class Vehicle
        {
            public string brand;
            public string model;
            public string variant;
            public Texture2D image;
        }

        private void OnGUI()
        {
            Header();

            SearchBar();

            GUILayout.Space(10);

            VehicleList();
        }

        private void Header()
        {
            GUIStyle headerStyle = new GUIStyle(GUI.skin.box);
            headerStyle.margin = new RectOffset(0, 0, 0, 5);


            GUI.color = Color.cyan;
            GUILayout.BeginVertical(headerStyle);
            GUI.color = Color.white;
            GUILayout.BeginHorizontal();

            GUIStyle textStyle = new GUIStyle(GUI.skin.label);
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.fontSize = 20;


            EditorGUILayout.LabelField("Vehicle Library", textStyle, GUILayout.Width(155), GUILayout.Height(40));

            GUIStyle tabButtonStyle = new GUIStyle(GUI.skin.button);
            tabButtonStyle.margin = new RectOffset(5, 5, 13, 10);
            GUI.color = Color.gray;
            if (GUILayout.Button("Browse", tabButtonStyle, GUILayout.Width(100), GUILayout.Height(20)))
            {

            }
            GUI.color = Color.white;
            if (GUILayout.Button("Downloaded", tabButtonStyle, GUILayout.Width(100), GUILayout.Height(20)))
            {

            }

            GUILayout.FlexibleSpace();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 16;
            buttonStyle.margin = new RectOffset(5, 5, 4, 5);

            GUIContent buttonContent = new GUIContent("Upload", uploadIcon);
            buttonStyle.imagePosition = ImagePosition.ImageLeft;
            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.Width(100), GUILayout.Height(35)))
            {
                UploadVehicle.ShowWindow();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private string searchBrandText = "";
        private string searchModelText = "";
        private string searchVariantText = "";
        private void SearchBar()
        {
            EditorGUILayout.BeginHorizontal();

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontStyle = FontStyle.Bold;
            buttonStyle.fontSize = 16;

            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontStyle = FontStyle.Bold;
            textFieldStyle.fontSize = 16;
            textFieldStyle.padding = new RectOffset(10, 5, 10, 5);

            searchBrandText = TextField(searchBrandText, "Enter Brand...");
            searchModelText = TextField(searchModelText, "Enter Model...");
            searchVariantText = TextField(searchVariantText, "Enter Variant...");

            GUIContent buttonContent = new GUIContent("   Search   ", searchIcon);

            if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxWidth(200), GUILayout.Height(40)))
            {
                LoadVehicles($"brand={searchBrandText}&model={searchModelText}&variant={searchVariantText}&page=0");
            }

            EditorGUILayout.EndHorizontal();
        }

        string TextField(string text, string placeholder)
        {
            return TextInput(text, placeholder);
        }

        string TextArea(string text, string placeholder)
        {
            return TextInput(text, placeholder, area: true);
        }

        private string TextInput(string text, string placeholder, bool area = false)
        {
            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.fontStyle = FontStyle.Bold;
            textFieldStyle.fontSize = 16;
            textFieldStyle.padding = new RectOffset(10, 5, 10, 5);

            var newText = area ? EditorGUILayout.TextArea(text, textFieldStyle, GUILayout.Height(40)) : EditorGUILayout.TextField(text, textFieldStyle, GUILayout.Height(40));
            if (string.IsNullOrEmpty(text.Trim()))
            {
                const int textMargin = 10;
                var guiColor = GUI.color;
                GUI.color = Color.grey;
                var textRect = GUILayoutUtility.GetLastRect();
                var position = new Rect(textRect.x + textMargin, textRect.y, textRect.width, textRect.height);
                EditorGUI.LabelField(position, placeholder);
                GUI.color = guiColor;
            }
            return newText;
        }

        private Vector2 scrollPosition;
        private void VehicleList()
        {
            Texture image = AssetDatabase.LoadAssetAtPath<Texture>(EditorUtils.GetGraphicsPath() + "SmartifyOS-VehicleLibrary.png");

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (vehicleArray != null && vehicleArray.Length > 0)
            {
                foreach (var vehicle in vehicleArray)
                {


                    VehicleEntry(vehicle.image, $"{vehicle.brand} {vehicle.model} {vehicle.variant}");
                }
            }
            else
            {
                GUIStyle textStyle = new GUIStyle(GUI.skin.label);
                textStyle.fontStyle = FontStyle.Bold;
                textStyle.fontSize = 20;
                textStyle.margin = new RectOffset(10, 10, 10, 10);
                textStyle.alignment = TextAnchor.MiddleCenter;
                GUILayout.FlexibleSpace();

                EditorGUILayout.LabelField("Vehicle Library is not implemented yet", textStyle, GUILayout.Height(40));
                EditorGUILayout.LabelField(infoText, textStyle, GUILayout.Height(40));

                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndScrollView();
        }

        private void VehicleEntry(Texture image, string name)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(200));

            EditorGUILayout.BeginHorizontal();

            ImageTile();

            SideTile();

            //GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.EndVertical();

            void ImageTile()
            {
                EditorGUILayout.BeginVertical();

                if (image != null)
                {
                    var imageStyle = new GUIStyle(EditorStyles.label);
                    imageStyle.padding = new RectOffset(0, 10, 0, 0);

                    GUILayout.Label(image, GUILayout.Width(300), GUILayout.Height(200));
                }

                EditorGUILayout.BeginHorizontal(GUILayout.Width(300));
                GUILayout.Label("Example model from sketchfab.com");
                /*
                GUIContent buttonContent = new GUIContent("   12K  ", likeIcon);

                if (GUILayout.Button(buttonContent, GUILayout.MaxWidth(100), GUILayout.Height(20)))
                {

                }*/
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            void SideTile()
            {
                var headingStyle = new GUIStyle(EditorStyles.label);
                headingStyle.fontStyle = FontStyle.Bold;
                headingStyle.fontSize = 30;
                headingStyle.padding = new RectOffset(10, 0, 0, -20);

                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField(name, headingStyle);

                GUILayout.Space(164);

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = 16;

                GUIContent buttonContent = new GUIContent("   Download  ", downloadIcon);

                if (GUILayout.Button(buttonContent, buttonStyle, GUILayout.MaxWidth(200), GUILayout.Height(40)))
                {

                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }
    }
}
