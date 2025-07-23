using System;
using SmartifyOS.LinuxBluetooth;
using SmartifyOS.SaveSystem;
using SmartifyOS.Settings;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;

namespace SmartifyOS.UI.MediaPlayer
{
    public class BluetoothPlayer : BaseUIWindow
    {
        public event Action OnOpened;
        [SerializeField] private IconButton previousButton;
        [SerializeField] private IconButton nextButton;
        [SerializeField] private IconButton playButton;
        [SerializeField] private IconButton settingsButton;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text artistText;
        [SerializeField] private TMP_Text sourceText;

        //[SerializeField] private UnityEngine.UI.Image play
        [SerializeField] private Sprite playingSprite;
        [SerializeField] private Sprite pausedSprite;

        private bool playing = false;

        private void Awake()
        {
            previousButton.onClick += () =>
            {
                BluetoothManager.Instance.PlayerPrevious();
            };
            nextButton.onClick += () =>
            {
                BluetoothManager.Instance.PlayerNext();
            };
            playButton.onClick += () =>
            {
                if (playing)
                {
                    BluetoothManager.Instance.PlayerPause();
                    playButton.SetIcon(pausedSprite);
                }
                else
                {
                    BluetoothManager.Instance.PlayerPlay();
                    playButton.SetIcon(playingSprite);
                }

                playing = !playing;
            };

            settingsButton.onClick += () =>
            {
                SettingsManager.Instance.ShowSettingsPage<BluetoothSettingsPage>();
            };
        }

        private void Start()
        {
            Init();

            BluetoothManager.OnPlayerPlaying += OnPlayerPlaying;
            BluetoothManager.OnPlayerPaused += OnPlayerPaused;
            BluetoothManager.OnPlayerTitleChanged += OnPlayerTitleChanged;
            BluetoothManager.OnPlayerArtistChanged += OnPlayerArtistChanged;
            BluetoothManager.OnPlayerStopped += OnPlayerStopped;

            Invoke(nameof(GetPlayer), 0.5f);
        }

        protected override void OnShow()
        {
            if (SaveManager.Load().system.autoplayOnConnect)
            {
                BluetoothManager.Instance.PlayerPlay();
                playButton.SetIcon(playingSprite);
                playing = true;
            }

            var connectedDevices = BluetoothManager.Instance.ListConnectedDevices();
            if (connectedDevices.Count > 0)
            {
                sourceText.text = connectedDevices[0].name;
            }
            OnOpened?.Invoke();
        }

        private void OnPlayerStopped()
        {
            Hide();
        }

        private void GetPlayer()
        {
            BluetoothManager.Instance.SendPlayerCommand("show");
        }

        private void OnPlayerArtistChanged(string obj)
        {
            artistText.text = obj;
        }

        private void OnPlayerTitleChanged(string obj)
        {
            if (string.IsNullOrEmpty(obj) || obj.ToLower().Contains("not provided"))
            {
                Hide();
            }

            titleText.text = obj;
        }

        private void OnPlayerPaused()
        {
            if (!UIManager.Instance.IsWindowVisible<FilePlayer>() || !UIManager.Instance.IsWindowVisible<InteriorUIWindow>())
                Show(ShowAction.OpenInBackground);

            playButton.SetIcon(pausedSprite);
            playing = false;
        }

        private void OnPlayerPlaying()
        {

            if (!UIManager.Instance.IsWindowVisible<FilePlayer>() || !UIManager.Instance.IsWindowVisible<InteriorUIWindow>())
                Show(ShowAction.OpenInBackground);

            playButton.SetIcon(playingSprite);
            playing = true;
        }
    }

}

