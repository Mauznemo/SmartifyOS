using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseUIWindow : MonoBehaviour
{
    protected bool wasOpen;

    protected void Init()
    {
        UIManager.OnWindowOpened += UIManager_OnWindowOpened;
        UIManager.OnWindowClosed += UIManager_OnWindowClosed;

        transform.localScale = Vector3.zero;
    }

    private void UIManager_OnWindowClosed(BaseUIWindow obj)
    {
        if(obj == this)
            return;

        HandleWindowClosed(obj);
    }

    private void UIManager_OnWindowOpened(BaseUIWindow obj)
    {
        if (obj == this)
            return;

        HandleWindowOpened(obj);
    }

    protected virtual void HandleWindowOpened(BaseUIWindow window) { }

    protected virtual void HandleWindowClosed(BaseUIWindow window) { }


    public void Show()
    {
        UIManager.Instance.AddOpenWindow(this);

        wasOpen = true;

        transform.localScale = Vector3.one;
    }

    public void Hide(bool internalUpdate = false)
    {
        UIManager.Instance.RemoveOpenWindow(this);

        if (!internalUpdate)
            wasOpen = false;

        transform.localScale = Vector3.zero;
    }
}
