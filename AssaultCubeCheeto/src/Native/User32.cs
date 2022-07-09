using System;
using System.Runtime.InteropServices;
using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.Native
{
    static class User32
    {
        public const int GWL_EXSTYLE = -20;

        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;

        public const int VK_SPACE = 0x20;
        public const int VK_END = 0x23;
        public const int VK_NUMPAD5 = 0x65;
        public const int VK_KEY_F = 0x46;
        public const int VK_KEY_Q = 0x51;

        private const string DLL_NAME = "user32.dll";

        [DllImport(DLL_NAME)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport(DLL_NAME)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport(DLL_NAME)]
        public static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

        [DllImport(DLL_NAME)]
        public static extern bool ClientToScreen(IntPtr hWnd, out Point lpPoint);
        
        [DllImport(DLL_NAME)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport(DLL_NAME)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport(DLL_NAME)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport(DLL_NAME)]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, int dwFlags);

        [DllImport(DLL_NAME)]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport(DLL_NAME)]
        public static extern int GetAsyncKeyState(int vKey);
    }
}
