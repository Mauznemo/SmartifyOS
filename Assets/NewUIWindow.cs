public class NewUIWindow : BaseUIWindow
{
    private void Start()
    {
        Init();
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        //Add all windows that should hide this window when they open
        //if (window.IsWindowOfType(typeof(UIWindow1), typeof(UIWindow2)))
        //{
        //    Hide(true);
        //}
    }

    protected override void HandleWindowClosed(BaseUIWindow window)
    {
        if(!wasOpen) { return; }

        //Add all windows that should trigger this window to reopen when they close
        //if (window.IsWindowOfType(typeof(UIWindow1), typeof(UIWindow2)))
        //{
        //    Show();
        //}
    }
}
