using System;

namespace Cucumba.Cheeto.Core
{
    public static class Mathf
    {
        public const float PI = (float)Math.PI;

        public const float Rad2Deg = 180 / PI;

        public const float Deg2Rad = PI / 180;

        public static float Sin(float value)
        {
            return (float)Math.Sin(value);
        }

        public static float Cos(float value)
        {
            return (float)Math.Cos(value);
        }

        public static float Sqrt(float value)
        {
            return (float)Math.Sqrt(value);
        }

        public static float Asin(float value)
        {
            return (float)Math.Asin(value);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }
    }
}
