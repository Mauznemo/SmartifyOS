using System.Collections;
using System.Collections.Generic;
using SmartifyOS.LinuxBluetooth;
using SmartifyOS.Settings;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;

namespace SmartifyOS.UI.MediaPlayer
{
    public class BluetoothPlayer : BaseUIWindow
    {
        [SerializeField] private IconButton previousButton;
        [SerializeField] private IconButton nextButton;
        [SerializeField] private IconButton playButton;
        [SerializeField] private IconButton settingsButton;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text artistText;

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
            BluetoothManager.OnPlayerPlaying += OnPlayerPlaying;
            BluetoothManager.OnPlayerPaused += OnPlayerPaused;
            BluetoothManager.OnPlayerTitleChanged += OnPlayerTitleChanged;
            BluetoothManager.OnPlayerArtistChanged += OnPlayerArtistChanged;
            BluetoothManager.OnPlayerStopped += OnPlayerStopped;

            Invoke(nameof(GetPlayer), 0.5f);
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
            Show();
            playButton.SetIcon(pausedSprite);
            playing = false;
        }

        private void OnPlayerPlaying()
        {
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
            }
        }

        protected override void HandleWindowClosed(BaseUIWindow window)
        {
            if (!wasOpen) { return; }

            //Add all windows that should trigger this window to reopen when they close
            if (window.IsWindowOfType(typeof(InteriorUIWindow)))
            {
                Show();
            }
        }
    }

}

