using System.IO;
using SmartifyOS.SaveSystem;
using SmartifyOS.SystemEvents;
using SmartifyOS.UI;
using TMPro;
using UnityEngine;

public class ReverseCameraUIWindow : BaseUIWindow
{
    public bool updateCameraFeed;

    [SerializeField] private UIReverseOverlay uiReverseOverlay;

    [SerializeField] private RenderTexture webcamRenderTexture;

    [SerializeField] private GameObject warningScreen;
    [SerializeField] private TMP_Text warningText;

    private WebCamTexture webcamTexture;

    private void Start()
    {
        Init();

        InitCamera();
    }

    private void Update()
    {
        UpdateCameraFeed();
    }

    private void InitCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            warningScreen.SetActive(true);
            warningText.text = "No camera found!";

            return;
        }

        if (devices.Length - 1 < SaveManager.Load().camera.currentCameraIndex)
        {
            warningScreen.SetActive(true);
            warningText.text = "Camera index out of range!";

            return;
        }

        Debug.Log($"Using camera: {devices[SaveManager.Load().camera.currentCameraIndex].name} at index: {SaveManager.Load().camera.currentCameraIndex}");
        webcamTexture = new WebCamTexture(devices[SaveManager.Load().camera.currentCameraIndex].name);
        webcamTexture.Play();
    }

    private void UpdateCameraFeed()
    {
        if (!updateCameraFeed || webcamTexture == null)
            return;

        Graphics.Blit(webcamTexture, webcamRenderTexture);

        uiReverseOverlay.steeringAngle = LiveDataController.wheelAngle / 36f;
    }


    protected override void OnShow()
    {
        EnableCameraConverter(true);

        updateCameraFeed = true;
    }

    protected override void OnHide()
    {
        EnableCameraConverter(false);

        updateCameraFeed = false;
    }

    //Only needed if Unity doesn't work with your camera
    private void EnableCameraConverter(bool enable)
    {
        SystemEventManager.CallEvent("SmartifyOS/Events/SetReverseCam", enable.ToString().ToLower());
    }

}
