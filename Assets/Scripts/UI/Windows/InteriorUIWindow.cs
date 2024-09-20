using System;
using Cinemachine;
using SmartifyOS.Settings;
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

    [SerializeField] private MeshRenderer lightMeshRenderer;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    //For controlwheel
    [SerializeField] private Outline[] selectButtonOutlines;
    private int selectedButton = -1;

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
            ToggleSliderMode();
        };

        moreButton.onClick += () =>
        {
            morePanel.SetActive(!morePanel.activeSelf);
        };

        sameColorButton.onClick += () =>
        {
            if (isSelectionPage)
                return;

            var color = Color.magenta;

            switch (selectedLightSource)
            {
                case SelectedLightSource.LeftFeet:
                    color = LedManager.Instance.GetSavedColor(LedStrip.Right);
                    break;
                case SelectedLightSource.RightFeet:
                    color = LedManager.Instance.GetSavedColor(LedStrip.Left);
                    break;
            }

            SetColor(color);

            morePanel.SetActive(false);
        };

        settingsButton.onClick += () => { SettingsManager.Instance.ShowSettingsPage<InteriorLightingSettingsPage>(); morePanel.SetActive(false); };

        leftLedSelectButton.onClick += () => { SelectSource(SelectedLightSource.LeftFeet); };
        rightLedSelectButton.onClick += () => { SelectSource(SelectedLightSource.RightFeet); };
        lampSelectButton.onClick += () =>
        {
            ToggleLight();
        };

        backButton.onClick.AddListener(() =>
        {
            selectedLightText.text = "Ambient";

            isSelectionPage = true;
            backButton.gameObject.SetActive(false);
            selectionScreen.SetActive(true);
            colorScreen.SetActive(false);
            morePanel.SetActive(false);

            for (int i = 0; i < selectButtonOutlines.Length; i++)
            {
                selectButtonOutlines[i].enabled = false;
            }
            selectedButton = -1;
        });

    }

    private void ToggleLight()
    {
        if (LedManager.interiorLightOn)
        {
            LedManager.Instance.SetInteriorLight(false);
            lightMeshRenderer.material = offMaterial;
        }
        else
        {
            LedManager.Instance.SetInteriorLight(true);
            lightMeshRenderer.material = onMaterial;
        }
    }

    private void ToggleSliderMode()
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
    }

    private void Start()
    {
        Init();

        CreateHueImage();

        interiorOnlyObjects.SetActive(false);

        MainController.OnInteriorLightChanged += MainController_OnInteriorLightChanged;

        ControlwheelManager.OnButtonPressed += ControlwheelManager_OnButtonPressed;
        ControlwheelManager.OnChanged += ControlwheelManager_OnChanged;
    }

    private void ControlwheelManager_OnChanged(int dir)
    {
        if (isSelectionPage)
        {
            selectedButton = (selectedButton + dir + 3) % 3;
            for (int i = 0; i < selectButtonOutlines.Length; i++)
            {
                selectButtonOutlines[i].enabled = i == selectedButton;
            }
            return;
        }

        if (!isColorSelected)
        {
            hueSlider.value += dir * 0.01f;
        }
        else
        {
            satSlider.value += dir * 0.01f;
        }
    }

    private void ControlwheelManager_OnButtonPressed()
    {
        if (isSelectionPage)
        {
            switch (selectedButton)
            {
                case 0:
                    SelectSource(SelectedLightSource.LeftFeet);
                    break;
                case 1:
                    ToggleLight();
                    break;
                case 2:
                    SelectSource(SelectedLightSource.RightFeet);
                    break;
            }
            return;
        }
        if (ControlwheelManager.GetMode() == ControlwheelManager.Mode.Ambient)
        {
            ToggleSliderMode();
        }
    }

    private void MainController_OnInteriorLightChanged(bool isOn)
    {
        if (isOn)
        {
            lightMeshRenderer.material = onMaterial;
        }
        else
        {
            lightMeshRenderer.material = offMaterial;
        }
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

        InitPointLights();

        morePanel.SetActive(false);

        Invoke(nameof(ShowSelectionScreen), 2f);

        for (int i = 0; i < selectButtonOutlines.Length; i++)
        {
            selectButtonOutlines[i].enabled = false;
        }
        selectedButton = -1;

        ControlwheelManager.SetMode(ControlwheelManager.Mode.Ambient);
    }

    private void ShowSelectionScreen()
    {
        selectionScreen.SetActive(true);
    }

    protected override void OnHide()
    {
        virtualCamera.Priority = 0;

        interiorOnlyObjects.SetActive(false);

        ControlwheelManager.SetDefaultMode();
    }

    private void SelectSource(SelectedLightSource source)
    {
        isSelectionPage = false;
        backButton.gameObject.SetActive(true);

        selectedLightSource = source;

        selectionScreen.SetActive(false);
        colorScreen.SetActive(true);

        SetColor(LedManager.Instance.GetSavedColor(LightSourceToLedStrip(source)));

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
        UpdatePointLight();
        LedManager.Instance.SetLedColor(LightSourceToLedStrip(selectedLightSource), color);
    }

    public void SetColor(Color color)
    {
        this.color = color;
        Color.RGBToHSV(color, out currentHue, out currentSat, out currentVal);
        satSlider.value = currentVal;
        hueSlider.value = currentHue;

        UpdatePointLight();
    }

    private void UpdatePointLight()
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

    private void InitPointLights()
    {
        leftLight.color = LedManager.Instance.GetSavedColor(LedStrip.Left);
        rightLight.color = LedManager.Instance.GetSavedColor(LedStrip.Right);
    }

    private SelectedLightSource LedStripToLightSource(LedStrip ledStrip)
    {
        switch (ledStrip)
        {
            case LedStrip.Left:
                return SelectedLightSource.LeftFeet;
            case LedStrip.Right:
                return SelectedLightSource.RightFeet;
            default:
                throw new ArgumentOutOfRangeException(nameof(ledStrip), ledStrip, null);
        }
    }

    private LedStrip LightSourceToLedStrip(SelectedLightSource selectedLightSource)
    {
        switch (selectedLightSource)
        {
            case SelectedLightSource.LeftFeet:
                return LedStrip.Left;
            case SelectedLightSource.RightFeet:
                return LedStrip.Right;
            default:
                throw new ArgumentOutOfRangeException(nameof(selectedLightSource), selectedLightSource, null);
        }
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
