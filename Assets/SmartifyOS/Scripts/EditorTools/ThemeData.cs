#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace SmartifyOS.Editor.Theming
{
    [CreateAssetMenu(fileName = "ThemeData", menuName = "SmartifyOS/ThemeData")]
    public class ThemeData : ScriptableObject
    {
        public static ThemeData instance;

        public ColorStyles colorStyles = new ColorStyles();
        public ValueStyles valueStyles = new ValueStyles();

        public static ThemeData GetThemeData()
        {
            if (instance == null)
                instance = AssetDatabase.LoadAssetAtPath<ThemeData>("Assets/ScriptableObjects/ThemeData.asset");

            return instance;
        }

        public void SaveAsset()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public Color GetColor(string styleName)
        {
            if (string.IsNullOrEmpty(styleName))
                return Color.white;

            if (colorStyles.styles.ContainsKey(styleName))
            {
                return colorStyles.styles[styleName].color;
            }
            return Color.white;
        }

        public float GetValue(string styleName)
        {
            if (string.IsNullOrEmpty(styleName))
                return 0f;

            if (valueStyles.styles.ContainsKey(styleName))
            {
                return valueStyles.styles[styleName].value;
            }
            return 0f;
        }
    }

    [System.Serializable]
    public class ColorStyles
    {
        public SerializedDictionary<string, ColorStyle> styles = new SerializedDictionary<string, ColorStyle>();

        public string GetFirstStyleName() => styles.Keys.ToArray()[0];

        public void AddStyle(string styleName, Color color, bool canBeRemoved = true)
        {
            if (styles.ContainsKey(styleName))
            {
                Debug.LogError("Color style already exists: " + styleName);
            }
            else
            {
                styles.Add(styleName, new ColorStyle() { color = color, canBeRemoved = canBeRemoved });
            }
        }

        public void RemoveStyle(string styleName)
        {
            if (styles.ContainsKey(styleName))
            {
                styles.Remove(styleName);
            }
        }
    }

    [System.Serializable]
    public class ValueStyles
    {
        public SerializedDictionary<string, ValueStyle> styles = new SerializedDictionary<string, ValueStyle>();

        public string GetFirstStyleName() => styles.Keys.ToArray()[0];

        public void AddStyle(string styleName, float value, bool canBeRemoved = true)
        {
            if (styles.ContainsKey(styleName))
            {
                Debug.LogError("Value style already exists: " + styleName);
            }
            else
            {
                styles.Add(styleName, new ValueStyle() { value = value, canBeRemoved = canBeRemoved });
            }
        }

        public void RemoveStyle(string styleName)
        {
            if (styles.ContainsKey(styleName))
            {
                styles.Remove(styleName);
            }
        }
    }

    [System.Serializable]
    public class ColorStyle
    {
        public Color color;
        public bool canBeRemoved = true;
    }

    [System.Serializable]
    public class ValueStyle
    {
        public float value;
        public bool canBeRemoved = true;
    }
}
#endif



