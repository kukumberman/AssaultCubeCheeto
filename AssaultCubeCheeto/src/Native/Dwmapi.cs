using System;
using System.Runtime.InteropServices;
using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.Native
{
    static class Dwmapi
    {
        private const string DLL_NAME = "dwmapi.dll";

        [DllImport(DLL_NAME)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);
    }
}
