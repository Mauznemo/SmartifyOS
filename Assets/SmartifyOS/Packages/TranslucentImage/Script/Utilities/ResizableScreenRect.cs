// Due to the use of asmdef in the ../Editor folder, this class cannot be put there,
// as then it cannot be referenced outside of the assembly
// The Blur region GUI requires the use of OnGUI for interactivity,
// so it cannot be completely done within the custom Editor either

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace SmartifyOS.UI.TranslucentImage
{
public static class ResizableScreenRect
{
    const float BORDER_THICKNESS = 2;
    const float DRAG_EXTEND      = 16;

    delegate void DragHandlerDelegate(Vector2 delta, ref Rect rect);

    static DragHandlerDelegate currentDragHandler;

    static void DrawSquareFromCenter(Vector2 postion, float extent)
    {
        var v = Vector2.one * extent;
        GUI.DrawTexture(new Rect(postion - v, v * 2), Texture2D.whiteTexture);
    }

    public static Rect Draw(Rect normalizedScreenRect)
    {
        var guiRect = normalizedScreenRect;
        guiRect.y      =  1 - guiRect.y - guiRect.height;
        guiRect.x      *= Screen.width;
        guiRect.width  *= Screen.width;
        guiRect.y      *= Screen.height;
        guiRect.height *= Screen.height;

        var borderThickness = BORDER_THICKNESS * EditorGUIUtility.pixelsPerPoint;
        GUI.DrawTexture(new Rect(guiRect.x - borderThickness,
                                 guiRect.y - borderThickness,
                                 borderThickness,
                                 guiRect.height + borderThickness * 2),
                        Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(guiRect.x,
                                 guiRect.y - borderThickness,
                                 guiRect.width + 1,
                                 borderThickness),
                        Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(guiRect.xMax,
                                 guiRect.y - borderThickness,
                                 borderThickness,
                                 guiRect.height + borderThickness * 2),
                        Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(guiRect.x,
                                 guiRect.yMax,
                                 guiRect.width + 1,
                                 borderThickness),
                        Texture2D.whiteTexture);
        var boxExtend = borderThickness * 2;
        DrawSquareFromCenter(guiRect.min,                             boxExtend);
        DrawSquareFromCenter(guiRect.max,                             boxExtend);
        DrawSquareFromCenter(new Vector2(guiRect.xMax, guiRect.yMin), boxExtend);
        DrawSquareFromCenter(new Vector2(guiRect.xMin, guiRect.yMax), boxExtend);

        return HandleEvent(guiRect);
    }

    static Rect HandleEvent(Rect guiRect)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            currentDragHandler = ChooseDragHandler(guiRect, Event.current.mousePosition);
        }
        else if (Event.current.type == EventType.MouseUp)
        {
            currentDragHandler = null;
        }
        else if (Event.current.type == EventType.MouseDrag)
        {
            currentDragHandler?.Invoke(Event.current.delta, ref guiRect);
        }

        var result = guiRect;

        result.x      /= Screen.width;
        result.y      /= Screen.height;
        result.width  /= Screen.width;
        result.height /= Screen.height;
        result.y      =  1 - result.y - result.height;

        result.xMin   = Mathf.Max(0, result.xMin);
        result.yMin   = Mathf.Max(0, result.yMin);
        result.width  = Mathf.Min(1, result.width);
        result.height = Mathf.Min(1, result.height);

        return result;
    }

    static DragHandlerDelegate ChooseDragHandler(Rect rect, Vector2 pointer)
    {
        float extend = DRAG_EXTEND * EditorGUIUtility.pixelsPerPoint;

        bool PointerXNear(float point) => Mathf.Abs(point - pointer.x) <= extend;
        bool PointerYNear(float point) => Mathf.Abs(point - pointer.y) <= extend;

        if (PointerXNear(rect.xMin))
        {
            if (PointerYNear(rect.yMin)) return DRAG_HANDLER_TOP_LEFT;
            if (PointerYNear(rect.yMax)) return DRAG_HANDLER_BOTTOM_LEFT;
            return DRAG_HANDLER_LEFT;
        }

        if (PointerXNear(rect.xMax))
        {
            if (PointerYNear(rect.yMin)) return DRAG_HANDLER_TOP_RIGHT;
            if (PointerYNear(rect.yMax)) return DRAG_HANDLER_BOTTOM_RIGHT;
            return DRAG_HANDLER_RIGHT;
        }

        if (PointerYNear(rect.yMin)) return DRAG_HANDLER_TOP;
        if (PointerYNear(rect.yMax)) return DRAG_HANDLER_BOTTOM;
        return DRAG_HANDLER_CENTER;
    }

    static readonly DragHandlerDelegate DRAG_HANDLER_CENTER =
        (Vector2 delta, ref Rect rect) => { rect.position += delta; };

    static readonly DragHandlerDelegate DRAG_HANDLER_LEFT =
        (Vector2 delta, ref Rect rect) => { rect.xMin += delta.x; };

    static readonly DragHandlerDelegate DRAG_HANDLER_TOP =
        (Vector2 delta, ref Rect rect) => { rect.yMin += delta.y; };

    static readonly DragHandlerDelegate DRAG_HANDLER_BOTTOM =
        (Vector2 delta, ref Rect rect) => { rect.yMax += delta.y; };

    static readonly DragHandlerDelegate DRAG_HANDLER_RIGHT =
        (Vector2 delta, ref Rect rect) => { rect.xMax += delta.x; };

    static readonly DragHandlerDelegate DRAG_HANDLER_TOP_LEFT =
        (Vector2 delta, ref Rect rect) =>
        {
            rect.xMin += delta.x;
            rect.yMin += delta.y;
        };

    static readonly DragHandlerDelegate DRAG_HANDLER_TOP_RIGHT =
        (Vector2 delta, ref Rect rect) =>
        {
            rect.xMax += delta.x;
            rect.yMin += delta.y;
        };

    static readonly DragHandlerDelegate DRAG_HANDLER_BOTTOM_RIGHT =
        (Vector2 delta, ref Rect rect) =>
        {
            rect.xMax += delta.x;
            rect.yMax += delta.y;
        };

    static readonly DragHandlerDelegate DRAG_HANDLER_BOTTOM_LEFT =
        (Vector2 delta, ref Rect rect) =>
        {
            rect.xMin += delta.x;
            rect.yMax += delta.y;
        };
}
}
#endif
