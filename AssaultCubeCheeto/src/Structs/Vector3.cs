using Cucumba.Cheeto.Core;
using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vector3
    {
        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString()
        {
            return $"{X:0.00} {Y:0.00} {Z:0.00}";
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3 operator *(Vector3 lhs, float rhs)
        {
            return new Vector3(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs);
        }

        public float Magnitude => Mathf.Sqrt(X * X + Y * Y + Z * Z);
    }
}
