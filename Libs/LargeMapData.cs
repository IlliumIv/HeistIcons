using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Helpers;
using SharpDX;
using System;

namespace HeistIcons.Libs
{
    public class LargeMapData
    {
        public Camera @Camera { get; set; }
        public ExileCore.PoEMemory.Elements.Map @MapWindow { get; set; }
        public RectangleF @MapRec { get; set; }
        public Vector2 @PlayerPos { get; set; }
        public float @PlayerPosZ { get; set; }
        public Vector2 @ScreenCenter { get; set; }
        public float @Diag { get; set; }
        public float @K { get; set; }
        public float @Scale { get; set; }


        public LargeMapData(GameController GC)
        {
            @Camera = GC.Game.IngameState.Camera;
            @MapWindow = GC.Game.IngameState.IngameUi.Map;
            @MapRec = @MapWindow.GetClientRect();
            @PlayerPos = GC.Player.GetComponent<Positioned>().GridPos;
            @PlayerPosZ = GC.Player.GetComponent<Render>().Z;
            @ScreenCenter = new Vector2(@MapRec.Width / 2, @MapRec.Height / 2).Translate(0, -20)
                               + new Vector2(@MapRec.X, @MapRec.Y)
                               + new Vector2(@MapWindow.LargeMapShiftX, @MapWindow.LargeMapShiftY);
            @Diag = (float)Math.Sqrt(@Camera.Width * @Camera.Width + @Camera.Height * @Camera.Height);
            @K = @Camera.Width < 1024f ? 1120f : 1024f;
            @Scale = @K / @Camera.Height * @Camera.Width * 3f / 4f / @MapWindow.LargeMapZoom;
        }
    }
}
