# CollinExecute

A cross-platform .NET library for executing shell commands with advanced features like real-time output streaming, customizable error handling, and automatic platform detection.

## Features

- **Cross-Platform Support**: Automatically detects and uses the appropriate shell for Windows (cmd.exe), macOS, and Linux (/bin/bash)
- **Real-time Output Streaming**: Option to stream command output to console as it arrives
- **Flexible Error Handling**: Configure whether stderr output should be treated as failure
- **Simple API**: Easy-to-use static methods for common shell operations
- **Comprehensive Exception Handling**: Custom exceptions for better error management

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package CollinExecute
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package CollinExecute
```

## Quick Start

```csharp
using CollinExecute;

// Execute a simple command with output streaming
bool success = Shell.SystemCommand("echo Hello World");

// Execute a command silently
bool success = Shell.SystemCommand("git status", stream: false);

// Treat stderr as failure condition
bool success = Shell.SystemCommand("npm build", treatStderrAsFailure: true);
```

## API Reference

### Utils Class

The `Utils` class provides utility methods for platform detection and shell configuration.

#### Platform Detection Methods

- `bool IsWindows()` - Returns true if running on Windows
- `bool IsMacOS()` - Returns true if running on macOS  
- `bool IsLinux()` - Returns true if running on Linux

#### Shell Configuration Methods

- `string GetShell()` - Returns the appropriate shell executable for the current platform
- `string GetShellArgs(string command)` - Returns properly formatted shell arguments for the given command

### Shell Class

The `Shell` class provides methods for executing shell commands.

#### SystemCommand Method

```csharp
public static bool SystemCommand(string command, bool stream = true, bool treatStderrAsFailure = false)
```

**Parameters:**
- `command` (string): The command to execute
- `stream` (bool, optional): Whether to stream output to console in real-time (default: true)
- `treatStderrAsFailure` (bool, optional): Whether stderr output should cause failure even with exit code 0 (default: false)

**Returns:**
- `true` if command executed successfully
- `false` if command failed or conditions weren't met

## Usage Examples

### Basic Command Execution

```csharp
// Simple command execution
bool result = Shell.SystemCommand("dir"); // Windows
bool result = Shell.SystemCommand("ls -la"); // Unix-like systems
```

### Silent Execution

```csharp
// Execute without streaming output to console
bool result = Shell.SystemCommand("git clone https://github.com/user/repo.git", stream: false);
```

### Strict Error Handling

```csharp
// Treat any stderr output as failure
bool result = Shell.SystemCommand("npm install", treatStderrAsFailure: true);
```

### Platform-Specific Operations

```csharp
// Check platform and execute appropriate commands
if (Utils.IsWindows())
{
    Shell.SystemCommand("dir /s");
}
else if (Utils.IsLinux() || Utils.IsMacOS())
{
    Shell.SystemCommand("find . -type f");
}
```

### Advanced Usage with Error Handling

```csharp
try
{
    string shell = Utils.GetShell();
    string args = Utils.GetShellArgs("complex command with | pipes && operators");
    
    bool success = Shell.SystemCommand("complex command with | pipes && operators", 
                                     stream: true, 
                                     treatStderrAsFailure: false);
    
    if (!success)
    {
        Console.WriteLine("Command execution failed");
    }
}
catch (PlatformNotSupportedException ex)
{
    Console.WriteLine($"Platform not supported: {ex.Message}");
}
```

## Exception Handling

The library provides custom exceptions for better error management:

- `ShellCommandException`: Base exception for shell command errors
- `ShellCommandTimeoutException`: Thrown when command execution times out (for future timeout features)

## Platform Support

| Platform | Shell | Command Format |
|----------|-------|----------------|
| Windows  | cmd.exe | `/c {command}` |
| macOS    | /bin/bash | `-c "{command}"` |
| Linux    | /bin/bash | `-c "{command}"` |

## Requirements

- .NET 8.0 or later
- Supported platforms: Windows, macOS, Linux

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## License

This project is licensed under the terms specified in the [License.md](License.md) file.

## Authors

- Collin Software

---

*CollinExecute - Making cross-platform shell command execution simple and reliable.*