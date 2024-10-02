using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace SmartifyOS.Editor
{
    public class Git
    {
        public static async Task<string> Command(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            Process process = new Process();

            // Use different shell commands based on OS
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                processInfo.FileName = "cmd.exe";
                processInfo.Arguments = $"/c git {command}";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {
                processInfo.FileName = "/bin/bash";
                processInfo.Arguments = $"-c \"git {command}\"";
            }

            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            process.StartInfo = processInfo;
            process.Start();

            // Read the output
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await Task.Run(() => process.WaitForExit());
            process.Close();

            // Log output and errors
            /*if (!string.IsNullOrEmpty(output))
            {
                UnityEngine.Debug.Log(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError(error);
            }*/

            return output;
        }

        public static async Task<bool> HasUpstream()
        {
            string output = await Command("remote -v");
            return output.Contains("upstream");
        }


        /// <summary>
        /// Get count of how many commits local branch is ahead of remote
        /// </summary>
        /// <param name="branch">remote branch</param>
        /// <returns>Count of commits ahead</returns>
        public static async Task<int> GetAhead(string branch = "origin/main")
        {
            string output = await Command($"rev-list --count {branch}..HEAD");
            return int.Parse(output);
        }


        /// <summary>
        /// Get count of how many commits local branch is behind remote
        /// </summary>
        /// <param name="branch">remote branch</param>
        /// <returns>Count of commits behind</returns>
        public static async Task<int> GetBehind(string branch = "origin/main")
        {
            string output = await Command($"rev-list --count HEAD..{branch}");
            return int.Parse(output);
        }

        public static async Task<List<Diff>> GetDiffs(string branch = "origin/main")
        {
            string output = await Command($"diff --name-status HEAD..{branch}");
            string[] lines = output.Split('\n');
            List<Diff> diffs = new List<Diff>();

            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                DiffType diffType;

                if (line.StartsWith("A"))
                {
                    diffType = DiffType.Added;
                }
                else if (line.StartsWith("M"))
                {
                    diffType = DiffType.Modified;
                }
                else if (line.StartsWith("D"))
                {
                    diffType = DiffType.Deleted;
                }
                else
                {
                    continue;
                }

                diffs.Add(new Diff
                {
                    name = line.Substring(1).Trim(),
                    diffType = diffType
                });
            }

            return diffs;
        }

        public static async Task<bool> CloneRepository(string url, string path)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(path))
            {
                UnityEngine.Debug.LogError("URL and path must not be empty");
                return false;
            }

            // Ensure the target path is valid
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string command = $"clone {url} {path}";
            try
            {
                await Command(command);
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to clone repository: {e.Message}");
                return false;
            }
        }

        public struct Diff
        {
            public string name;
            public DiffType diffType;
        }

        public enum DiffType
        {
            Added,
            Modified,
            Deleted
        }

    }
}
