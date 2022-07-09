using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct Rect
    {
        public int Left, Top, Right, Bottom;

        public int Width => Right - Left;
        public int Height => Bottom - Top;

        public override string ToString()
        {
            return $"{Left} {Top} {Width} {Height}";
        }
    }
}
