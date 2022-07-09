using System;
using System.Runtime.InteropServices;
using Cucumba.Cheeto.Native;

namespace Cucumba.Cheeto.Core
{
    class MemoryContext : IDisposable
    {
        public static MemoryContext Instance = null;

        private readonly IntPtr _handle = IntPtr.Zero;

        public MemoryContext(IntPtr handle)
        {
            _handle = handle;
        }

        public void Dispose()
        {
            Kernel32.CloseHandle(_handle);
        }

        public IntPtr ReadPointer(IntPtr address)
        {
            return Read<IntPtr>(_handle, address);
        }

        public string ReadStringASCII(IntPtr address, int size)
        {
            var bytes = ReadBytes(address, size);
            return System.Text.Encoding.ASCII.GetString(bytes).Trim('\0');
        }

        public T Read<T>(ulong address) where T : unmanaged
        {
            return Read<T>((IntPtr)address);
        }

        public T Read<T>(IntPtr address) where T : unmanaged
        {
            return Read<T>(_handle, address);
        }

        public byte[] ReadBytes(IntPtr address, int size)
        {
            var bytes = new byte[size];
            Kernel32.ReadProcessMemory(_handle, address, bytes, size, out var bytesRead);
            return bytes;
        }

        public bool Write<T>(IntPtr address, T value) where T : unmanaged
        {
            return Write(_handle, address, value);
        }

        public bool WriteBytes(IntPtr address, byte[] bytes)
        {
            return Kernel32.WriteProcessMemory(_handle, address, bytes, bytes.Length, out var bytesWritten);
        }

        public bool NopBytes(IntPtr address, int size)
        {
            const byte NOP = 0x90;

            var bytes = new byte[size];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = NOP;
            }
            return WriteBytes(address, bytes);
        }

        public static T Read<T>(IntPtr hProcess, IntPtr lpBaseAddress) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var buffer = (object)default(T);
            Kernel32.ReadProcessMemory(hProcess, lpBaseAddress, buffer, size, out var bytesRead);
            return bytesRead == size ? (T)buffer : default;
        }

        public static bool Write<T>(IntPtr hProcess, IntPtr lpBaseAddress, T value) where T : unmanaged
        {
            var size = Marshal.SizeOf<T>();
            var bytes = new byte[size];
            var buffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, buffer, false);
            Marshal.Copy(buffer, bytes, 0, size);
            Marshal.FreeHGlobal(buffer);
            return Kernel32.WriteProcessMemory(hProcess, lpBaseAddress, bytes, bytes.Length, out var bytesWritten);
        }
    }
}
