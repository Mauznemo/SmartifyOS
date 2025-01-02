using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmartifyOS
{
    public class LinuxCommand
    {
        public static string Run(string command)
        {
            // Check if the system is running on Linux
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                UnityEngine.Debug.Log("Unsupported platform: This function is intended for Linux systems only.");
                return "";
            }
            // Create process start info
            ProcessStartInfo psi = new ProcessStartInfo("/bin/bash", "-c \"" + command + "\"");
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            // Start the process
            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            // Read the output
            string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit(10_000);

            if (process != null && !process.HasExited)
            {
                UnityEngine.Debug.LogError("Process timed out!");
                process.Kill(); // Terminate the process
            }

            string errors = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(errors))
            {
                UnityEngine.Debug.LogError("Command execution error: " + errors);
            }

            return output;
        }

        public static async Task<string> RunAsync(string command)
        {
            // Check if the system is running on Linux
            if (System.Environment.OSVersion.Platform != PlatformID.Unix)
            {
                UnityEngine.Debug.Log("Unsupported platform: This function is intended for Linux systems only.");
                return "";
            }

            ProcessStartInfo psi = new ProcessStartInfo("/bin/bash", $"-c \"{command}\"");
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            // Asynchronously read the output
            string output = await process.StandardOutput.ReadToEndAsync();

            // Wait for the process to finish
            await Task.Run(() => process.WaitForExit(10_000));
            if (process != null && !process.HasExited)
            {
                UnityEngine.Debug.LogError("Process timed out!");
                process.Kill(); // Terminate the process
            }

            string errors = process.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(errors))
            {
                UnityEngine.Debug.LogError("Command execution error: " + errors);
            }

            return output;
        }
    }
}

