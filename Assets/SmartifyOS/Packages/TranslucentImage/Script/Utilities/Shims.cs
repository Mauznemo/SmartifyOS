using System.Runtime.CompilerServices;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage
{
public static class Shims
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T FindObjectOfType<T>(bool includeInactive = false, bool sorted = true) where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        if (sorted)
            return Object.FindFirstObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
        else
            return Object.FindAnyObjectByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);
#elif UNITY_2020_1_OR_NEWER
        return Object.FindObjectOfType<T>(includeInactive);
#else
        return Object.FindObjectOfType<T>();
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T[] FindObjectsOfType<T>(bool includeInactive = false) where T : Object
    {
#if UNITY_2023_1_OR_NEWER
        return Object.FindObjectsByType<T>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude,
                                           FindObjectsSortMode.None);
#elif UNITY_2020_1_OR_NEWER
        return Object.FindObjectsOfType<T>(includeInactive);
#else
        return Object.FindObjectsOfType<T>();
#endif
    }
}
}
