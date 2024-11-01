using System.Collections;
using System.Collections.Generic;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text firstText;
    [SerializeField] private TMP_Text secondText;

    private float firstValue;
    private float secondValue;
    private string firstExtension = "km/h";
    private string secondExtension = " RPM";
    private string firstFormat = "0";
    private string secondFormat = "0000";

    private bool isMain = true;


    public void SetFirstText(float value, string extension, string format = "0.00")
    {
        firstValue = value;
        firstExtension = extension;
        firstFormat = format;

        if (isMain)
            firstText.text = value.ToString(format);
        else
            secondText.text = value.ToString(format) + " " + extension;
    }

    public void SetSecondText(float value, string extension, string format = "0.00")
    {
        secondValue = value;
        secondExtension = extension;
        secondFormat = format;

        if (!isMain)
            firstText.text = value.ToString(format);
        else
            secondText.text = value.ToString(format) + " " + extension;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isMain = !isMain;
        SetFirstText(firstValue, firstExtension, firstFormat);
        SetSecondText(secondValue, secondExtension, secondFormat);
    }
}
