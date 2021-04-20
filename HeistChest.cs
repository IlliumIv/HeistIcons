using ExileCore.PoEMemory.MemoryObjects;
using HeistIcons.Enums;
using SharpDX;
using System.Linq;
using System.Text;

namespace HeistIcons
{
    public class HeistChest
    {
        public Entity Entity { get; }
        public HeistChestTypes Type { get; }
        public HeistRewardTypes Reward { get; }
        public string Name { get; }
        public ChestIcon WorldIcon { get;  }
        public ChestIcon MapIcon { get; }
        public bool IsClosed { get; private set; }

        // Right order is important
        public HeistChest(Entity e)
        {
            Entity = e;
            Type = e.Path.Contains("RewardRoom") ? HeistChestTypes.RewardRoom : HeistChestTypes.Normal;
            Name = e.Path.Extract_CleanName();
            Reward = GetRewardType();
            WorldIcon = GetWorldIcon();
            MapIcon = GetMapIcon();
            IsClosed = true;
        }

        public void Update()
        {
            IsClosed = Entity.IsValid && !Entity.IsOpened;
        }

        // Right order is important
        private HeistRewardTypes GetRewardType()
        {
            if (Name.Contains("Smugglers"))
                return HeistRewardTypes.Smugglers;
            if (Name.Contains("Safe"))
                return HeistRewardTypes.Safe;
            if (Name.Contains("Quality"))
                return HeistRewardTypes.QualityCurrency;
            if (Name.Contains("Currency"))
                return HeistRewardTypes.Currency;
            if (Name.Contains("Armour"))
                return HeistRewardTypes.Armour;
            if (Name.Contains("Weapons"))
                return HeistRewardTypes.Weapons;
            if (Name.Contains("Jewellery"))
                return HeistRewardTypes.Jewellery;
            if (Name.Contains("Trinkets"))
                return HeistRewardTypes.Jewellery;
            if (Name.Contains("Jewels"))
                return HeistRewardTypes.Jewels;
            if (Name.Contains("Maps"))
                return HeistRewardTypes.Maps;
            if (Name.Contains("Divination"))
                return HeistRewardTypes.DivinationCards;
            if (Name.Contains("Stacked"))
                return HeistRewardTypes.StackedDecks;
            if (Name.Contains("Gems"))
                return HeistRewardTypes.Gems;
            if (Name.Contains("Corrupted"))
                return HeistRewardTypes.Corrupted;
            if (Name.Contains("Uniques"))
                return HeistRewardTypes.Uniques;
            if (Name.Contains("Prophecies"))
                return HeistRewardTypes.Prophecies;
            if (Name.Contains("Essences"))
                return HeistRewardTypes.Essences;
            if (Name.Contains("Fragments"))
                return HeistRewardTypes.Fragments;

            return HeistRewardTypes.None;
        }

        private ChestIcon GetWorldIcon()
        {
            switch (Reward)
            {
                case HeistRewardTypes.Smugglers:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("HeistSumgglersCache"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value * 0.6f);

                case HeistRewardTypes.Safe:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedGeneric"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.QualityCurrency:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedCurrency"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value, Color.Gray);

                case HeistRewardTypes.Currency:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedCurrency"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Armour:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedArmour"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Weapons:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedWeapons"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Jewellery:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedTrinkets"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Jewels:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("Jewel"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Maps:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedMaps"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.DivinationCards:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedDivination"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.StackedDecks:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("StackedDecks"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Gems:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedGems"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Corrupted:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("Corruption"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Uniques:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedUniques"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Prophecies:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedProphecies"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Essences:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedEssence"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Fragments:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("ChestUnopenedFragments"),
                        Main.Core.Plugin.Settings.WorldIconSize.Value);

                default:
                    return null;
            }
        }

        private ChestIcon GetMapIcon()
        {
            switch (Reward)
            {
                case HeistRewardTypes.Safe:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("HeistPathChest"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.QualityCurrency:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardCurrency"),
                        Main.Core.Plugin.Settings.MapIconSize.Value, Color.Gray);

                case HeistRewardTypes.Currency:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardCurrency"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Armour:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardArmour"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Weapons:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardWeapons"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Jewellery:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardJewellery"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Jewels:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("Jewel"),
                        Main.Core.Plugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Maps:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardMaps"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.DivinationCards:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardDivinationCards"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.StackedDecks:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("StackedDecks"),
                        Main.Core.Plugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Gems:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardGems"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Corrupted:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("Corruption"),
                        Main.Core.Plugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Uniques:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardUniques"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Prophecies:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardProphecy"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Essences:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardEssences"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Fragments:
                    return new ChestIcon(Main.Core.Plugin.GetAtlasTexture("RewardFragments"),
                        Main.Core.Plugin.Settings.MapIconSize.Value);

                default:
                    return null;
            }
        }
    }

    public static class StringExtension
    {
        private static string[] _replacements;

        static StringExtension()
        {
            _replacements = new string[18]
            {
                "Metadata/Chests/LeagueHeist/HeistChest",
                "Metadata/Chests/LeaguesHeist/HeistChest",
                "Metadata/Chests/LeagueHeist/Heist",

                "Military",
                "Thug",
                "Science",
                "Robot",

                "Secondary",
                "RewardRoom",

                "LockPicking",
                "BruteForce",
                "Perception",
                "Demolition",
                "CounterThaumaturge",
                "TrapDisarmament",
                "Agility",
                "Deception",
                "Engineering"
            };
        }

        public static string Extract_CleanName(this string s)
        {
            StringBuilder sb = new StringBuilder(s);
            s = "";

            foreach (string to_replace in _replacements)
            {
                sb.Replace(to_replace, "");
            }

            foreach (var x in sb.ToString().ToCharArray())
            {
                s += char.IsUpper(x) ? " " + x : "" + x;
            }

            return s + " ";
        }
    }
}
