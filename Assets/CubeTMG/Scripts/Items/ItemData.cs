using UnityEngine;
using CubeTMG.Stats;

namespace CubeTMG.Items
{
    public enum AbilityType
    {
        None,
        Spellbomb,  // Wizard: explodes into 16 ring projectiles at cursor
        Helm,       // Warrior: Berserk + Speedy buff
        Tome,       // Priest: Burst heal + Healing buff
        Quiver,     // Archer: Piercing shot that paralyzes target
        Shield      // Knight: Shotgun burst that stuns enemies
    }

    [System.Serializable]
    public class ItemData
    {
        public string id;
        public string name;
        [TextArea(2, 4)]
        public string description;
        public ItemType itemType;
        public BagTier bagTier = BagTier.Brown;
        public PlayerClassType requiredClass = PlayerClassType.Wizard;
        public bool anyClassCanEquip = false;

        [Header("Trove Rarity & Buff System")]
        public TroveRarity troveRarity = TroveRarity.Common;
        [Range(0, 5)] public int stars = 0;              // ⭐ 0 to 5 Stars (Forged)
        [Range(0, 5)] public int pearlsOfWisdom = 0;     // 🔮 Up to 5 Pearls of Wisdom
        public int forgeLevel = 0;                       // +0 to +25 Forge Upgrades

        [Header("Stat Bonuses")]
        public StatBlock statBonus = new StatBlock();

        [Header("Weapon Stats (If Weapon)")]
        public int minDamage = 15;
        public int maxDamage = 25;
        public float rateOfFire = 1.0f;
        public float range = 8.5f;
        public float projectileSpeed = 16f;
        public int numProjectiles = 1;
        public float arcGapAngle = 10f;
        public bool isPiercing = false;
        public bool isArmorPiercing = false;
        public bool isWavy = false;
        public Color bulletColor = Color.yellow;
        public float bulletSize = 0.35f;

        [Header("Ability Stats (If Ability)")]
        public AbilityType abilityType = AbilityType.None;
        public int manaCost = 50;
        public float cooldown = 0.5f;
        public int abilityPower = 100;
        public float abilityRadius = 3f;
        public StatusEffectType inflictedStatus = StatusEffectType.None;
        public float statusDuration = 3f;

        [Header("Consumable Stats")]
        public int healHP = 0;
        public int healMP = 0;
        public bool isPermanentStatPotion = false;
        public StatType permanentStat = StatType.Attack;
        public int permanentStatAmount = 1;

        public ItemData Clone()
        {
            var clone = (ItemData)MemberwiseClone();
            clone.statBonus = statBonus.Clone();
            return clone;
        }

        // Trove Stat Multiplier:
        // Rarity Tier Multiplier: Common (1.0x) -> Shadow (1.8x) -> Radiant (2.4x) -> Stellar (3.2x) -> Crystal (4.2x)
        // Stars: +20% per star
        // Forge Level: +4% per level
        // Pearls: +10% per pearl
        public float GetTrovePowerMultiplier()
        {
            float rarityMult = 1.0f + ((int)troveRarity * 0.28f);
            float starMult = 1.0f + (stars * 0.20f);
            float forgeMult = 1.0f + (forgeLevel * 0.04f);
            float pearlMult = 1.0f + (pearlsOfWisdom * 0.10f);

            return rarityMult * starMult * forgeMult * pearlMult;
        }

        public int RollDamage()
        {
            float mult = GetTrovePowerMultiplier();
            int baseRoll = Random.Range(minDamage, maxDamage + 1);
            return Mathf.RoundToInt(baseRoll * mult);
        }

        public StatBlock GetEffectiveStatBonus()
        {
            float mult = GetTrovePowerMultiplier();
            return new StatBlock
            {
                maxHP = Mathf.RoundToInt(statBonus.maxHP * mult),
                maxMP = Mathf.RoundToInt(statBonus.maxMP * mult),
                attack = Mathf.RoundToInt(statBonus.attack * mult),
                defense = Mathf.RoundToInt(statBonus.defense * mult),
                speed = Mathf.RoundToInt(statBonus.speed * mult),
                dexterity = Mathf.RoundToInt(statBonus.dexterity * mult),
                vitality = Mathf.RoundToInt(statBonus.vitality * mult),
                wisdom = Mathf.RoundToInt(statBonus.wisdom * mult)
            };
        }

        public string GetStarString()
        {
            if (stars <= 0) return "";
            return new string('★', stars);
        }

        public string GetPearlString()
        {
            if (pearlsOfWisdom <= 0) return "";
            return $" [{pearlsOfWisdom} 🔮]";
        }

        public string GetTroveRarityTag()
        {
            string hex = TroveRarityColors.GetHexColor(troveRarity);
            string starStr = GetStarString();
            string starSuffix = string.IsNullOrEmpty(starStr) ? "" : $" {starStr}";
            return $"<color=#{hex}>[{troveRarity.ToString().ToUpper()}{starSuffix}]</color>";
        }

        public string GetTierString()
        {
            if (bagTier == BagTier.White || troveRarity >= TroveRarity.Radiant) return "UT";
            if (itemType == ItemType.StatPotion) return "POT";
            return $"T{(int)bagTier + 1}";
        }

        public bool UpgradeStar()
        {
            if (stars < 5)
            {
                stars++;
                return true;
            }
            return false;
        }

        public bool AddPearl()
        {
            if (pearlsOfWisdom < 5)
            {
                pearlsOfWisdom++;
                return true;
            }
            return false;
        }
    }
}
