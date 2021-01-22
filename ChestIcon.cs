using System;
using ExileCore.Shared.AtlasHelper;
using SharpDX;

namespace HeistIcons
{
    public class ChestIcon
    {
        public AtlasTexture Texture;
        public Color Color = Color.White;
        public float Size;

        public ChestIcon(AtlasTexture texture, float size, Color color)
            : this(texture, size)
        {
            Color = color;
        }

        public ChestIcon(AtlasTexture texture, float size)
        {
            Texture = texture;
            Size = size;
        }

        public static Vector2 DeltaInWorldToMinimapDelta(Vector2 delta, double diag, float scale, float deltaZ = 0)
        {
            const float CAMERA_ANGLE = 38 * MathUtil.Pi / 180;

            // Values according to 40 degree rotation of cartesian coordiantes, still doesn't seem right but closer
            var cos = (float)(diag * Math.Cos(CAMERA_ANGLE) / scale);
            var sin = (float)(diag * Math.Sin(CAMERA_ANGLE) / scale); // possible to use cos so angle = nearly 45 degrees

            // 2D rotation formulas not correct, but it's what appears to work?
            return new Vector2((delta.X - delta.Y) * cos, deltaZ - (delta.X + delta.Y) * sin);
        }
    }
}
