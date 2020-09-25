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
using System.Collections.Generic;

namespace HeistIcons
{
    public partial class HeistIconsCore : BaseSettingsPlugin<HeistIconsSettings>
    {
        private IngameUIElements ingameStateIngameUi;
        private float k;
        private bool largeMap;
        private float scale;
        private Vector2 screentCenterCache;
        private Vector2 playerPos;
        private float posZ;
        private Queue<Entity> Entities = new Queue<Entity>(128);

        private CachedValue<RectangleF> _mapRectangle;
        private CachedValue<float> _diag;

        private ExileCore.PoEMemory.Elements.Map MapWindow => GameController.Game.IngameState.IngameUi.Map;
        private RectangleF MapRectangle => _mapRectangle?.Value ?? (_mapRectangle = new TimeCache<RectangleF>(() => MapWindow.GetClientRect(), 100)).Value;
        private Camera Camera => GameController.Game.IngameState.Camera;
        private float Diag =>
            _diag?.Value ?? (_diag = new TimeCache<float>(() =>
            {
                if (ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
                {
                    var mapRectangle = ingameStateIngameUi.Map.SmallMiniMap.GetClientRect();
                    return (float)(Math.Sqrt(mapRectangle.Width * mapRectangle.Width + mapRectangle.Height * mapRectangle.Height) / 2f);
                }

                return (float)Math.Sqrt(Camera.Width * Camera.Width + Camera.Height * Camera.Height);
            }, 100)).Value;

        private Vector2 ScreenCenter =>
            new Vector2(MapRectangle.Width / 2, MapRectangle.Height / 2 - 20) + new Vector2(MapRectangle.X, MapRectangle.Y) +
            new Vector2(MapWindow.LargeMapShiftX, MapWindow.LargeMapShiftY);

        public override bool Initialise()
        {
            Name = "Heist Icons";
            return base.Initialise();
        }

        public override Job Tick()
        {
            return GameController.MultiThreadManager.AddJob(TickLogic, nameof(HeistIconsCore));
        }

        private void TickLogic()
        {
            ingameStateIngameUi = GameController.Game.IngameState.IngameUi;

            if (ingameStateIngameUi.Map.SmallMiniMap.IsVisibleLocal)
            {
                var mapRectangle = ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                screentCenterCache = new Vector2(mapRectangle.X + mapRectangle.Width / 2, mapRectangle.Y + mapRectangle.Height / 2);
                largeMap = false;
            }
            else if (ingameStateIngameUi.Map.LargeMap.IsVisibleLocal)
            {
                screentCenterCache = ScreenCenter;
                largeMap = true;
            }

            k = Camera.Width < 1024f ? 1120f : 1024f;
            scale = k / Camera.Height * Camera.Width * 3.06f / 4f / MapWindow.LargeMapZoom;
            playerPos = GameController.Player.GetComponent<Positioned>().GridPos;
            posZ = GameController.Player.GetComponent<Render>().Pos.Z;

            if (Entities.Count == 0) return;

            var e = Entities.Dequeue();

            if (!e.IsValid) return;
            if (e.Type == EntityType.Monster && e.IsDead) return;
            if (e.Type == EntityType.Chest && e.IsOpened) return;

            Entities.Enqueue(e);
        }

        public override void Render()
        {
            var mapWindowLargeMapZoom = MapWindow.LargeMapZoom;

            foreach (var e in Entities)
            {
                try
                {
                    var renderComponent = e?.GetComponent<Render>();
                    if (renderComponent == null) continue;

                    string renderName = e.Path
                        .Replace("Metadata/Chests/LeagueHeist/HeistChest", "")
                        .Replace("Metadata/Chests/LeaguesHeist/HeistChest", "")
                        .Replace("Metadata/Chests/LeagueHeist/Heist", "")

                        .Replace("Military", "")
                        .Replace("Thug", "")
                        .Replace("Science", "")
                        .Replace("Robot", "")

                        .Replace("Secondary", "");

                    if (!e.Path.Contains("RewardRoom"))
                    {
                        var icon = GetMapIcon(e);
                        if (icon == null) continue;

                        var size = icon.Size * (1 + mapWindowLargeMapZoom);
                        var iconZ = renderComponent.Pos.Z;
                        Vector2 position;

                        if (largeMap)
                        {
                            position = screentCenterCache + MapIcon.DeltaInWorldToMinimapDelta(
                                e.GetComponent<Positioned>().GridPos - playerPos, Diag, scale, (iconZ - posZ) / (9f / mapWindowLargeMapZoom));

                            Graphics.DrawImage(icon.Texture, new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size), icon.Color);
                        }
                        else
                        {
                            position = screentCenterCache + MapIcon.DeltaInWorldToMinimapDelta(
                                e.GetComponent<Positioned>().GridPos - playerPos, Diag, 240f, (iconZ - posZ) / 20);

                            var mapRectangle = ingameStateIngameUi.Map.SmallMiniMap.GetClientRectCache;
                            var rectangle = new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size);

                            mapRectangle.Contains(ref rectangle, out var isContain);
                            if (isContain)
                                Graphics.DrawImage(icon.Texture, new RectangleF(position.X - size / 2f, position.Y - size / 2f, size, size), icon.Color);
                        }
                    }

                    if (e.Type != EntityType.Chest) continue;

                    if (Settings.TextEnable || Settings.WorldIcon)
                    {
                        var worldtoscreen = Camera.WorldToScreen(e.Pos);

                        if (Settings.TextEnable)
                        {
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
                            renderName += " ";

                            var textBox = Graphics.MeasureText(renderName);
                            System.Numerics.Vector2 backgroundBox;
                            backgroundBox.X = textBox.X + 2;
                            backgroundBox.Y = textBox.Y * 2f;

                            var rectangleHeight = backgroundBox.Y;

                            if (Settings.UseDefaultText)
                                Graphics.DrawText(renderName, worldtoscreen.ToVector2Num(), Settings.TextColor.Value, 22, "Default:13", FontAlign.Center);
                            else
                            {
                                Graphics.DrawText(renderName, worldtoscreen.ToVector2Num(), Settings.TextColor.Value, 22, FontAlign.Center);
                                backgroundBox.X *= Settings.BackgroundWidth;
                                rectangleHeight *= Settings.BackgroundHeight;
                            }

                            var rectangle = new RectangleF(worldtoscreen.X - backgroundBox.X / 2, worldtoscreen.Y - (backgroundBox.Y - textBox.Y) / 2, backgroundBox.X, rectangleHeight);

                            Graphics.DrawBox(rectangle, Settings.TextBackgroundColor.Value);
                            Graphics.DrawFrame(rectangle, Settings.TextBorderColor.Value, 1);
                        }

                        if (Settings.WorldIcon && !e.Path.Contains("RewardRoom"))
                        {
                            var icon = GetWorldIcon(e);
                            if (icon == null) continue;

                            worldtoscreen = Camera.WorldToScreen(e.Pos.Translate(0, 0, -150));

                            if (worldtoscreen == new Vector2()) continue;

                            Graphics.DrawImage(icon.Texture, new RectangleF(worldtoscreen.X - icon.Size / 2f, worldtoscreen.Y - icon.Size / 2f, icon.Size, icon.Size), icon.Color);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogError($"{Name}: {ex.Message}");
                }
            }

            base.Render();
        }

        public override void EntityAdded(Entity e)
        {
            if (e == null) return;
            if (!e.IsHostile) return;
            if (!e.Path.Contains("Heist")) return;
            if (!e.Path.Contains("Monsters") && !e.Path.Contains("Chest")) return;
            if (e.Type == EntityType.Monster && e.Rarity != MonsterRarity.Unique) return;

            Entities.Enqueue(e);
        }

        private MapIcon GetWorldIcon(Entity e)
        {
            // if (e.Path.Contains("Waypoint")) { return new MapIcon(GetAtlasTexture("ChestUnopenedGeneric"), Settings.WorldIconSize.Value); }

            if (e.Path.Contains("Safe")) { return new MapIcon(GetAtlasTexture("ChestUnopenedGeneric"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("QualityCurrency")) { return new MapIcon(GetAtlasTexture("ChestUnopenedCurrency"), Settings.WorldIconSize.Value, Color.Gray); }
            if (e.Path.Contains("Currency")) { return new MapIcon(GetAtlasTexture("ChestUnopenedCurrency"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Armour")) { return new MapIcon(GetAtlasTexture("ChestUnopenedArmour"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Weapons")) { return new MapIcon(GetAtlasTexture("ChestUnopenedWeapons"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Jewellery")) { return new MapIcon(GetAtlasTexture("ChestUnopenedTrinkets"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Jewels")) { return new MapIcon(GetAtlasTexture("Jewel"), Settings.WorldIconSize.Value * 0.8f); }
            if (e.Path.Contains("Maps")) { return new MapIcon(GetAtlasTexture("ChestUnopenedMaps"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("DivinationCards")) { return new MapIcon(GetAtlasTexture("ChestUnopenedDivination"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("StackedDecks")) { return new MapIcon(GetAtlasTexture("StackedDecks"), Settings.WorldIconSize.Value * 0.8f); }
            if (e.Path.Contains("Gems")) { return new MapIcon(GetAtlasTexture("ChestUnopenedGems"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Corrupted")) { return new MapIcon(GetAtlasTexture("Corruption"), Settings.WorldIconSize.Value * 0.8f); }
            if (e.Path.Contains("Uniques")) { return new MapIcon(GetAtlasTexture("ChestUnopenedUniques"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Prophecies")) { return new MapIcon(GetAtlasTexture("ChestUnopenedProphecies"), Settings.WorldIconSize.Value); }
            if (e.Path.Contains("Essences")) { return new MapIcon(GetAtlasTexture("ChestUnopenedEssence"), Settings.WorldIconSize.Value); }

            return null;
        }

        public MapIcon GetMapIcon(Entity e)
        {
            // if (e.Path.Contains("Waypoint")) { return new MapIcon(GetAtlasTexture("HeistPathChest"), Settings.MapIconSize.Value); }

            if (e.Path.Contains("Heist") && e.Path.Contains("Monster")) { return new MapIcon(GetAtlasTexture("HeistSpottedMiniBoss"), Settings.MapIconSize.Value * 0.8f); }

            if (e.Path.Contains("Smugglers")) { return new MapIcon(GetAtlasTexture("HeistSumgglersCache"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Safe")) { return new MapIcon(GetAtlasTexture("HeistPathChest"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("QualityCurrency")) { return new MapIcon(GetAtlasTexture("RewardCurrency"), Settings.MapIconSize.Value, Color.Gray); }
            if (e.Path.Contains("Currency")) { return new MapIcon(GetAtlasTexture("RewardCurrency"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Armour")) { return new MapIcon(GetAtlasTexture("RewardArmour"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Weapons")) { return new MapIcon(GetAtlasTexture("RewardWeapons"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Jewellery")) { return new MapIcon(GetAtlasTexture("RewardJewellery"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Jewels")) { return new MapIcon(GetAtlasTexture("Jewel"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Maps")) { return new MapIcon(GetAtlasTexture("RewardMaps"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("DivinationCards")) { return new MapIcon(GetAtlasTexture("RewardDivinationCards"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("StackedDecks")) { return new MapIcon(GetAtlasTexture("StackedDecks"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Gems")) { return new MapIcon(GetAtlasTexture("RewardGems"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Corrupted")) { return new MapIcon(GetAtlasTexture("Corruption"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Uniques")) { return new MapIcon(GetAtlasTexture("RewardUniques"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Prophecies")) { return new MapIcon(GetAtlasTexture("RewardProphecy"), Settings.MapIconSize.Value); }
            if (e.Path.Contains("Essences")) { return new MapIcon(GetAtlasTexture("RewardEssences"), Settings.MapIconSize.Value); }

            return null;
        }
    }
}
