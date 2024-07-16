using System;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteriorUIWindow : BaseUIWindow
{
    public event Action<Color> OnColorChanged;

    public Color color;

    [SerializeField] private GameObject selectionScreen;
    [SerializeField] private GameObject colorScreen;

    [SerializeField] private TMP_Text selectedLightText;

    [SerializeField] private IconButton leftLedSelectButton;
    [SerializeField] private IconButton rightLedSelectButton;
    [SerializeField] private IconButton lampSelectButton;


    [SerializeField] private RawImage hueImage;
    [SerializeField] private Image previewImage;

    [SerializeField] private Slider hueSlider;
    [SerializeField] private Slider satSlider;

    [SerializeField] private Sprite colorModeSprite;
    [SerializeField] private Sprite brightnessModeSprite;
    [SerializeField] private IconButton modeSwitchButton;
    [SerializeField] private GameObject morePanel;
    [SerializeField] private IconButton moreButton;
    [SerializeField] private SmartifyOS.UI.Components.Button sameColorButton;
    [SerializeField] private SmartifyOS.UI.Components.Button settingsButton;

    private Texture2D hueTexture;
    private float currentHue;
    private float currentSat;
    private float currentVal;


    private bool isSelectionPage = true;
    private SelectedLightSource selectedLightSource;
    private bool isColorSelected;

    private void Awake()
    {
        hueSlider.onValueChanged.AddListener((value) =>
        {
            currentHue = value;
            UpdateColor();
        });

        satSlider.onValueChanged.AddListener((value) =>
        {
            currentVal = value;
            UpdateColor();
        });

        modeSwitchButton.onClick += () =>
        {
            if (isColorSelected)
            {
                hueSlider.gameObject.SetActive(true);
                satSlider.gameObject.SetActive(false);
                modeSwitchButton.SetIcon(brightnessModeSprite);
            }
            else
            {
                hueSlider.gameObject.SetActive(false);
                satSlider.gameObject.SetActive(true);
                modeSwitchButton.SetIcon(colorModeSprite);
            }

            isColorSelected = !isColorSelected;
        };

        moreButton.onClick += () =>
        {
            morePanel.SetActive(!morePanel.activeSelf);
        };

        leftLedSelectButton.onClick += () => { SelectSource(SelectedLightSource.LeftFeet); };
        rightLedSelectButton.onClick += () => { SelectSource(SelectedLightSource.RightFeet); };
        //TODO: Toggle lamp
        lampSelectButton.onClick += () => {  };

    }

    private void Start()
    {
        Init();

        colorScreen.SetActive(false);

        CreateHueImage();
    }

    private void SelectSource(SelectedLightSource source)
    {
        selectedLightSource = source;

        selectionScreen.SetActive(false);
        colorScreen.SetActive(true);

        switch (source)
        {
            case SelectedLightSource.LeftFeet:
                selectedLightText.text = "LEDs Left Feet";
                break;
            case SelectedLightSource.RightFeet:
                selectedLightText.text = "LEDs Right Feet";
                break;
        }
    }

    private void CreateHueImage()
    {
        hueTexture = new Texture2D(16, 1)
        {
            wrapMode = TextureWrapMode.Clamp,
            name = "HueTexture"
        };

        for (int i = 0; i < hueTexture.width; i++)
        {
            hueTexture.SetPixel(i, 0, Color.HSVToRGB((float)i / hueTexture.width, 1, 1f));
        }

        hueTexture.Apply();

        currentHue = 0;

        hueImage.texture = hueTexture;
    }

    private void UpdateColor()
    {
        color = Color.HSVToRGB(currentHue, 1, currentVal);
        previewImage.color = color;
        OnColorChanged?.Invoke(color);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);
        satSlider.value = currentVal;
        hueSlider.value = currentHue;
    }

    private enum SelectedLightSource
    {
        None,
        LeftFeet,
        RightFeet,
    }
}
