using Cucumba.Cheeto.Core;
using System.Runtime.InteropServices;

namespace Cucumba.Cheeto.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    struct Vector2
    {
        public float X, Y;

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"{X:0.00} {Y:0.00}";
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            Vector2 diff = a - b;
            return Mathf.Sqrt(diff.X * diff.X + diff.Y * diff.Y);
        }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X + rhs.X, lhs.X + rhs.Y);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector2 operator *(Vector2 lhs, float rhs)
        {
            return new Vector2(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vector2 operator *(float lhs, Vector2 rhs)
        {
            return new Vector2(rhs.X * lhs, rhs.Y * lhs);
        }
    }
}
