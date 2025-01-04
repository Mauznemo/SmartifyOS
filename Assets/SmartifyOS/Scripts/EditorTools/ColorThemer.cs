#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    [AddComponentMenu("SmartifyOS/Color Themer", 0)]
    public class ColorThemer : MonoBehaviour
    {
        public string styleName;
        [SerializeField] private UnityEventColor updateColor = new UnityEventColor();

        public string GetStyleName() => styleName;

        public void UpdateColor(Color color)
        {
            updateColor.Invoke(color);
        }

        public UnityEventColor OnUpdateColor
        {
            get => updateColor;
            set => updateColor = value;
        }
    }

    [Serializable]
    public class UnityEventColor : UnityEvent<Color> { };
}
#endif