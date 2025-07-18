using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CollinExecute
{
    public static class Utils
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static string GetShell()
        {
            if (IsWindows()) return "cmd.exe";
            if (IsMacOS()) return "/bin/bash";
            if (IsLinux()) return "/bin/bash";
            throw new PlatformNotSupportedException("Unsupported OS");
        }

        public static string GetShellArgs(string command)
        {
            // Proper shell switch differs by platform.
            if (IsWindows())
            {
                // /c runs the command & then exits cmd.exe
                return $"/c {command}";
            }
            else
            {
                // Quote so spaces & operators work as expected
                return $"-c \"{command}\"";
            }
        }
    }

    public class Shell
    {
        /// <summary>
        /// Run a shell command.
        /// </summary>
        /// <param name="command">Command line to execute.</param>
        /// <param name="stream">
        /// If true, write stdout/stderr to Console as lines arrive.
        /// If false, capture them silently (change signature to surface captured text if needed).
        /// </param>
        /// <param name="treatStderrAsFailure">
        /// If true, ANY stderr output causes failure even if exit code == 0.
        /// If false, only the exit code is used to decide success/failure.
        /// </param>
        /// <returns>
        /// true on success, false on failure (non-zero exit OR stderr when treatStderrAsFailure == true).
        /// </returns>
        public static bool SystemCommand(string command, bool stream = true, bool treatStderrAsFailure = false)
        {
            var psi = new ProcessStartInfo
            {
                FileName = Utils.GetShell(),
                Arguments = Utils.GetShellArgs(command),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi, EnableRaisingEvents = false };

            StringBuilder? stdoutBuf = stream ? null : new StringBuilder();
            StringBuilder? stderrBuf = stream ? null : new StringBuilder();
            bool sawStderr = false;

            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data is null) return;
                if (stream) Console.WriteLine(args.Data);
                else stdoutBuf!.AppendLine(args.Data);
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data is null) return;
                sawStderr = true;
                if (stream) Console.WriteLine($"ERR: {args.Data}");
                else stderrBuf!.AppendLine(args.Data);
            };

            try
            {
                if (!process.Start())
                {
                    // Could not start process at all.
                    return false; // invert here if needed
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                bool exitOk = process.ExitCode == 0;
                bool stderrOk = !treatStderrAsFailure || !sawStderr;

                bool success = exitOk && stderrOk;

                return success;
            }
            catch (Exception ex)
            {
                if (stream)
                {
                    Console.WriteLine($"Exception starting process: {ex.Message}");
                }
                else
                {
                    stderrBuf?.AppendLine(ex.ToString());
                }
                return false; // invert here if you want true-on-failure
            }
        }
    }
}
