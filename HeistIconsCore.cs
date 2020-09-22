using ExileCore;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using HeistIcons.Libs;
using SharpDX;
using System;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Components;

namespace HeistIcons
{
    public partial class HeistIconsCore : BaseSettingsPlugin<HeistIconsSettings>
    {
        private IngameUIElements ingameStateIngameUi;
        private float k;
        private bool largeMap;
        private float scale;
        private Vector2 screentCenterCache;

        private CachedValue<RectangleF> _mapRect;
        private CachedValue<float> _diag;

        private ExileCore.PoEMemory.Elements.Map MapWindow => GameController.Game.IngameState.IngameUi.Map;
        private RectangleF MapRect => _mapRect?.Value ?? (_mapRect = new TimeCache<RectangleF>(() => MapWindow.GetClientRect(), 100)).Value;
        private Camera Camera => GameController.Game.IngameState.Camera;
        private float Diag =>
            _diag?.Value ?? (_diag = new TimeCache<float>(() =>
            {
                if (ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
                {
                    var mapRect = ingameStateIngameUi.Map.SmallMiniMap.GetClientRect();
                    return (float)(Math.Sqrt(mapRect.Width * mapRect.Width + mapRect.Height * mapRect.Height) / 2f);
                }

                return (float)Math.Sqrt(Camera.Width * Camera.Width + Camera.Height * Camera.Height);
            }, 100)).Value;
        private Vector2 ScreenCenter =>
            new Vector2(MapRect.Width / 2, MapRect.Height / 2 - 20) + new Vector2(MapRect.X, MapRect.Y) +
            new Vector2(MapWindow.LargeMapShiftX, MapWindow.LargeMapShiftY);

        public override bool Initialise()
        {
            Name = "Heist Icons";

            return base.Initialise();
        }

        public override Job Tick()
        {
            TickLogic();
            return null;
        }

        private void TickLogic()
        {
            ingameStateIngameUi = GameController.Game.IngameState.IngameUi;

            if (ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
            {
                var mapRect = ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                screentCenterCache = new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2);
                largeMap = false;
            }
            else if (ingameStateIngameUi.Map.LargeMap.IsVisibleLocal)
            {
                screentCenterCache = ScreenCenter;
                largeMap = true;
            }

            k = Camera.Width < 1024f ? 1120f : 1024f;
            scale = k / Camera.Height * Camera.Width * 3.06f / 4f / MapWindow.LargeMapZoom;
        }

        public override void Render()
        {

            var entities = GameController.EntityListWrapper.OnlyValidEntities;
            var playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            var posZ = GameController.Player.GetComponent<Render>().Pos.Z;
            var mapWindowLargeMapZoom = MapWindow.LargeMapZoom;

            foreach (var e in entities)
            {
                try
                {

                    if (e == null)
                        continue;

                    if (e.League != LeagueType.Heist)
                        continue;

                    if (e.Type == EntityType.Monster && e.Rarity != MonsterRarity.Unique)
                        continue;

                    if (e.Type == EntityType.Chest && e.IsOpened)
                        continue;

                    if (!e.IsHostile)
                        continue;

                    string renderName = e.Path
                        .Replace("Metadata/Chests/LeagueHeist/HeistChest", "")

                        .Replace("Military", "")
                        .Replace("Thug", "")
                        .Replace("Science", "")
                        .Replace("Robot", "")

                        .Replace("Secondary", "");

                    if (!renderName.Contains("RewardRoom"))
                    {
                        var component = e?.GetComponent<Render>();
                        if (component == null) continue;

                        var icon = GetMapIcon(e);
                        if (icon == null) continue;

                        var size = icon.Size * (1 + mapWindowLargeMapZoom);
                        var iconZ = component.Pos.Z;
                        Vector2 position;

                        if (largeMap)
                        {
                            position = screentCenterCache + MapIcon.DeltaInWorldToMinimapDelta(
                                e.GetComponent<Positioned>().GridPos - playerPos, Diag, scale, (iconZ - posZ) / (9f / mapWindowLargeMapZoom));
                            
                        }
                        else
                            position = screentCenterCache + MapIcon.DeltaInWorldToMinimapDelta(
                                e.GetComponent<Positioned>().GridPos - playerPos, Diag, 240f, (iconZ - posZ) / 20);


                        Graphics.DrawImage(icon.Texture, new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size), icon.Color);
                    }

                    renderName = renderName.Replace("RewardRoom", "")
                        .Replace("LockPicking", "")
                        .Replace("BruteForce", "")
                        .Replace("Perception", "")
                        .Replace("Demolition", "")
                        .Replace("CounterThaumaturge", "")
                        .Replace("TrapDisarmament", "")
                        .Replace("Agility", "")
                        .Replace("Deception", "")
                        .Replace("Engineering", "");

                    renderName = string.Join("", renderName.ToCharArray().Select(x => char.IsUpper(x) ? " " + x : "" + x).ToList());

                    var camera = GameController.IngameState.Camera;
                    var worldtoscreen = camera.WorldToScreen(e.Pos);
                    var textBox = Graphics.MeasureText(renderName);
                    var backgroundBox = new System.Numerics.Vector2(textBox.X * (float)1.2, textBox.Y * (float)2);
                    var rectangle = new RectangleF(worldtoscreen.X - backgroundBox.X / 2, worldtoscreen.Y - (backgroundBox.Y - textBox.Y) / 2, backgroundBox.X, backgroundBox.Y);

                    Graphics.DrawText(renderName, worldtoscreen.ToVector2Num(), Settings.TextColor.Value, 22, "Default:13", FontAlign.Center);
                    Graphics.DrawBox(rectangle, Settings.TextBackgroundColor.Value);
                    Graphics.DrawFrame(rectangle, Settings.TextBorderColor.Value, 1);

                }
                catch (Exception ex)
                {
                    LogError($"{Name}: {ex.Message}");
                }
            }

            base.Render();
        }

