using System.Runtime.InteropServices;

namespace CollinExecute
{
    static class Utils
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    public class CollinExecute
    {
        static bool Shell(string command)
        {

        }
    }
}
