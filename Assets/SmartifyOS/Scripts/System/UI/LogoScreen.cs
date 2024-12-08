using UnityEngine;
using UnityEngine.UI;

public class LogoScreen : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ShowScreen()
    {
        LeanTween.alphaCanvas(canvasGroup, 1, 0.5f).setEaseInOutSine();
    }

    public void HideScreen()
    {
        LeanTween.alphaCanvas(canvasGroup, 0, 0.5f).setEaseInOutSine();
    }

    public void ShowScreenFor(float time, bool fadeIn = true)
    {
        if (fadeIn)
            ShowScreen();
        else
            canvasGroup.alpha = 1;

        Invoke(nameof(HideScreen), time);
    }
}
