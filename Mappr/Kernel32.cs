using System.Runtime.InteropServices;

namespace Mappr
{
    public static class Kernel32
    {
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_WM_READ = 0x0010;
        public const int PROCESS_WM_WRITE = 0x0020;
        public const int PROCESS_WM_OPERATION = 0x0008;


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern nint OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(nint hProcess, nint lpBaseAddress, byte[] lpBuffer, int nSize, out nint lpNumberOfBytesRead);

    }

}
