using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RealtimeInfoGraphs : MonoBehaviour
{
    public bool updateGraphs;

    [SerializeField] private Transform toggleParent;
    [SerializeField] private Toggle togglePrefab;

    [SerializeField] private UIGraphLineRenderer lineRendererKmh;
    [SerializeField] private UIGraphLineRenderer lineRendererRPM;

    private bool kmhActive = true;
    private bool rpmActive = true;

    private int xPosition;

    private void Awake()
    {
        CreateToggle("km/h", lineRendererKmh, (value) => { kmhActive = value; });
        CreateToggle("rpm", lineRendererRPM, (value) => { rpmActive = value; });
    }

    private void Start()
    {
        InvokeRepeating(nameof(UpdateGraphs), 0.1f, 0.1f);
    }

    private void UpdateGraphs()
    {
        if (!updateGraphs)
        {
            return;
        }

        if (xPosition > 15_000)
        {
            if (kmhActive)
                lineRendererKmh.ShiftPoints(new Vector2Int(xPosition, Mathf.RoundToInt(LiveDataController.speedKmh * 10000) / 75));
            if (rpmActive)
                lineRendererRPM.ShiftPoints(new Vector2Int(xPosition, Mathf.RoundToInt(LiveDataController.rpm * 10000) / 3125));
        }
        else
        {
            xPosition += 100;
            if (kmhActive)
                lineRendererKmh.AddPoint(new Vector2Int(xPosition, Mathf.RoundToInt(LiveDataController.speedKmh * 10000) / 75));
            if (rpmActive)
                lineRendererRPM.AddPoint(new Vector2Int(xPosition, Mathf.RoundToInt(LiveDataController.rpm * 10000) / 3125));
        }

    }

    private void CreateToggle(string text, UIGraphLineRenderer line, Action<bool> OnValueChanged)
    {
        Toggle toggle = Instantiate(togglePrefab, toggleParent);
        toggle.gameObject.SetActive(true);
        TMP_Text toggleText = toggle.GetComponentInChildren<TMP_Text>();
        toggleText.text = text;

        toggle.onValueChanged.AddListener((value) =>
        {
            OnValueChanged(value);
            line.gameObject.SetActive(value);
        });

    }
}