        public MapIcon GetMapIcon(Entity e)
        {
            if (e.Path.Contains("Monster")) { return new MapIcon(GetAtlasTexture("HeistSpottedMiniBoss"), Settings.IconSize.Value * (float)0.8); }
            if (e.Path.Contains("Smugglers")) { return new MapIcon(GetAtlasTexture("HeistSumgglersCache"), Settings.IconSize.Value); }
            if (e.Path.Contains("Safe")) { return new MapIcon(GetAtlasTexture("HeistPathChest"), Settings.IconSize.Value); }
            if (e.Path.Contains("Currency")) { return new MapIcon(GetAtlasTexture("RewardCurrency"), Settings.IconSize.Value); }
            if (e.Path.Contains("Armour")) { return new MapIcon(GetAtlasTexture("RewardArmour"), Settings.IconSize.Value); }
            if (e.Path.Contains("Weapons")) { return new MapIcon(GetAtlasTexture("RewardWeapons"), Settings.IconSize.Value); }
            if (e.Path.Contains("Jewellery")) { return new MapIcon(GetAtlasTexture("RewardJewellery"), Settings.IconSize.Value); }
            if (e.Path.Contains("Jewels")) { return new MapIcon(GetAtlasTexture("Jewel"), Settings.IconSize.Value); }
            if (e.Path.Contains("Maps")) { return new MapIcon(GetAtlasTexture("RewardMaps"), Settings.IconSize.Value); }
            if (e.Path.Contains("DivinationCards")) { return new MapIcon(GetAtlasTexture("RewardDivinationCards"), Settings.IconSize.Value); }
            if (e.Path.Contains("StackedDecks")) { return new MapIcon(GetAtlasTexture("StackedDecks"), Settings.IconSize.Value); }
            if (e.Path.Contains("Gems")) { return new MapIcon(GetAtlasTexture("RewardGems"), Settings.IconSize.Value); }
            if (e.Path.Contains("Corrupted")) { return new MapIcon(GetAtlasTexture("Corruption"), Settings.IconSize.Value); }
            if (e.Path.Contains("Uniques")) { return new MapIcon(GetAtlasTexture("RewardUniques"), Settings.IconSize.Value); }
            if (e.Path.Contains("Prophecies")) { return new MapIcon(GetAtlasTexture("RewardProphecy"), Settings.IconSize.Value); }
            if (e.Path.Contains("Essences")) { return new MapIcon(GetAtlasTexture("RewardEssences"), Settings.IconSize.Value); }

            return null;
        }
    }
}
