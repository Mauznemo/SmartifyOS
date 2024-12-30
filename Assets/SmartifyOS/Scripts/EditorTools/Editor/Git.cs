using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace SmartifyOS.Editor
{
    public class Git
    {
        public static async Task<(string, string)> Command(string command)
        {
            if (!IsGitInstalled())
            {
                bool result = EditorUtility.DisplayDialog("Error", "Git is not installed. Please install Git and try again.", "Install", "Cancel");
                if (result)
                {
                    Application.OpenURL("https://git-scm.com/downloads");
                }
                throw new Exception("Git is not installed.");
            }

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
                //processInfo.Arguments = $"-c \"LANG=en_US.UTF-8 git {command}\"";
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
            if (!string.IsNullOrEmpty(output))
            {
                if (output.Contains("CONFLICT") || output.Contains("error"))
                {
                    UnityEngine.Debug.LogError(output);

                    if (output.Trim() != error.Trim())
                    {
                        error = output;
                    }
                }
            }

            if (!string.IsNullOrEmpty(error))
            {
                if (error.Contains("CONFLICT") || error.Contains("error"))
                {
                    UnityEngine.Debug.LogError(error);
                }
            }

            return (output, error);
        }

        public static async Task<bool> HasUpstream()
        {
            (string output, string error) = await Command("remote -v");
            return output.Contains("upstream");
        }


        /// <summary>
        /// Get count of how many commits local branch is ahead of remote
        /// </summary>
        /// <param name="branch">remote branch</param>
        /// <returns>Count of commits ahead</returns>
        public static async Task<int> GetAhead(string branch = "origin/main")
        {
            (string output, string error) = await Command($"rev-list --count {branch}..HEAD");
            return int.Parse(output);
        }


        /// <summary>
        /// Get count of how many commits local branch is behind remote
        /// </summary>
        /// <param name="branch">remote branch</param>
        /// <returns>Count of commits behind</returns>
        public static async Task<int> GetBehind(string branch = "origin/main")
        {
            (string output, string error) = await Command($"rev-list --count HEAD..{branch}");
            return int.Parse(output);
        }

        public static async Task<List<Diff>> GetDiffs(string branch = "origin/main")
        {
            (string output, string error) = await Command($"diff --name-status HEAD..{branch}");
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

        public static async Task<List<string>> GetChangelog(string branch = "origin/main")
        {
            (string output, string error) = await Command($"log ..{branch} --pretty=format:\"%s\"");
            List<string> changeLog = new List<string>();

            string[] lines = output.Split('\n');
            foreach (string line in lines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    changeLog.Add(line);
                }
            }

            return changeLog;
        }

        public static bool IsGitInstalled()
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            Process process = new Process();

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                processInfo.FileName = "cmd.exe";
                processInfo.Arguments = "/c git --version";
            }
            else if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.LinuxEditor)
            {
                processInfo.FileName = "/bin/bash";
                processInfo.Arguments = "-c \"git --version\"";
            }

            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;
            processInfo.CreateNoWindow = true;

            process.StartInfo = processInfo;

            try
            {
                process.Start();
                string standardError = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Git is installed if the exit code is 0 and no errors occurred
                return process.ExitCode == 0 && string.IsNullOrEmpty(standardError);
            }
            catch
            {
                // If an exception occurs, Git is likely not installed
                return false;
            }
            finally
            {
                process.Dispose();
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
