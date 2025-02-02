using UnityEngine;
using UnityEngine.Events;

namespace SmartifyOS.Editor.Theming
{
    public abstract class BaseThemeable<T> : MonoBehaviour
    {
        public string styleName;

        public string GetStyleName() => styleName;

        public abstract void UpdateValue(T value);
    }
}
