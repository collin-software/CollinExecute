using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CollinExecute
{
    /// <summary>
    /// Provides utility methods for cross-platform system operations, particularly for detecting 
    /// the operating system and configuring shell command execution parameters.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Determines if the current operating system is Windows.
        /// </summary>
        /// <returns><c>true</c> if running on Windows; otherwise, <c>false</c>.</returns>
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Determines if the current operating system is macOS.
        /// </summary>
        /// <returns><c>true</c> if running on macOS; otherwise, <c>false</c>.</returns>
        public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        /// <summary>
        /// Determines if the current operating system is Linux.
        /// </summary>
        /// <returns><c>true</c> if running on Linux; otherwise, <c>false</c>.</returns>
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        /// <summary>
        /// Gets the appropriate shell executable path for the current operating system.
        /// </summary>
        /// <returns>
        /// The shell executable path: "cmd.exe" for Windows, "/bin/bash" for macOS and Linux.
        /// </returns>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown when the current operating system is not supported (not Windows, macOS, or Linux).
        /// </exception>
        public static string GetShell()
        {
            if (IsWindows()) return "cmd.exe";
            if (IsMacOS()) return "/bin/bash";
            if (IsLinux()) return "/bin/bash";
            throw new PlatformNotSupportedException("Unsupported OS");
        }

        /// <summary>
        /// Generates the appropriate shell arguments for executing a command on the current operating system.
        /// </summary>
        /// <param name="command">The command to be executed by the shell.</param>
        /// <returns>
        /// The properly formatted shell arguments: "/c {command}" for Windows (cmd.exe), 
        /// or "-c \"{command}\"" for Unix-based systems (bash).
        /// </returns>
        /// <remarks>
        /// For Windows, uses the /c switch which runs the command and then exits cmd.exe.
        /// For Unix-based systems, uses the -c switch with proper quoting to handle spaces and operators.
        /// </remarks>
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

    /// <summary>
    /// Provides methods for executing shell commands across different operating systems with support for 
    /// streaming output, error handling, and customizable failure conditions.
    /// </summary>
    public class Shell
    {
        /// <summary>
        /// Executes a shell command on the current operating system with configurable output handling and error treatment.
        /// </summary>
        /// <param name="command">The command line to execute in the system shell.</param>
        /// <param name="stream">
        /// If <c>true</c>, writes stdout and stderr to the console as lines arrive in real-time.
        /// If <c>false</c>, captures output silently without displaying it.
        /// </param>
        /// <param name="treatStderrAsFailure">
        /// If <c>true</c>, any stderr output causes the method to return <c>false</c> even if the exit code is 0.
        /// If <c>false</c>, only the process exit code determines success or failure.
        /// </param>
        /// <returns>
        /// <c>true</c> if the command executed successfully (exit code 0 and no stderr when <paramref name="treatStderrAsFailure"/> is <c>true</c>);
        /// <c>false</c> if the command failed (non-zero exit code or stderr output when <paramref name="treatStderrAsFailure"/> is <c>true</c>).
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method uses the appropriate shell for the current operating system:
        /// - Windows: cmd.exe with /c switch
        /// - macOS/Linux: /bin/bash with -c switch
        /// </para>
        /// <para>
        /// When <paramref name="stream"/> is <c>true</c>, stderr output is prefixed with "ERR: " for clarity.
        /// </para>
        /// <para>
        /// The method handles process startup exceptions gracefully and returns <c>false</c> on any execution errors.
        /// </para>
        /// </remarks>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown when the current operating system is not supported by the <see cref="Utils.GetShell"/> method.
        /// </exception>
        /// <example>
        /// <code>
        /// // Execute a simple command and stream output
        /// bool success = Shell.SystemCommand("echo Hello World");
        /// 
        /// // Execute a command silently and treat stderr as failure
        /// bool success = Shell.SystemCommand("git status", stream: false, treatStderrAsFailure: true);
        /// 
        /// // Execute a command that might produce stderr but shouldn't fail
        /// bool success = Shell.SystemCommand("npm install", stream: true, treatStderrAsFailure: false);
        /// </code>
        /// </example>
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
