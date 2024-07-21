using System;
using Cinemachine;
using SmartifyOS.UI;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class InteriorUIWindow : BaseUIWindow
{
    //public event Action<Color> OnColorChanged;

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
    [SerializeField] private Button backButton;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private GameObject interiorOnlyObjects;
    [SerializeField] private Light leftLight;
    [SerializeField] private Light rightLight;

    private Texture2D hueTexture;
    private float currentHue;
    private float currentSat;
    private float currentVal;


    private bool isSelectionPage = true;
    private SelectedLightSource selectedLightSource;
    private bool isColorSelected;

    private void Awake()
    {
        currentVal = 1;

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
        lampSelectButton.onClick += () => { };

        backButton.onClick.AddListener(() =>
        {
            selectedLightText.text = "Ambient";

            isSelectionPage = true;
            backButton.gameObject.SetActive(false);
            selectionScreen.SetActive(true);
            colorScreen.SetActive(false);
        });

    }

    private void Start()
    {
        Init();

        CreateHueImage();

        interiorOnlyObjects.SetActive(false);
    }

    protected override void OnShow()
    {
        virtualCamera.Priority = 100;

        interiorOnlyObjects.SetActive(true);

        colorScreen.SetActive(false);
        selectionScreen.SetActive(false);
        backButton.gameObject.SetActive(false);
        selectedLightText.text = "Ambient";

        isSelectionPage = true;

        Invoke(nameof(ShowSelectionScreen), 2f);
    }

    private void ShowSelectionScreen()
    {
        selectionScreen.SetActive(true);
    }

    protected override void OnHide()
    {
        virtualCamera.Priority = 0;

        interiorOnlyObjects.SetActive(false);
    }

    private void SelectSource(SelectedLightSource source)
    {
        isSelectionPage = false;
        backButton.gameObject.SetActive(true);

        selectedLightSource = source;

        selectionScreen.SetActive(false);
        colorScreen.SetActive(true);

        switch (source)
        {
            case SelectedLightSource.LeftFeet:
                selectedLightText.text = "      LEDs Left Feet";
                break;
            case SelectedLightSource.RightFeet:
                selectedLightText.text = "      LEDs Right Feet";
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
        //OnColorChanged?.Invoke(color);
        OnColorChanged(color);
    }

    private void OnColorChanged(Color color)
    {
        switch (selectedLightSource)
        {
            case SelectedLightSource.LeftFeet:
                leftLight.color = color;
                break;
            case SelectedLightSource.RightFeet:
                rightLight.color = color;
                break;
        }
    }

    public void SetColor(Color color)
    {
        this.color = color;
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);
        satSlider.value = currentVal;
        hueSlider.value = currentHue;
    }

    protected override void HandleWindowOpened(BaseUIWindow window)
    {
        if (window.IsWindowOfType(typeof(AppListUIWindow)))
        {
            Hide(true);
        }
    }

    private enum SelectedLightSource
    {
        None,
        LeftFeet,
        RightFeet,
    }
}
