using System;
using System.IO;
using SmartifyOS.LinuxFilePlayer;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;

namespace SmartifyOS.UI.MediaPlayer
{
    public class FilePlayer : BaseUIWindow
    {
        [SerializeField] private string filePath;

        [SerializeField] private IconButton previousButton;
        [SerializeField] private IconButton nextButton;
        [SerializeField] private IconButton playButton;
        [SerializeField] private PlayBar progressBar;

        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text artistText;
        [SerializeField] private TMP_Text sourceText;

        [SerializeField] private TMP_Text totalTimeText;
        [SerializeField] private TMP_Text currentTimeText;

        //[SerializeField] private UnityEngine.UI.Image play
        [SerializeField] private Sprite playingSprite;
        [SerializeField] private Sprite pausedSprite;

        [SerializeField] private FilePickerUIWindow filePicker;
        [SerializeField] private IconButton pickFileButton;

        [SerializeField] private float timeUntilClosing = 20f;

        private bool playing = false;
        private float timer = 0;

        private void Start()
        {
            Init();

            playButton.interactable = false;

            PlayerManager.OnMetadataChanged += PlayerManager_OnMetadataChanged;
            PlayerManager.OnDurationChanged += PlayerManager_OnDurationChanged;
            PlayerManager.OnEndOfFile += PlayerManager_OnEndOfFile;
        }

        private void Awake()
        {
            playButton.onClick += () =>
            {
                if (!PlayerManager.hasInstance)
                {
                    InstantiatePlayer();
                }

                else
                {
                    if (playing)
                    {
                        playing = false;
                        PlayerManager.Instance.Pause();
                        playButton.SetIcon(pausedSprite);
                    }
                    else
                    {
                        playing = true;
                        PlayerManager.Instance.Play();
                        playButton.SetIcon(playingSprite);
                    }
                }
            };

            pickFileButton.onClick += () =>
            {
                filePicker.Show();
            };

            progressBar.OnValueChanged += (value) =>
            {
                float timeStamp = value * PlayerManager.Instance.GetDuration();

                timer = timeStamp;
                PlayerManager.Instance.SkipTo(timeStamp);
            };
        }

        public void SelectAndPlay(string path)
        {
            filePath = path;
            InstantiatePlayer();

            playButton.interactable = true;
        }

        private void PlayerManager_OnEndOfFile()
        {
            UnityMainThreadDispatcher.GetInstance().Enqueue(EndOfFile);
        }

        private void EndOfFile()
        {
            playing = false;
            playButton.SetIcon(pausedSprite);

            Invoke(nameof(AutoHide), timeUntilClosing);
        }

        private void AutoHide()
        {
            Hide();
        }

        private void PlayerManager_OnDurationChanged(float duration)
        {
            totalTimeText.text = FormatTime(duration);
        }

        private void InstantiatePlayer()
        {
            if (filePath == "") return;
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                return;
            }

            Show();

            var file = new FileInfo(filePath);
            sourceText.text = file.Name;

            progressBar.SetValue(0);
            timer = 0;
            playing = true;
            PlayerManager.Instance.StartPlayerInstance(filePath);
            playButton.SetIcon(playingSprite);
            CancelInvoke(nameof(AutoHide));
        }

        private void Update()
        {
            if (PlayerManager.hasInstance && playing)
            {
                timer += Time.deltaTime;
                currentTimeText.text = FormatTime(timer);
                progressBar.SetValue(timer / PlayerManager.Instance.GetDuration());
            }
        }

        private void PlayerManager_OnMetadataChanged(PlayerManager.SongMetadata metadata)
        {
            titleText.text = metadata.title;
            artistText.text = metadata.artist;

            Debug.Log("Metadata other: " + metadata.year + " - " + metadata.album);
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

        public static string FormatTime(float timeInSeconds)
        {
            int hours = Mathf.FloorToInt(timeInSeconds / 3600);
            int minutes = Mathf.FloorToInt((timeInSeconds % 3600) / 60);
            int seconds = Mathf.FloorToInt(timeInSeconds % 60);

            if (hours > 0)
            {
                return $"{hours}:{minutes:D2}:{seconds:D2}";
            }
            else if (minutes > 0)
            {
                return $"{minutes}:{seconds:D2}";
            }
            else
            {
                return $"{seconds:D2}";
            }
        }
    }

}
