using ExileCore;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using SharpDX;
using System;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Components;
using System.Collections.Generic;
using HeistIcons.Enums;

namespace HeistIcons.Main
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        public static Core Plugin { get; private set; }

        private readonly Queue<Entity> EntityAddedQueue = new Queue<Entity>();

        #region CameraExtension
        private IngameUIElements _ingameStateIngameUi;
        private float k;
        private float scale;
        private float playerPosZ;
        private bool largeMap;
        private Vector2 screentCenterCache;
        private Vector2 playerPos;
        private CachedValue<RectangleF> mapRectangle;
        private CachedValue<float> diag;

        private ExileCore.PoEMemory.Elements.Map MapWindow => GameController.Game.IngameState.IngameUi.Map;
        private RectangleF MapRectangle => mapRectangle?.Value ?? (mapRectangle = new TimeCache<RectangleF>(() => MapWindow.GetClientRect(), 100)).Value;
        private Camera Camera => GameController.Game.IngameState.Camera;
        private float Diag =>
            diag?.Value ?? (diag = new TimeCache<float>(() =>
            {
                if (_ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
                {
                    var mapRectangle = _ingameStateIngameUi.Map.SmallMiniMap.GetClientRect();
                    return (float)(Math.Sqrt(mapRectangle.Width * mapRectangle.Width + mapRectangle.Height * mapRectangle.Height) / 2f);
                }

                return (float)Math.Sqrt(Camera.Width * Camera.Width + Camera.Height * Camera.Height);
            }, 100)).Value;

        private Vector2 ScreenCenter =>
            new Vector2(MapRectangle.Width / 2, MapRectangle.Height / 2 - 20) + new Vector2(MapRectangle.X, MapRectangle.Y) +
            new Vector2(MapWindow.LargeMapShiftX, MapWindow.LargeMapShiftY);
        #endregion

        public override void OnLoad()
        {
            CanUseMultiThreading = true;
        }

        public override bool Initialise()
        {
            Name = "Heist Icons";
            Plugin = this;

            return true;
        }

        public override void EntityAdded(Entity e)
        {
            if (!Settings.Enable.Value) return;

            if (e == null) return;

            if (e.Type != EntityType.Chest) return;
            if (e.IsOpened) return;

            if (!e.Path.Contains("Heist")) return;
            // if (e.Path.Contains("RewardRoom")) return;

            EntityAddedQueue.Enqueue(e);
        }

        public override void AreaChange(AreaInstance area)
        {
            EntityAddedQueue.Clear();
        }

        public override Job Tick()
        {
            if (Settings.MultiThreading)
                return GameController.MultiThreadManager.AddJob(TickLogic, Name);

            TickLogic();

            return null;
        }

        private void TickLogic()
        {
            _ingameStateIngameUi = GameController.Game.IngameState.IngameUi;

            if (_ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
            {
                var mapRectangle = _ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                screentCenterCache = new Vector2(mapRectangle.X + mapRectangle.Width / 2, mapRectangle.Y + mapRectangle.Height / 2);
                largeMap = false;
            }
            else if (_ingameStateIngameUi.Map.LargeMap.IsVisibleLocal)
            {
                screentCenterCache = ScreenCenter;
                largeMap = true;
            }

            k = Camera.Width < 1024f ? 1120f : 1024f;
            scale = k / Camera.Height * Camera.Width * 3.06f / 4f / MapWindow.LargeMapZoom;
            playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            playerPosZ = GameController.Player.GetComponent<Render>().Pos.Z;

            while (EntityAddedQueue.Count > 0)
            {
                var e = EntityAddedQueue.Dequeue();
                e.SetHudComponent(new HeistChest(e));
            }

            foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Chest])
            {
                var drawCmd = e.GetHudComponent<HeistChest>();
                drawCmd?.Update();
            }
        }

        public override void Render()
        {
            var mapWindowLargeMapZoom = MapWindow.LargeMapZoom;

            foreach (var e in GameController.EntityListWrapper.ValidEntitiesByType[EntityType.Chest])
            {
                var renderComponent = e?.GetComponent<Render>();
                if (renderComponent == null) continue;

                var heistChestComponent = e?.GetHudComponent<HeistChest>();
                if (heistChestComponent == null) continue;

                var positionedComponent = e?.GetComponent<Positioned>();
                if (positionedComponent == null) continue;

                if (!heistChestComponent.IsClosed) continue;

                if (heistChestComponent.MapIcon != null && heistChestComponent.Type == HeistChestTypes.Normal)
                {
                    var size = heistChestComponent.MapIcon.Size * (1 + mapWindowLargeMapZoom);
                    Vector2 position;

                    if (largeMap)
                    {
                        position = screentCenterCache + ChestIcon.DeltaInWorldToMinimapDelta(
                            positionedComponent.GridPos - playerPos, Diag, scale,
                            (renderComponent.Pos.Z - playerPosZ) / (9f / mapWindowLargeMapZoom));

                        Graphics.DrawImage(
                            heistChestComponent.MapIcon.Texture,
                            new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size),
                            heistChestComponent.MapIcon.Color);
                    }
                    else
                    {
                        position = screentCenterCache + ChestIcon.DeltaInWorldToMinimapDelta(
                            positionedComponent.GridPos - playerPos,
                            Diag, 240f, (renderComponent.Pos.Z - playerPosZ) / 20);

                        var mapRectangle = _ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                        var rectangle = new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size);

                        mapRectangle.Contains(ref rectangle, out var isContain);

                        if (isContain)
                            Graphics.DrawImage(
                                heistChestComponent.MapIcon.Texture,
                                new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size),
                                heistChestComponent.MapIcon.Color);
                    }
                }

                if (Settings.WorldIcon.Value && heistChestComponent.WorldIcon != null)
                {
                    var worldtoscreen = heistChestComponent.Type == HeistChestTypes.Normal ?
                        Camera.WorldToScreen(e.Pos.Translate(0, 0, -150)) : Camera.WorldToScreen(e.Pos.Translate(0, 0, -50));

                    if (worldtoscreen == new Vector2()) continue;

                    var size = heistChestComponent.WorldIcon.Size;

                    Graphics.DrawImage(
                        heistChestComponent.WorldIcon.Texture,
                        new RectangleF(worldtoscreen.X - size / 2f, worldtoscreen.Y - size / 2f, size, size),
                        heistChestComponent.WorldIcon.Color);
                }

                if (Settings.TextEnable.Value)
                {
                    var textBox = Graphics.MeasureText(heistChestComponent.Name);
                    System.Numerics.Vector2 backgroundBox;
                    backgroundBox.X = textBox.X + 2;
                    backgroundBox.Y = textBox.Y * 2f;

                    var rectangleHeight = backgroundBox.Y;
                    var worldtoscreen = Camera.WorldToScreen(e.Pos);

                    if (Settings.UseDefaultText)
                        Graphics.DrawText(
                            heistChestComponent.Name, worldtoscreen.ToVector2Num(),
                            Settings.TextColor.Value, 22, "Default:13", FontAlign.Center);
                    else
                    {
                        Graphics.DrawText(
                            heistChestComponent.Name, worldtoscreen.ToVector2Num(),
                            Settings.TextColor.Value, 22, FontAlign.Center);

                        backgroundBox.X *= Settings.BackgroundWidth;
                        rectangleHeight *= Settings.BackgroundHeight;
                    }

                    var rectangle = new RectangleF(worldtoscreen.X - backgroundBox.X / 2, worldtoscreen.Y - (backgroundBox.Y - textBox.Y) / 2, backgroundBox.X, rectangleHeight);

                    Graphics.DrawBox(rectangle, Settings.TextBackgroundColor.Value);
                    Graphics.DrawFrame(rectangle, Settings.TextBorderColor.Value, 1);
                }
            }
        }
    }
}
