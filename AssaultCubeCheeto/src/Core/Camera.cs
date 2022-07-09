using Cucumba.Cheeto.Structs;

namespace Cucumba.Cheeto.Core
{
    class Camera
    {
        public Vector2Int ScreenSize;
        public Matrix4x4 ViewMatrix;

        public Camera()
        {
            ViewMatrix = new Matrix4x4();
        }

        public bool WorldToScreen(Vector3 worldPosition, out Vector2 screenPosition)
        {
            return WorldToScreen(ViewMatrix, worldPosition, ScreenSize, out screenPosition);
        }

        public static bool WorldToScreen(Matrix4x4 matrix, Vector3 worldPos, Vector2Int screenSize, out Vector2 screenPos)
        {
            screenPos.X = 0;
            screenPos.Y = 0;

            var w = matrix[3] * worldPos.X + matrix[7] * worldPos.Y + matrix[11] * worldPos.Z + matrix[15];

            if (w < 0.1f)
            {
                return false;
            }

            Vector2 clipCoords;
            clipCoords.X = matrix[0] * worldPos.X + matrix[4] * worldPos.Y + matrix[8] * worldPos.Z + matrix[12];
            clipCoords.Y = matrix[1] * worldPos.X + matrix[5] * worldPos.Y + matrix[9] * worldPos.Z + matrix[13];

            Vector2 viewportPos;
            viewportPos.X = 0.5f * (1f + clipCoords.X / w);
            viewportPos.Y = 0.5f * (1f - clipCoords.Y / w);

            screenPos.X = viewportPos.X * screenSize.X;
            screenPos.Y = viewportPos.Y * screenSize.Y;

            return true;
        }
    }
}