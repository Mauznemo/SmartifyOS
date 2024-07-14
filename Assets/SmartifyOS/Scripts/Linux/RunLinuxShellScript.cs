using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SmartifyOS.Linux
{
    public class RunLinuxShellScript
    {
        public static void Run(string path, string args = "" /*Action<string> OutputDataReceived, Action<string> ErrorDataReceived*/)
        {

            if (path.StartsWith("~"))
            {
                path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            }

            // Get the directory of the path
            string scriptDirectory = Path.GetDirectoryName(path);
            string scriptName = Path.GetFileName(path);

            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogError("File not found: " + path);
                return;
            }

            UnityEngine.Debug.Log("Script Directory: " + scriptDirectory);

            UnityEngine.Debug.Log("/bin/bash -c \"./" + scriptName + "\"");

            ProcessStartInfo psi = new ProcessStartInfo("/bin/bash", "-c \"./" + scriptName + "\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true, // Capture standard output
                RedirectStandardError = true,  // Capture standard error
                WorkingDirectory = scriptDirectory,
                CreateNoWindow = true           // Hide the window if running on a GUI system
            };

            // Start the process
            Process process = new Process
            {
                StartInfo = psi
            };

            process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data); // Log output
            process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data); // Log errors

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Optionally, wait for the process to exit
            process.WaitForExit();

            // Check the exit code
            int exitCode = process.ExitCode;
            UnityEngine.Debug.Log($"Process exited with code {exitCode}");
            /*
                        // Optional: Handle process output and errors asynchronously
                        process.OutputDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                UnityEngine.Debug.Log("Output: " + e.Data);
                                // OutputDataReceived?.Invoke(e.Data);
                            }
                        };
                        process.ErrorDataReceived += (sender, e) =>
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                UnityEngine.Debug.LogError("Error: " + e.Data);
                                //ErrorDataReceived?.Invoke(e.Data);
                            }
                        };

                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();*/
        }
    }
}

