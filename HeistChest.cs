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

        public HeistChest(Entity e)
        {
            Entity = e;
            Type = e.Path.Contains("RewardRoom") ? HeistChestTypes.RewardRoom : HeistChestTypes.Normal;
            Name = e.Path.Clean();
            Reward = GetRewardType(Name);
            WorldIcon = GetWorldIcon(Reward);
            MapIcon = GetMapIcon(Reward);
            IsClosed = true;
        }

        public void Update()
        {
            IsClosed = Entity.IsValid && !Entity.IsOpened;
        }

        private HeistRewardTypes GetRewardType(string name)
        {
            if (name.Contains("Smugglers"))
                return HeistRewardTypes.Smugglers;
            if (name.Contains("Safe"))
                return HeistRewardTypes.Safe;
            if (name.Contains("QualityCurrency"))
                return HeistRewardTypes.QualityCurrency;
            if (name.Contains("Currency"))
                return HeistRewardTypes.Currency;
            if (name.Contains("Armour"))
                return HeistRewardTypes.Armour;
            if (name.Contains("Weapons"))
                return HeistRewardTypes.Weapons;
            if (name.Contains("Jewellery"))
                return HeistRewardTypes.Jewellery;
            if (name.Contains("Trinkets"))
                return HeistRewardTypes.Jewellery;
            if (name.Contains("Jewels"))
                return HeistRewardTypes.Jewels;
            if (name.Contains("Maps"))
                return HeistRewardTypes.Maps;
            if (name.Contains("DivinationCards"))
                return HeistRewardTypes.DivinationCards;
            if (name.Contains("StackedDecks"))
                return HeistRewardTypes.StackedDecks;
            if (name.Contains("Gems"))
                return HeistRewardTypes.Gems;
            if (name.Contains("Corrupted"))
                return HeistRewardTypes.Corrupted;
            if (name.Contains("Uniques"))
                return HeistRewardTypes.Uniques;
            if (name.Contains("Prophecies"))
                return HeistRewardTypes.Prophecies;
            if (name.Contains("Essences"))
                return HeistRewardTypes.Essences;

            return HeistRewardTypes.None;
        }

        private ChestIcon GetWorldIcon(HeistRewardTypes rewardType)
        {
            switch (rewardType)
            {
                case HeistRewardTypes.Safe:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedGeneric"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.QualityCurrency:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedCurrency"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value, Color.Gray);

                case HeistRewardTypes.Currency:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedCurrency"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Armour:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedArmour"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Weapons:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedWeapons"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Jewellery:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedTrinkets"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Jewels:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("Jewel"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Maps:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedMaps"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.DivinationCards:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedDivination"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.StackedDecks:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("StackedDecks"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Gems:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedGems"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Corrupted:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("Corruption"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value * 0.8f);

                case HeistRewardTypes.Uniques:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedUniques"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Prophecies:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedProphecies"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                case HeistRewardTypes.Essences:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("ChestUnopenedEssence"),
                        Core.Core.MainPlugin.Settings.WorldIconSize.Value);

                default:
                    return null;
            }
        }

        private ChestIcon GetMapIcon(HeistRewardTypes rewardType)
        {
            switch (rewardType)
            {
                case HeistRewardTypes.Smugglers:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("HeistSumgglersCache"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value * 0.6f);

                case HeistRewardTypes.Safe:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("HeistPathChest"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.QualityCurrency:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardCurrency"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value, Color.Gray);

                case HeistRewardTypes.Currency:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardCurrency"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Armour:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardArmour"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Weapons:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardWeapons"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Jewellery:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardJewellery"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Jewels:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("Jewel"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Maps:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardMaps"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.DivinationCards:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardDivinationCards"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.StackedDecks:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("StackedDecks"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Gems:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardGems"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Corrupted:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("Corruption"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value * 0.8f);

                case HeistRewardTypes.Uniques:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardUniques"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Prophecies:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardProphecy"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);

                case HeistRewardTypes.Essences:
                    return new ChestIcon(Core.Core.MainPlugin.GetAtlasTexture("RewardEssences"),
                        Core.Core.MainPlugin.Settings.MapIconSize.Value);
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

        public static string Clean(this string s)
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
