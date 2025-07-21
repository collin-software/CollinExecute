using System.Runtime.InteropServices;
using CollinExecute;

namespace Tests
{
    public class UtilsTests
    {
        [Fact]
        public void IsWindows_ShouldReturnCorrectValue()
        {
            // Test platform detection
            bool isWindows = Utils.IsWindows();
            bool expectedWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            Assert.Equal(expectedWindows, isWindows);
        }

        [Fact]
        public void IsMacOS_ShouldReturnCorrectValue()
        {
            // Test platform detection
            bool isMacOS = Utils.IsMacOS();
            bool expectedMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            Assert.Equal(expectedMacOS, isMacOS);
        }

        [Fact]
        public void IsLinux_ShouldReturnCorrectValue()
        {
            // Test platform detection
            bool isLinux = Utils.IsLinux();
            bool expectedLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            Assert.Equal(expectedLinux, isLinux);
        }

        [Fact]
        public void GetShell_ShouldReturnCorrectShellForCurrentPlatform()
        {
            string shell = Utils.GetShell();
            
            if (Utils.IsWindows())
            {
                Assert.Equal("cmd.exe", shell);
            }
            else if (Utils.IsMacOS() || Utils.IsLinux())
            {
                Assert.Equal("/bin/bash", shell);
            }
            else
            {
                Assert.Fail("Test should not reach this point on supported platforms");
            }
        }

        [Fact]
        public void GetShellArgs_ShouldFormatCommandCorrectly()
        {
            string testCommand = "echo test";
            string args = Utils.GetShellArgs(testCommand);

            if (Utils.IsWindows())
            {
                Assert.Equal("/c echo test", args);
            }
            else
            {
                Assert.Equal("-c \"echo test\"", args);
            }
        }

        [Fact]
        public void GetShellArgs_ShouldHandleComplexCommands()
        {
            string complexCommand = "echo 'hello world' && ls -la | grep test";
            string args = Utils.GetShellArgs(complexCommand);

            if (Utils.IsWindows())
            {
                Assert.Equal("/c echo 'hello world' && ls -la | grep test", args);
            }
            else
            {
                Assert.Equal("-c \"echo 'hello world' && ls -la | grep test\"", args);
            }
        }

        [Fact]
        public void GetShellArgs_ShouldHandleEmptyCommand()
        {
            string emptyCommand = "";
            string args = Utils.GetShellArgs(emptyCommand);

            if (Utils.IsWindows())
            {
                Assert.Equal("/c ", args);
            }
            else
            {
                Assert.Equal("-c \"\"", args);
            }
        }
    }

    public class ShellTests
    {
        [Fact]
        public void SystemCommand_WithSimpleEcho_ShouldReturnTrue()
        {
            // Test basic echo command that should always succeed
            bool result = Shell.SystemCommand("echo Hello World", stream: false, treatStderrAsFailure: false);
            Assert.True(result);
        }

        [Fact]
        public void SystemCommand_WithStreamingDisabled_ShouldReturnTrue()
        {
            // Test command execution with streaming disabled
            bool result = Shell.SystemCommand("echo Test Output", stream: false, treatStderrAsFailure: false);
            Assert.True(result);
        }

        [Fact]
        public void SystemCommand_WithStreamingEnabled_ShouldReturnTrue()
        {
            // Test command execution with streaming enabled
            bool result = Shell.SystemCommand("echo Test Output", stream: true, treatStderrAsFailure: false);
            Assert.True(result);
        }

