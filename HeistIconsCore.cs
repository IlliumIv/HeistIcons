using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.AtlasHelper;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using HeistIcons.Libs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HeistIcons
{
    public partial class HeistIconsCore : BaseSettingsPlugin<HeistIconsSettings>
    {
        private static LargeMapData LargeMapInformation { get; set; }

        public override bool Initialise()
        {
            Name = "Heist Icons";

            return base.Initialise();
        }

        public override void Render()
        {
            var entities = GameController.EntityListWrapper.OnlyValidEntities;

            foreach (var e in entities)
            {
                try
                {
                    if (e == null)
                        continue;

                    if (e.League != LeagueType.Heist)
                        continue;

                    /*
                    if (entity.Type != EntityType.Monster || entity.Type != EntityType.Chest)
                        continue;
                    */    

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
                        if (GameController.Game.IngameState.IngameUi.Map.LargeMap.IsVisible)
                        {
                            LargeMapInformation = new LargeMapData(GameController);
                            if (e.HasComponent<Render>() && e.HasComponent<Positioned>())
                                DrawToLargeMiniMap(e);
                        }
                        else if (GameController.Game.IngameState.IngameUi.Map.SmallMiniMap.IsVisible)
                            if (e.HasComponent<Render>() && e.HasComponent<Positioned>())
                                DrawToSmallMiniMap(e);

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

        private void DrawToLargeMiniMap(Entity e)
        {
            var icon = GetMapIcon(e);

            if (icon == null)
                return;

            var iconZ = e.GetComponent<Render>().Z;
            var point = LargeMapInformation.ScreenCenter
                        + MapIcon.DeltaInWorldToMinimapDelta(e.GetComponent<Positioned>().GridPos - LargeMapInformation.PlayerPos,
                            LargeMapInformation.Diag, LargeMapInformation.Scale,
                            (iconZ - LargeMapInformation.PlayerPosZ) /
                            (9f / LargeMapInformation.MapWindow.LargeMapZoom));

            var size = icon.Size;
            Graphics.DrawImage(icon.Texture, new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size), icon.Color);
        }

        private void DrawToSmallMiniMap(Entity e)
        {
            var icon = GetMapIcon(e);

            if (icon == null)
                return;

            var smallMinimap = GameController.Game.IngameState.IngameUi.Map.SmallMiniMap;
            var playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            var posZ = GameController.Player.GetComponent<Render>().Z;
            const float scale = 240f;
            var mapRect = smallMinimap.GetClientRect();
            var mapCenter = new Vector2(mapRect.X + mapRect.Width / 2, mapRect.Y + mapRect.Height / 2).Translate(0, 0);
            var diag = Math.Sqrt(mapRect.Width * mapRect.Width + mapRect.Height * mapRect.Height) / 2.0;
            var iconZ = e.GetComponent<Render>().Z;
            var point = mapCenter + MapIcon.DeltaInWorldToMinimapDelta(e.GetComponent<Positioned>().GridPos - playerPos, diag, scale, (iconZ - posZ) / 20);

            var size = icon.Size;
            var rect = new RectangleF(point.X - size / 2f, point.Y - size / 2f, size, size);

            mapRect.Contains(ref rect, out var isContain);

            if (isContain)
            {
                Graphics.DrawImage(icon.Texture, rect, icon.Color);
            }
        }
    }
}
