using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public class Margins
    {
        public int Left, Right, Top, Bottom;
    }
}
