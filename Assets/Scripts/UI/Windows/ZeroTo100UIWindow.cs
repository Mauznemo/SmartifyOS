using System;
using System.Collections;
using System.Diagnostics;
using SmartifyOS.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ZeroTo100UIWindow : BaseUIWindow
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject infoScreen;
    [SerializeField] private GameObject resultScreen;

    [SerializeField] private TMP_Text startText;

    [SerializeField] private TMP_Text infoSpeedText;
    [SerializeField] private TMP_Text infoTimeText;

    [SerializeField] private TMP_Text resultTimeText;

    private Stopwatch stopwatch = new Stopwatch();

    private void Start()
    {
        Init();
    }

    protected override void OnShow()
    {
        startScreen.SetActive(true);
        infoScreen.SetActive(false);
        resultScreen.SetActive(false);

        if (!LiveDataController.hasGpsSignal)
        {
            startText.text = "No GPS signal!";
            return;
        }

        if (Mathf.Abs(LiveDataController.steeringWheelAngle) > 50f)
        {
            startText.text = "Steering angle too high!";
            return;
        }

        startText.text = "Fully stop the vehicle!";

        StartCoroutine(WaitForStop());
    }

    private IEnumerator WaitForStop()
    {
        yield return new WaitUntil(() => LiveDataController.speedKmh < 5f);

        startText.text = "Now start driving for the timer to start!";

        yield return new WaitUntil(() => LiveDataController.speedKmh > 5f);

        startScreen.SetActive(false);
        infoScreen.SetActive(true);

        stopwatch.Reset();
        stopwatch.Start();

        yield return new WaitUntil(() => LiveDataController.speedKmh >= 100f);

        stopwatch.Stop();

        infoScreen.SetActive(false);
        resultScreen.SetActive(true);

        resultTimeText.text = FormatElapsedTime(stopwatch.Elapsed);
    }

    private void Update()
    {
        if (stopwatch.IsRunning)
        {
            infoSpeedText.text = LiveDataController.speedKmh.ToString("f0");

            infoTimeText.text = $"{FormatElapsedTime(stopwatch.Elapsed)} sec";
        }
    }

    private string FormatElapsedTime(TimeSpan timeSpan)
    {
        return string.Format("{0:D2}.{1:D3}", timeSpan.Seconds, timeSpan.Milliseconds);
    }
}
