using System;
using ExileCore.Shared.AtlasHelper;
using SharpDX;

namespace HeistIcons.Libs
{
    public class WorldIcon : MapIcon
    {
        public WorldIcon(AtlasTexture texture, float size, Color color)
            : base(texture, size, color) { }

        public WorldIcon(AtlasTexture texture, float size)
            : base(texture, size) { }
    }
}