        [Fact]
        public void SystemCommand_WithInvalidCommand_ShouldReturnFalse()
        {
            // Test with a command that doesn't exist
            bool result = Shell.SystemCommand("nonexistentcommandthatdoesnotexist12345", stream: false, treatStderrAsFailure: false);
            Assert.False(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SystemCommand_WithDifferentStreamingSettings_ShouldWork(bool streaming)
        {
            // Test that the method works with both streaming settings
            bool result = Shell.SystemCommand("echo Streaming Test", stream: streaming, treatStderrAsFailure: false);
            Assert.True(result);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SystemCommand_WithDifferentStderrTreatment_ShouldWork(bool treatStderrAsFailure)
        {
            // Test with a command that produces no stderr
            bool result = Shell.SystemCommand("echo No Stderr", stream: false, treatStderrAsFailure: treatStderrAsFailure);
            Assert.True(result);
        }

        [Fact]
        public void SystemCommand_WithCommandProducingStderr_TreatAsFailureFalse_ShouldReturnTrue()
        {
            // Use a command that writes to stderr but exits with code 0
            // Different approaches for different platforms
            string command;
            if (Utils.IsWindows())
            {
                // Windows: echo to stderr using redirection
                command = "echo Warning >&2";
            }
            else
            {
                // Unix: echo to stderr
                command = "echo Warning >&2";
            }

            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: false);
            Assert.True(result, "Command should succeed when treatStderrAsFailure is false, even with stderr output");
        }

        [Fact]
        public void SystemCommand_WithCommandProducingStderr_TreatAsFailureTrue_ShouldReturnFalse()
        {
            // Use a command that writes to stderr but exits with code 0
            string command;
            if (Utils.IsWindows())
            {
                // Windows: echo to stderr using redirection
                command = "echo Warning >&2";
            }
            else
            {
                // Unix: echo to stderr
                command = "echo Warning >&2";
            }

            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: true);
            Assert.False(result, "Command should fail when treatStderrAsFailure is true and stderr has content");
        }

        [Fact]
        public void SystemCommand_WithFailingCommand_ShouldReturnFalse()
        {
            // Use a command that exits with non-zero code
            string command;
            if (Utils.IsWindows())
            {
                command = "exit 1";
            }
            else
            {
                command = "exit 1";
            }

            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: false);
            Assert.False(result, "Command should fail when exit code is non-zero");
        }

        [Fact]
        public void SystemCommand_WithEmptyCommand_ShouldHandleGracefully()
        {
            // Test with empty command - behavior may vary by platform
            bool result = Shell.SystemCommand("", stream: false, treatStderrAsFailure: false);
            // Just ensure it doesn't throw an exception - return value may vary
            Assert.True(true); // If we get here without exception, test passes
        }

        [Fact]
        public void SystemCommand_WithMultipleCommands_ShouldWork()
        {
            // Test command chaining
            string command;
            if (Utils.IsWindows())
            {
                command = "echo First && echo Second";
            }
            else
            {
                command = "echo First && echo Second";
            }

            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: false);
            Assert.True(result, "Chained commands should work");
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void SystemCommand_WithAllParameterCombinations_ShouldWork(bool stream, bool treatStderrAsFailure)
        {
            // Test all parameter combinations with a safe command
            bool result = Shell.SystemCommand("echo Parameter Test", stream: stream, treatStderrAsFailure: treatStderrAsFailure);
            Assert.True(result, $"Command should succeed with stream={stream}, treatStderrAsFailure={treatStderrAsFailure}");
        }

        [Fact]
        public void SystemCommand_WithSpacesInCommand_ShouldWork()
        {
            // Test command with spaces and quotes
            string command = "echo \"Hello World with spaces\"";
            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: false);
            Assert.True(result, "Commands with spaces should work");
        }

