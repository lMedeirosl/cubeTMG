using System;
using UnityEngine;

namespace CubeTMG.Stats
{
    public enum StatType
    {
        MaxHP,
        MaxMP,
        Attack,
        Defense,
        Speed,
        Dexterity,
        Vitality,
        Wisdom
    }

    [Flags]
    public enum StatusEffectType
    {
        None = 0,
        Stunned = 1 << 0,       // Cannot shoot
        Paralyzed = 1 << 1,     // Cannot move
        Dazed = 1 << 2,         // Rate of fire cut in half
        Blind = 1 << 3,         // Vision heavily reduced
        Bleeding = 1 << 4,      // Takes periodic damage
        Sick = 1 << 5,          // Cannot heal from VIT or potions
        Quiet = 1 << 6,         // Cannot regenerate MP, MP set to 0
        Berserk = 1 << 7,       // +50% Rate of fire
        Damaging = 1 << 8,      // +50% Projectile damage
        Speedy = 1 << 9,        // +50% Movement speed
        Healing = 1 << 10,      // +20 HP/sec regeneration
        Armored = 1 << 11,      // Defense doubled
        Invulnerable = 1 << 12  // Takes zero damage
    }

    public enum PlayerClassType
    {
        Wizard,
        Warrior,
        Priest,
        Archer,
        Knight
    }

    public enum ItemType
    {
        Weapon,
        Ability,
        Armor,
        Ring,
        Consumable,
        StatPotion,
        CraftingMaterial // Flux, Pearls
    }

    public enum BagTier
    {
        Brown = 0,   // Low tier items / minor pots
        Purple = 1,  // Mid tier gear
        Cyan = 2,    // High tier gear
        Blue = 3,    // Stat increase potions
        White = 4    // Legendary Untiered (UT) / Stellar / Crystal gear!
    }

    // Trove-style Rarity Hierarchy
    public enum TroveRarity
    {
        Common = 0,       // Gray
        Uncommon = 1,     // Green
        Rare = 2,         // Blue
        Epic = 3,         // Purple
        Legendary = 4,    // Orange
        Relic = 5,        // Red
        Resplendent = 6,  // Rainbow / Bright Cyan
        Shadow = 7,       // Deep Violet / Dark Void
        Radiant = 8,      // Radiant Solar White/Gold
        Stellar = 9,      // Cosmic Gold with stars
        Crystal = 10      // Iridescent Diamond Crystal
    }

    public static class TroveRarityColors
    {
        public static Color GetColor(TroveRarity rarity)
        {
            switch (rarity)
            {
                case TroveRarity.Common: return new Color(0.75f, 0.75f, 0.75f);
                case TroveRarity.Uncommon: return new Color(0.2f, 0.85f, 0.2f);
                case TroveRarity.Rare: return new Color(0.2f, 0.6f, 1f);
                case TroveRarity.Epic: return new Color(0.7f, 0.25f, 0.95f);
                case TroveRarity.Legendary: return new Color(1f, 0.55f, 0.05f);
                case TroveRarity.Relic: return new Color(0.95f, 0.15f, 0.15f);
                case TroveRarity.Resplendent: return new Color(0.1f, 0.95f, 0.9f);
                case TroveRarity.Shadow: return new Color(0.45f, 0.1f, 0.7f);
                case TroveRarity.Radiant: return new Color(1f, 0.95f, 0.4f);
                case TroveRarity.Stellar: return new Color(1f, 0.8f, 0.1f);
                case TroveRarity.Crystal: return new Color(0.3f, 0.95f, 1f);
                default: return Color.white;
            }
        }

        public static string GetHexColor(TroveRarity rarity)
        {
            Color c = GetColor(rarity);
            return ColorUtility.ToHtmlStringRGB(c);
        }
    }
}
