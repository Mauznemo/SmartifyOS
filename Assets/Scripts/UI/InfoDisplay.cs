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


    public void SetFirstText(string text)
    {
        if (isMain)
            firstText.text = text;
        else
            secondText.text = text;
    }

    public void SetSecondText(string text)
    {
        if (!isMain)
            firstText.text = text;
        else
            secondText.text = text;
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