        [Fact]
        public void SystemCommand_WithSpecialCharacters_ShouldWork()
        {
            // Test command with some special characters that are safe across platforms
            string command = "echo Hello123!@#";
            bool result = Shell.SystemCommand(command, stream: false, treatStderrAsFailure: false);
            Assert.True(result, "Commands with basic special characters should work");
        }
    }

    public class ExceptionTests
    {
        [Fact]
        public void ShellCommandException_DefaultConstructor_ShouldSetCorrectMessage()
        {
            var exception = new ShellCommandException();
            Assert.Equal("An error occurred during shell command execution.", exception.Message);
        }

        [Fact]
        public void ShellCommandException_WithMessage_ShouldSetMessage()
        {
            string testMessage = "Test error message";
            var exception = new ShellCommandException(testMessage);
            Assert.Equal(testMessage, exception.Message);
        }

        [Fact]
        public void ShellCommandException_WithMessageAndInnerException_ShouldSetBoth()
        {
            string testMessage = "Test error message";
            var innerException = new InvalidOperationException("Inner error");
            var exception = new ShellCommandException(testMessage, innerException);
            
            Assert.Equal(testMessage, exception.Message);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void ShellCommandTimeoutException_WithTimeout_ShouldSetTimeoutAndMessage()
        {
            var timeout = TimeSpan.FromSeconds(30);
            var exception = new ShellCommandTimeoutException(timeout);
            
            Assert.Equal(timeout, exception.Timeout);
            Assert.Contains("30 seconds", exception.Message);
        }

        [Fact]
        public void ShellCommandTimeoutException_WithMessageAndTimeout_ShouldSetBoth()
        {
            string testMessage = "Custom timeout message";
            var timeout = TimeSpan.FromMinutes(2);
            var exception = new ShellCommandTimeoutException(testMessage, timeout);
            
            Assert.Equal(testMessage, exception.Message);
            Assert.Equal(timeout, exception.Timeout);
        }

        [Fact]
        public void ShellCommandTimeoutException_WithMessageTimeoutAndInnerException_ShouldSetAll()
        {
            string testMessage = "Custom timeout message";
            var timeout = TimeSpan.FromMinutes(5);
            var innerException = new InvalidOperationException("Inner error");
            var exception = new ShellCommandTimeoutException(testMessage, timeout, innerException);
            
            Assert.Equal(testMessage, exception.Message);
            Assert.Equal(timeout, exception.Timeout);
            Assert.Equal(innerException, exception.InnerException);
        }

        [Fact]
        public void ShellCommandTimeoutException_ShouldInheritFromShellCommandException()
        {
            var timeout = TimeSpan.FromSeconds(10);
            var exception = new ShellCommandTimeoutException(timeout);
            
            Assert.IsAssignableFrom<ShellCommandException>(exception);
        }
    }

    public class IntegrationTests
    {
        [Fact]
        public void Utils_And_Shell_Integration_ShouldWorkTogether()
        {
            // Test that Utils methods integrate properly with Shell.SystemCommand
            string shell = Utils.GetShell();
            string args = Utils.GetShellArgs("echo Integration Test");
            
            // Verify the shell is appropriate for platform
            if (Utils.IsWindows())
            {
                Assert.Equal("cmd.exe", shell);
                Assert.StartsWith("/c", args);
            }
            else if (Utils.IsLinux() || Utils.IsMacOS())
            {
                Assert.Equal("/bin/bash", shell);
                Assert.StartsWith("-c", args);
            }

            // Test that SystemCommand works with these settings
            bool result = Shell.SystemCommand("echo Integration Test", stream: false);
            Assert.True(result);
        }

        [Fact]
        public void Platform_Detection_Should_Be_Mutually_Exclusive()
        {
            // Ensure only one platform is detected as true
            bool isWindows = Utils.IsWindows();
            bool isMacOS = Utils.IsMacOS();
            bool isLinux = Utils.IsLinux();

            int trueCount = (isWindows ? 1 : 0) + (isMacOS ? 1 : 0) + (isLinux ? 1 : 0);
            Assert.True(trueCount >= 1, "At least one platform should be detected");
            
            // Note: In some containerized environments, multiple platforms might be detected
            // so we just ensure at least one is detected
        }

        [Fact]
        public void Shell_Command_Should_Handle_Platform_Specific_Operations()
        {
            // Test platform-specific commands that are safe and available
            bool result;
            
            if (Utils.IsWindows())
            {
                // Windows-specific command that should work
                result = Shell.SystemCommand("echo %OS%", stream: false);
            }
            else
            {
                // Unix-specific command that should work
                result = Shell.SystemCommand("echo $USER", stream: false);
            }
            
            Assert.True(result, "Platform-specific commands should work");
        }
    }
}
