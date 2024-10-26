using System;
using System.Collections;
using System.Collections.Generic;
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
        private bool allowAutoOpen = true;

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
            titleText.text = obj;
        }

        private void OnPlayerPaused()
        {
            if (allowAutoOpen)
                Show();
            playButton.SetIcon(pausedSprite);
            playing = false;
        }

        private void OnPlayerPlaying()
        {
            if (allowAutoOpen)
                Show();
            playButton.SetIcon(playingSprite);
            playing = true;
        }

        protected override void HandleWindowOpened(BaseUIWindow window)
        {
            //Add all windows that should hide this window when they open
            if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Hide(true);
                allowAutoOpen = false;
            }
        }

        protected override void HandleWindowClosed(BaseUIWindow window)
        {
            if (!wasOpen) { return; }

            //Add all windows that should trigger this window to reopen when they close
            if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Show();
                allowAutoOpen = true;
            }
        }
    }

}

