using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;

namespace CubeTMG.Items
{
    public static class ItemDatabase
    {
        private static readonly Dictionary<string, ItemData> items = new Dictionary<string, ItemData>();

        static ItemDatabase()
        {
            RegisterAllItems();
        }

        public static ItemData Get(string id)
        {
            if (items.TryGetValue(id, out var item))
            {
                return item.Clone();
            }
            Debug.LogWarning($"ItemDatabase: Item '{id}' not found!");
            return null;
        }

        private static void Add(ItemData item)
        {
            items[item.id] = item;
        }

        private static void RegisterAllItems()
        {
            // ==========================================
            // WEAPONS: STAVES (Wizard)
            // ==========================================
            Add(new ItemData
            {
                id = "staff_energy",
                name = "Energy Staff",
                description = "A novice wizard staff firing dual energy cubes.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Wizard,
                minDamage = 12, maxDamage = 20,
                rateOfFire = 1.0f, range = 8.5f, projectileSpeed = 16f,
                numProjectiles = 2, arcGapAngle = 6f,
                bulletColor = new Color(0.2f, 0.7f, 1f), bulletSize = 0.3f
            });

            Add(new ItemData
            {
                id = "staff_destruction",
                name = "Staff of Destruction",
                description = "Emits crackling fire and arcane force.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Purple,
                requiredClass = PlayerClassType.Wizard,
                minDamage = 28, maxDamage = 45,
                rateOfFire = 1.0f, range = 9f, projectileSpeed = 17f,
                numProjectiles = 2, arcGapAngle = 6f,
                bulletColor = new Color(1f, 0.4f, 0.1f), bulletSize = 0.35f,
                statBonus = new StatBlock { attack = 2, wisdom = 2 }
            });

            Add(new ItemData
            {
                id = "staff_cosmic",
                name = "Staff of the Cosmic Whole",
                description = "High tier staff channelled with galactic stellar energy.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Cyan,
                requiredClass = PlayerClassType.Wizard,
                minDamage = 55, maxDamage = 85,
                rateOfFire = 1.0f, range = 9.5f, projectileSpeed = 18f,
                numProjectiles = 2, arcGapAngle = 6f,
                bulletColor = new Color(0.8f, 0.2f, 1f), bulletSize = 0.4f,
                statBonus = new StatBlock { attack = 4, wisdom = 4 }
            });

            Add(new ItemData
            {
                id = "staff_ep",
                name = "Staff of Extreme Prejudice",
                description = "[WHITE BAG UT] Fires a devastating spiral nova of 8 lethal fireballs around the caster!",
                itemType = ItemType.Weapon,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Wizard,
                minDamage = 75, maxDamage = 110,
                rateOfFire = 1.2f, range = 5.0f, projectileSpeed = 12f,
                numProjectiles = 8, arcGapAngle = 45f,
                bulletColor = new Color(1f, 0.2f, 0.2f), bulletSize = 0.45f,
                statBonus = new StatBlock { attack = 6, defense = -2 }
            });

            // ==========================================
            // WEAPONS: SWORDS (Warrior & Knight)
            // ==========================================
            Add(new ItemData
            {
                id = "sword_short",
                name = "Short Sword",
                description = "A basic steel blade for close range cleaving.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Warrior,
                anyClassCanEquip = false,
                minDamage = 25, maxDamage = 40,
                rateOfFire = 1.0f, range = 4.5f, projectileSpeed = 18f,
                numProjectiles = 1,
                bulletColor = Color.white, bulletSize = 0.35f
            });

            Add(new ItemData
            {
                id = "sword_archon",
                name = "Archon Sword",
                description = "Tempered golden greatsword capable of cleaving multiple foes.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Purple,
                requiredClass = PlayerClassType.Warrior,
                minDamage = 60, maxDamage = 95,
                rateOfFire = 1.0f, range = 4.8f, projectileSpeed = 19f,
                numProjectiles = 1, isPiercing = true,
                bulletColor = new Color(1f, 0.85f, 0.2f), bulletSize = 0.45f,
                statBonus = new StatBlock { defense = 2, attack = 2 }
            });

            Add(new ItemData
            {
                id = "sword_dblade",
                name = "Demon Blade",
                description = "[WHITE BAG UT] Dual-bladed hellfire sword forged in the Abyss. Fires two diverging blades with colossal damage.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Warrior,
                minDamage = 110, maxDamage = 150,
                rateOfFire = 1.0f, range = 4.2f, projectileSpeed = 16f,
                numProjectiles = 2, arcGapAngle = 18f,
                bulletColor = new Color(1f, 0.15f, 0.05f), bulletSize = 0.5f,
                statBonus = new StatBlock { attack = 5, defense = -3 }
            });

            // ==========================================
            // WEAPONS: BOWS (Archer)
            // ==========================================
            Add(new ItemData
            {
                id = "bow_short",
                name = "Shortbow",
                description = "Light hunting bow firing a spread of 3 arrows.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Archer,
                minDamage = 8, maxDamage = 18,
                rateOfFire = 1.0f, range = 8.0f, projectileSpeed = 18f,
                numProjectiles = 3, arcGapAngle = 9f,
                bulletColor = new Color(0.3f, 0.9f, 0.3f), bulletSize = 0.25f
            });

            Add(new ItemData
            {
                id = "bow_covert",
                name = "Bow of Covert Havens",
                description = "High tier composite bow that fires 3 piercing venomous arrows.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Cyan,
                requiredClass = PlayerClassType.Archer,
                minDamage = 35, maxDamage = 60,
                rateOfFire = 1.0f, range = 8.8f, projectileSpeed = 20f,
                numProjectiles = 3, arcGapAngle = 8f, isPiercing = true,
                bulletColor = new Color(0.2f, 1f, 0.6f), bulletSize = 0.32f,
                statBonus = new StatBlock { dexterity = 4, speed = 2 }
            });

            Add(new ItemData
            {
                id = "bow_dbow",
                name = "Doom Bow",
                description = "[WHITE BAG UT] The legendary doom bow. Fires a single, massive, piercing blast of ancient apocalyptic power.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Archer,
                minDamage = 260, maxDamage = 360,
                rateOfFire = 0.45f, range = 10.0f, projectileSpeed = 14f,
                numProjectiles = 1, isPiercing = true,
                bulletColor = new Color(0.1f, 0.9f, 1f), bulletSize = 0.6f,
                statBonus = new StatBlock { attack = 8 }
            });

            // ==========================================
            // WEAPONS: WANDS (Priest)
            // ==========================================
            Add(new ItemData
            {
                id = "wand_fire",
                name = "Fire Wand",
                description = "A holy wand firing long-range piercing beams of light.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Priest,
                minDamage = 14, maxDamage = 24,
                rateOfFire = 1.0f, range = 9.5f, projectileSpeed = 18f,
                numProjectiles = 1, isPiercing = true,
                bulletColor = new Color(1f, 0.9f, 0.3f), bulletSize = 0.3f
            });

            Add(new ItemData
            {
                id = "wand_fallen",
                name = "Wand of the Fallen",
                description = "[WHITE BAG UT] Fires heavy holy orbs that explode through multiple enemies.",
                itemType = ItemType.Weapon,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Priest,
                minDamage = 85, maxDamage = 130,
                rateOfFire = 0.85f, range = 9.0f, projectileSpeed = 15f,
                numProjectiles = 1, isPiercing = true,
                bulletColor = new Color(1f, 0.95f, 0.5f), bulletSize = 0.5f,
                statBonus = new StatBlock { attack = 4, wisdom = 5 }
            });

            // ==========================================
            // ABILITIES
            // ==========================================
            Add(new ItemData
            {
                id = "spell_destruction",
                name = "Destruction Sphere",
                description = "Wizard Ability: Targets cursor and explodes into 16 spell cubes bursting outwards.",
                itemType = ItemType.Ability,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Wizard,
                abilityType = AbilityType.Spellbomb,
                manaCost = 65, cooldown = 0.4f, abilityPower = 40, abilityRadius = 3.5f,
                statBonus = new StatBlock { attack = 2, maxMP = 10 }
            });

            Add(new ItemData
            {
                id = "spell_tablet",
                name = "Tablet of the King's Avatar",
                description = "[WHITE BAG UT] Ancient avatar tablet that fires piercing arcane darts converging on cursor.",
                itemType = ItemType.Ability,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Wizard,
                abilityType = AbilityType.Spellbomb,
                manaCost = 85, cooldown = 0.3f, abilityPower = 85, abilityRadius = 4.0f,
                statBonus = new StatBlock { attack = 5, wisdom = 5 }
            });

            Add(new ItemData
            {
                id = "helm_golden",
                name = "Golden Helm",
                description = "Warrior Ability: Inspires warrior fury, granting Berserk (+50% RoF) and Speedy (+50% Spd).",
                itemType = ItemType.Ability,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Warrior,
                abilityType = AbilityType.Helm,
                manaCost = 55, cooldown = 5.0f, statusDuration = 5.0f,
                statBonus = new StatBlock { defense = 3, vitality = 2 }
            });

            Add(new ItemData
            {
                id = "helm_jugg",
                name = "Helm of the Juggernaut",
                description = "[WHITE BAG UT] Infuses player with unyielding stone, granting Armored (2x DEF) and Berserk!",
                itemType = ItemType.Ability,
                bagTier = BagTier.White,
                requiredClass = PlayerClassType.Warrior,
                abilityType = AbilityType.Helm,
                manaCost = 75, cooldown = 6.0f, statusDuration = 5.5f,
                statBonus = new StatBlock { defense = 8, attack = 3 }
            });

            Add(new ItemData
            {
                id = "tome_rejuvenation",
                name = "Tome of Rejuvenation",
                description = "Priest Ability: Heals 120 HP to self and grants Healing buff for 4 seconds.",
                itemType = ItemType.Ability,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Priest,
                abilityType = AbilityType.Tome,
                manaCost = 60, cooldown = 1.0f, abilityPower = 120, statusDuration = 4.0f,
                statBonus = new StatBlock { wisdom = 3, vitality = 3 }
            });

            Add(new ItemData
            {
                id = "quiver_golden",
                name = "Golden Quiver",
                description = "Archer Ability: Shoots a heavy bolt that pierces enemies and Paralyzes them for 3 seconds.",
                itemType = ItemType.Ability,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Archer,
                abilityType = AbilityType.Quiver,
                manaCost = 60, cooldown = 0.5f, abilityPower = 90, statusDuration = 3.0f,
                inflictedStatus = StatusEffectType.Paralyzed,
                statBonus = new StatBlock { attack = 2, dexterity = 2 }
            });

            Add(new ItemData
            {
                id = "shield_mithril",
                name = "Mithril Shield",
                description = "Knight Ability: Launches a brutal cone of heavy bash projectiles that Stuns enemies for 3 seconds.",
                itemType = ItemType.Ability,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Knight,
                abilityType = AbilityType.Shield,
                manaCost = 70, cooldown = 1.0f, abilityPower = 140, statusDuration = 3.0f,
                inflictedStatus = StatusEffectType.Stunned,
                statBonus = new StatBlock { defense = 6, vitality = 3 }
            });

            // ==========================================
            // ARMORS
            // ==========================================
            Add(new ItemData
            {
                id = "robe_novice",
                name = "Novice Robe",
                description = "Woven cloth robe enhancing magic and mana.",
                itemType = ItemType.Armor,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Wizard,
                statBonus = new StatBlock { defense = 3, maxMP = 20, wisdom = 2 }
            });

            Add(new ItemData
            {
                id = "robe_moon",
                name = "Robe of the Grand Sorcerer",
                description = "Royal silk imbued with stellar magic.",
                itemType = ItemType.Armor,
                bagTier = BagTier.Cyan,
                requiredClass = PlayerClassType.Wizard,
                statBonus = new StatBlock { defense = 12, maxMP = 50, attack = 4, wisdom = 5 }
            });

            Add(new ItemData
            {
                id = "armor_iron",
                name = "Iron Chainmail",
                description = "Heavy iron plating for warriors.",
                itemType = ItemType.Armor,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Warrior,
                statBonus = new StatBlock { defense = 7, maxHP = 20 }
            });

            Add(new ItemData
            {
                id = "armor_acropolis",
                name = "Acropolis Armor",
                description = "Gleaming golden plate armor of immense defense.",
                itemType = ItemType.Armor,
                bagTier = BagTier.Cyan,
                requiredClass = PlayerClassType.Warrior,
                statBonus = new StatBlock { defense = 22, maxHP = 40 }
            });

            Add(new ItemData
            {
                id = "leather_stud",
                name = "Studded Leather",
                description = "Agile armor for archers.",
                itemType = ItemType.Armor,
                bagTier = BagTier.Brown,
                requiredClass = PlayerClassType.Archer,
                statBonus = new StatBlock { defense = 5, speed = 2, dexterity = 2 }
            });

            // ==========================================
            // RINGS
            // ==========================================
            Add(new ItemData
            {
                id = "ring_def_minor",
                name = "Ring of Minor Defense",
                description = "Grants +3 Defense.",
                itemType = ItemType.Ring,
                bagTier = BagTier.Brown,
                anyClassCanEquip = true,
                statBonus = new StatBlock { defense = 3 }
            });

            Add(new ItemData
            {
                id = "ring_def_para",
                name = "Para Defense Ring",
                description = "Grants +6 Defense.",
                itemType = ItemType.Ring,
                bagTier = BagTier.Purple,
                anyClassCanEquip = true,
                statBonus = new StatBlock { defense = 6 }
            });

            Add(new ItemData
            {
                id = "ring_pyra",
                name = "Ring of the Pyramid",
                description = "[WHITE BAG UT] Ancient artifact granting high HP and Defense.",
                itemType = ItemType.Ring,
                bagTier = BagTier.White,
                anyClassCanEquip = true,
                statBonus = new StatBlock { maxHP = 80, defense = 4, attack = 4 }
            });

            // ==========================================
            // STAT INCREASE POTIONS (BLUE BAGS)
            // ==========================================
            Add(new ItemData
            {
                id = "pot_att",
                name = "Potion of Attack",
                description = "Permanently increases Attack by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Attack,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_def",
                name = "Potion of Defense",
                description = "Permanently increases Defense by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Defense,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_spd",
                name = "Potion of Speed",
                description = "Permanently increases Speed by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Speed,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_dex",
                name = "Potion of Dexterity",
                description = "Permanently increases Dexterity by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Dexterity,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_vit",
                name = "Potion of Vitality",
                description = "Permanently increases Vitality by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Vitality,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_wis",
                name = "Potion of Wisdom",
                description = "Permanently increases Wisdom by 1.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.Wisdom,
                permanentStatAmount = 1
            });

            Add(new ItemData
            {
                id = "pot_life",
                name = "Potion of Life",
                description = "Permanently increases Max HP by 5.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.MaxHP,
                permanentStatAmount = 5
            });

            Add(new ItemData
            {
                id = "pot_mana",
                name = "Potion of Mana",
                description = "Permanently increases Max MP by 5.",
                itemType = ItemType.StatPotion,
                bagTier = BagTier.Blue,
                anyClassCanEquip = true,
                isPermanentStatPotion = true,
                permanentStat = StatType.MaxMP,
                permanentStatAmount = 5
            });

            // ==========================================
            // CONSUMABLES
            // ==========================================
            Add(new ItemData
            {
                id = "hp_pot",
                name = "Health Potion",
                description = "Instantly restores 100 HP.",
                itemType = ItemType.Consumable,
                bagTier = BagTier.Brown,
                anyClassCanEquip = true,
                healHP = 100
            });

            Add(new ItemData
            {
                id = "mp_pot",
                name = "Magic Potion",
                description = "Instantly restores 100 MP.",
                itemType = ItemType.Consumable,
                bagTier = BagTier.Brown,
                anyClassCanEquip = true,
                healMP = 100
            });
        }
    }
}
