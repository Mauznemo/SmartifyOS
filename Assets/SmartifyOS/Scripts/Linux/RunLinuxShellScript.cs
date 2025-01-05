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
        public static string Run(string path, string args = "")
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
                return "file_not_found";
            }

            UnityEngine.Debug.Log("Script Directory: " + scriptDirectory);

            ProcessStartInfo psi = new ProcessStartInfo("/bin/bash", "-c \"./" + scriptName + " " + args + "\"")
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

            //process.OutputDataReceived += (sender, args) => UnityEngine.Debug.Log(args.Data); // Log output
            //process.ErrorDataReceived += (sender, args) => UnityEngine.Debug.LogError(args.Data); // Log errors

            process.Start();
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();


            // Optionally, wait for the process to exit
            process.WaitForExit(01_000);
            if (process != null && !process.HasExited)
            {
                UnityEngine.Debug.LogError("Process timed out!");
                process.Kill(); // Terminate the process
            }

            string output = process.StandardOutput.ReadToEnd();
            // Check the exit code
            int exitCode = process.ExitCode;
            UnityEngine.Debug.Log($"Process exited with code {exitCode}");

            return output;
        }

        public static void RunWithWindow(string path, string args = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
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

            string[] terminalEmulators = { "gnome-terminal", "xterm", "konsole", "mate-terminal", "xfce4-terminal", "lxterminal" };
            string terminalCommand = null;

            foreach (var terminal in terminalEmulators)
            {
                if (File.Exists($"/usr/bin/{terminal}"))
                {
                    if (terminal == "lxterminal")
                    {
                        terminalCommand = $"{terminal} -e 'cd {scriptDirectory} && ./{scriptName} {args}; exit'";
                        UnityEngine.Debug.Log(terminalCommand);
                        break;
                    }
                    else
                    {
                        terminalCommand = $"{terminal} -- bash -c 'cd {scriptDirectory} && ./{scriptName} {args}; exit'";
                        UnityEngine.Debug.Log(terminalCommand);
                        break;
                    }
                }
            }

            if (terminalCommand == null)
            {
                UnityEngine.Debug.LogError("No suitable terminal emulator found.");
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo("/bin/bash", "-c \"" + terminalCommand + "\"")
            {
                UseShellExecute = true,
                WorkingDirectory = scriptDirectory,
                WindowStyle = windowStyle
            };

            // Start the process
            Process process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            return;
        }
    }
}

