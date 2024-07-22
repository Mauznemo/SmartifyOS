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


    private bool isMain = true;


    public void SetFirstText(float value, string extension, string format = "0.00")
    {
        if (isMain)
            firstText.text = value.ToString(format) + " " + extension;
        else
            secondText.text = value.ToString(format);
    }

    public void SetSecondText(float value, string extension, string format = "0.00")
    {
        if (isMain)
            firstText.text = value.ToString(format) + " " + extension;
        else
            secondText.text = value.ToString(format);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isMain = !isMain;
        string textOne = firstText.text;
        string textTwo = secondText.text;
        firstText.text = textTwo;
        secondText.text = textOne;
    }
}
