using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Utilities
{
    /// <summary>Check if Pointer is over UI</summary>
    /// <returns><see cref="bool"/> <see langword="true"/> if Pointer is over UI</returns>
    public static bool IsOverUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        return results.Count > 0;
    }
}
