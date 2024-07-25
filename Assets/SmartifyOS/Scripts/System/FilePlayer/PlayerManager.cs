using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Microsoft.CSharp.RuntimeBinder;

namespace SmartifyOS.LinuxFilePlayer
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        public static bool hasInstance { get; private set; }

        public static event Action<SongMetadata> OnMetadataChanged;
        public static event Action<float> OnDurationChanged;

        public static event Action OnEndOfFile;

        private const string SOCKET = "/tmp/mpvsocket";

        private Process process;
        private StreamWriter processInputWriter;

        private float duration = -1;
        private string filePath;

        private void Awake()
        {
            Instance = this;
        }

        public void StartPlayerInstance(string filePath)
        {
            this.filePath = filePath;
            // Check if the system is running on Linux
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                UnityEngine.Debug.Log("Unsupported platform: This function is intended for Linux systems only.");
                return;
            }

            if (hasInstance)
            {
                StopPlayerInstance();
            }

            hasInstance = true;
            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo("mpv");
            psi.Arguments = $"--input-ipc-server={SOCKET} --no-video \"{filePath}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            // Start the process
            process = new Process();
            process.StartInfo = psi;
            process.Start();

            // Read the output
            process.OutputDataReceived += OutputDataReceived;
            process.ErrorDataReceived += ErrorDataReceived;

            processInputWriter = process.StandardInput;

            process.BeginOutputReadLine();

            Invoke(nameof(RequestMetadata), 0.2f);
            Invoke(nameof(RequestDuration), 0.2f);

        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Contains("Exiting"))
            {
                hasInstance = false;

                if (e.Data.Contains("Errors when loading file"))
                {
                    UnityEngine.Debug.LogError("Errors while loading file");
                }
                else if (e.Data.ToLower().Contains("end"))
                {
                    UnityEngine.Debug.Log("End of file reached");
                    OnEndOfFile?.Invoke();
                }

            }

            //UnityEngine.Debug.Log(e.Data);
        }

        private void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.LogError(e.Data);
        }

        public void Pause()
        {
            LinuxCommand.Run("echo '{ \\\"command\\\": [\\\"set_property\\\", \\\"pause\\\", true] }' | socat - " + SOCKET);
        }

        public void Play()
        {
            LinuxCommand.Run("echo '{ \\\"command\\\": [\\\"set_property\\\", \\\"pause\\\", false] }' | socat - " + SOCKET);
        }

        public void StopPlayerInstance()
        {
            hasInstance = false;
            if (process != null && !process.HasExited)
            {
                process.Kill();
            }
        }

        public void SkipTo(float time)
        {
            if (process != null && !process.HasExited)
            {
                string output = LinuxCommand.Run("echo '{ \\\"command\\\": [\\\"seek\\\", \\\"" + time.ToString().Replace(",", ".") + "\\\", \\\"absolute\\\"] }' | socat - " + SOCKET);
            }
        }

        public float GetDuration()
        {
            return duration;
        }

        private void RequestDuration()
        {
            if (process != null && !process.HasExited)
            {
                string output = LinuxCommand.Run("echo '{ \\\"command\\\": [\\\"get_property\\\", \\\"duration\\\"] }' | socat - " + SOCKET);
                dynamic data = JsonConvert.DeserializeObject<dynamic>(output);
                if (data != null)
                {
                    duration = data.data;
                    OnDurationChanged?.Invoke(duration);
                }
                else
                {
                    Invoke(nameof(RequestDuration), 1f);
                }
            }
        }

        private void RequestMetadata()
        {
            if (process != null && !process.HasExited)
            {
                string output = LinuxCommand.Run("echo '{ \\\"command\\\": [\\\"get_property\\\", \\\"metadata\\\"] }' | socat - " + SOCKET);
                dynamic data = JsonConvert.DeserializeObject<dynamic>(output);


                try
                {
                    string title = data.data.title;
                    if (string.IsNullOrEmpty(title))
                    {
                        title = new FileInfo(filePath).Name;
                    }
                    SongMetadata metadata = new SongMetadata
                    {
                        title = title,
                        artist = data.data.artist,
                        album = data.data.album,
                        year = data.data.date
                    };

                    OnMetadataChanged?.Invoke(metadata);
                }
                catch (RuntimeBinderException e)
                {
                    SongMetadata TempMetadata = new SongMetadata
                    {
                        title = "Loading...",
                        artist = "",
                        album = "",
                        year = ""
                    };

                    OnMetadataChanged?.Invoke(TempMetadata);
                    Invoke(nameof(RequestMetadata), 1f);
                }

            }

        }

        void OnDestroy()
        {
            StopPlayerInstance();
        }

        public struct SongMetadata
        {
            public string title;
            public string artist;
            public string album;
            public string year;
        }
    }
}

