using System;
using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Native
{
    static class Kernel32
    {
        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_VM_OPERATION = 0x8;
        public const int PROCESS_VM_READ = 0x10;
        public const int PROCESS_VM_WRITE = 0x20;

        private const string DLL_NAME = "kernel32.dll";

        [DllImport(DLL_NAME)]
        public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport(DLL_NAME)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport(DLL_NAME)]
        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out int lpNumberOfBytesRead
        );

        [DllImport(DLL_NAME)]
        public static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out int lpNumberOfBytesWritten
        );
    }
}
