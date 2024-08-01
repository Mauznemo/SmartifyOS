using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsEntry : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text infoText;

    public void Init(string name, Sprite icon, string info)
    {
        infoText.text = info;
        nameText.text = name;
        iconImage.sprite = icon;
    }
}
