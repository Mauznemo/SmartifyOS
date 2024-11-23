using System.Collections;
using SmartifyOS.UI;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIWindow : BaseUIWindow
{
    [SerializeField] private ScrollRect scrollRect;

    private void Start()
    {
        Init();
    }

    protected override void OnShow()
    {
        StartCoroutine(SetScrollPositionAfterLayout());
    }

    IEnumerator SetScrollPositionAfterLayout()
    {
        yield return null;
        scrollRect.verticalNormalizedPosition = 1f;
    }
}
