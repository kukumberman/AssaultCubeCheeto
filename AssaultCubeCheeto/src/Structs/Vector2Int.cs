using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vector2Int
    {
        public int X, Y;

        public override string ToString()
        {
            return $"{X} {Y}";
        }
    }
}
