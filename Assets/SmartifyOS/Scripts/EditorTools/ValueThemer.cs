using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    [AddComponentMenu("SmartifyOS/Value Themer", 0)]
    public class ValueThemer : MonoBehaviour
    {
        public string styleName;
        public float multiplier = 1f;
        [SerializeField] private UnityEventFloat updateValue = new UnityEventFloat();

        public string GetStyleName() => styleName;

        public void UpdateValue(float value)
        {
            updateValue.Invoke(value * multiplier);
        }

        public UnityEventFloat OnUpdateFloat
        {
            get => updateValue;
            set => updateValue = value;
        }
    }

    [Serializable]
    public class UnityEventFloat : UnityEvent<float> { };
}