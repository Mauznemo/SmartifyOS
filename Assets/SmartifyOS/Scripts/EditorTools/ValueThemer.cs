#if UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    [AddComponentMenu("SmartifyOS/Value Themer", 0)]
    public class ValueThemer : BaseThemeable<float>
    {
        public float multiplier = 1f;
        [SerializeField] private UnityEventFloat updateValue = new UnityEventFloat();

        public override void UpdateValue(float value)
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
#endif