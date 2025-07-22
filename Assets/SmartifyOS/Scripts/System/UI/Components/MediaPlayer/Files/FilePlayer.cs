using System;
using System.IO;
using SmartifyOS.LinuxFilePlayer;
using SmartifyOS.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SmartifyOS.UI.MediaPlayer
{
    public class FilePlayer : BaseUIWindow, IDragHandler, IEndDragHandler, IBeginDragHandler
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

        [SerializeField] private BluetoothPlayer bluetoothPlayer;

        [SerializeField] private float timeUntilClosing = 20f;

        private bool playing = false;
        private float timer = 0;

        private Vector2 offset;
        private Vector2 startPosition;

        private void Start()
        {
            Init();

            playButton.interactable = false;

            PlayerManager.OnMetadataChanged += PlayerManager_OnMetadataChanged;
            PlayerManager.OnDurationChanged += PlayerManager_OnDurationChanged;
            PlayerManager.OnEndOfFile += PlayerManager_OnEndOfFile;

            bluetoothPlayer.OnOpened += () =>
            {
                Hide();
            };
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

            Show(ShowAction.OpenInBackground);

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

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = eventData.position - (Vector2)transform.position;
            startPosition = transform.position;
            LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.2f).setEaseInOutSine();
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position - offset;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (Vector2.Distance(transform.position, startPosition) > 100)
            {
                Hide();
                transform.position = startPosition;
            }
            else
            {
                LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutSine();
                transform.position = startPosition;
            }
        }

    }

}
