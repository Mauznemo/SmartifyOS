using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SmartifyOS.UI;

public static class Extensions
{
    /*public static bool IsWindowOfType<T>(this BaseUIWindow window) where T : BaseUIWindow
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        return window is T;
    }*/

    public static bool IsNullOrEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    public static void SetIfNotNull(this TMP_Text textComponent, string value)
    {
        if (textComponent != null)
        {
            textComponent.text = value;
        }
    }

    public static void SetColorIfNotNull(this TMP_Text textComponent, Color value)
    {
        if (textComponent != null)
        {
            textComponent.color = value;
        }
    }

    public static void SetColorIfNotNull(this Image image, Color value)
    {
        if (image != null)
        {
            image.color = value;
        }
    }

    public static void SetSpriteIfNotNull(this Image image, Sprite sprite)
    {
        if (image != null && sprite != null)
        {
            image.sprite = sprite;
        }
    }

    public static bool IsWindowOfType(this BaseUIWindow window, params Type[] types)
    {
        if (window == null)
        {
            throw new ArgumentNullException(nameof(window));
        }

        if (types == null || types.Length == 0)
        {
            throw new ArgumentNullException(nameof(types));
        }

        foreach (var type in types)
        {
            if (!type.IsSubclassOf(typeof(BaseUIWindow)) && type != typeof(BaseUIWindow))
            {
                throw new ArgumentException($"{type} is not a subtype of BaseBaseUIWindow");
            }

            if (type.IsInstanceOfType(window))
            {
                return true;
            }
        }

        return false;
    }
}
