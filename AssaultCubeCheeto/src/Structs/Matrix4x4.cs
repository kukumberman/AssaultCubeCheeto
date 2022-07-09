namespace Cucumba.Cheeto.Structs
{
    class Matrix4x4
    {
        public readonly float[] Values = new float[16];

        public float this[int index] => Values[index];

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder(128);
            for (int i = 0; i < 4; i++)
            {
                var j = i * 4;
                sb.AppendLine($"{Values[j + 0]:0.00} {Values[j + 1]:0.00} {Values[j + 2]:0.00} {Values[j + 3]:0.00}");
            }
            return sb.ToString();
        }
    }
}