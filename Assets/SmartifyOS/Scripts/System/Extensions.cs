using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
