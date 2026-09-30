using System;
using UnityEngine;
using CubeTMG.Stats;

namespace CubeTMG.Items
{
    public class Inventory : MonoBehaviour
    {
        public const int INVENTORY_SIZE = 8;

        [Header("Trove Currencies")]
        public int flux = 250;                 // Trove Forge Currency
        public int pearlsOfWisdomInventory = 2; // Enchantment Pearls

        [Header("Equipment Slots")]
        public ItemData weaponSlot;
        public ItemData abilitySlot;
        public ItemData armorSlot;
        public ItemData ringSlot;

        [Header("Backpack (8 Slots)")]
        public ItemData[] backpack = new ItemData[INVENTORY_SIZE];

        [Header("Potion Quick Slots")]
        public int healthPotions = 3;
        public int magicPotions = 3;
        public const int MAX_QUICK_POTIONS = 6;

        public event Action OnInventoryUpdated;
        public event Action OnEquipmentUpdated;

        private CharacterStats stats;

        private void Awake()
        {
            stats = GetComponent<CharacterStats>();
        }

        private void Start()
        {
            EquipStarterKit(stats != null ? stats.characterClass : PlayerClassType.Wizard);
        }

        public void EquipStarterKit(PlayerClassType pClass)
        {
            weaponSlot = null;
            abilitySlot = null;
            armorSlot = null;
            ringSlot = null;
            for (int i = 0; i < INVENTORY_SIZE; i++) backpack[i] = null;
            healthPotions = 4;
            magicPotions = 4;
            flux = 350;
            pearlsOfWisdomInventory = 3;

            switch (pClass)
            {
                case PlayerClassType.Wizard:
                    weaponSlot = ItemDatabase.Get("staff_energy");
                    weaponSlot.troveRarity = TroveRarity.Uncommon;
                    weaponSlot.stars = 1;

                    abilitySlot = ItemDatabase.Get("spell_destruction");
                    abilitySlot.troveRarity = TroveRarity.Rare;

                    armorSlot = ItemDatabase.Get("robe_novice");
                    ringSlot = ItemDatabase.Get("ring_def_minor");

                    var s1 = ItemDatabase.Get("staff_destruction");
                    s1.troveRarity = TroveRarity.Epic;
                    s1.stars = 2;
                    backpack[0] = s1;

                    backpack[1] = ItemDatabase.Get("pot_att");
                    backpack[2] = ItemDatabase.Get("pot_def");

                    // Stellar Trove staff!
                    var stellarStaff = ItemDatabase.Get("staff_ep");
                    stellarStaff.troveRarity = TroveRarity.Stellar;
                    stellarStaff.stars = 3;
                    stellarStaff.pearlsOfWisdom = 2;
                    backpack[3] = stellarStaff;
                    break;

                case PlayerClassType.Warrior:
                    weaponSlot = ItemDatabase.Get("sword_short");
                    weaponSlot.troveRarity = TroveRarity.Uncommon;
                    weaponSlot.stars = 1;

                    abilitySlot = ItemDatabase.Get("helm_golden");
                    armorSlot = ItemDatabase.Get("armor_iron");
                    ringSlot = ItemDatabase.Get("ring_def_minor");

                    var sw1 = ItemDatabase.Get("sword_archon");
                    sw1.troveRarity = TroveRarity.Epic;
                    sw1.stars = 2;
                    backpack[0] = sw1;
                    backpack[1] = ItemDatabase.Get("pot_def");

                    var dblade = ItemDatabase.Get("sword_dblade");
                    dblade.troveRarity = TroveRarity.Radiant;
                    dblade.stars = 3;
                    backpack[2] = dblade;
                    break;

                case PlayerClassType.Priest:
                    weaponSlot = ItemDatabase.Get("wand_fire");
                    abilitySlot = ItemDatabase.Get("tome_rejuvenation");
                    armorSlot = ItemDatabase.Get("robe_novice");
                    ringSlot = ItemDatabase.Get("ring_def_minor");

                    var wf = ItemDatabase.Get("wand_fallen");
                    wf.troveRarity = TroveRarity.Radiant;
                    wf.stars = 2;
                    backpack[0] = wf;
                    backpack[1] = ItemDatabase.Get("pot_wis");
                    break;

                case PlayerClassType.Archer:
                    weaponSlot = ItemDatabase.Get("bow_short");
                    abilitySlot = ItemDatabase.Get("quiver_golden");
                    armorSlot = ItemDatabase.Get("leather_stud");
                    ringSlot = ItemDatabase.Get("ring_def_minor");

                    var bc = ItemDatabase.Get("bow_covert");
                    bc.troveRarity = TroveRarity.Epic;
                    backpack[0] = bc;

                    var dbow = ItemDatabase.Get("bow_dbow");
                    dbow.troveRarity = TroveRarity.Crystal;
                    dbow.stars = 4;
                    dbow.pearlsOfWisdom = 3;
                    backpack[1] = dbow;
                    break;

                case PlayerClassType.Knight:
                    weaponSlot = ItemDatabase.Get("sword_short");
                    abilitySlot = ItemDatabase.Get("shield_mithril");
                    armorSlot = ItemDatabase.Get("armor_iron");
                    ringSlot = ItemDatabase.Get("ring_def_minor");

                    var acr = ItemDatabase.Get("armor_acropolis");
                    acr.troveRarity = TroveRarity.Shadow;
                    acr.stars = 2;
                    backpack[0] = acr;

                    var swD = ItemDatabase.Get("sword_dblade");
                    swD.troveRarity = TroveRarity.Stellar;
                    backpack[1] = swD;
                    break;
            }

            RecalculateEquipBonuses();
            OnInventoryUpdated?.Invoke();
            OnEquipmentUpdated?.Invoke();
        }

        public bool AddItem(ItemData item)
        {
            if (item == null) return false;

            if (item.id == "hp_pot" && healthPotions < MAX_QUICK_POTIONS)
            {
                healthPotions++;
                OnInventoryUpdated?.Invoke();
                return true;
            }
            if (item.id == "mp_pot" && magicPotions < MAX_QUICK_POTIONS)
            {
                magicPotions++;
                OnInventoryUpdated?.Invoke();
                return true;
            }

            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                if (backpack[i] == null)
                {
                    backpack[i] = item;
                    OnInventoryUpdated?.Invoke();
                    return true;
                }
            }
            return false;
        }

        public bool CanEquip(ItemData item, ItemType targetSlotType)
        {
            if (item == null) return false;
            if (item.itemType != targetSlotType) return false;
            if (item.anyClassCanEquip) return true;
            if (stats != null && item.requiredClass != stats.characterClass) return false;
            return true;
        }

        public void EquipFromBackpack(int index)
        {
            if (index < 0 || index >= INVENTORY_SIZE) return;
            ItemData item = backpack[index];
            if (item == null) return;

            if (item.itemType == ItemType.Consumable || item.itemType == ItemType.StatPotion)
            {
                UseConsumable(index);
                return;
            }

            switch (item.itemType)
            {
                case ItemType.Weapon:
                    if (CanEquip(item, ItemType.Weapon))
                    {
                        backpack[index] = weaponSlot;
                        weaponSlot = item;
                    }
                    break;

                case ItemType.Ability:
                    if (CanEquip(item, ItemType.Ability))
                    {
                        backpack[index] = abilitySlot;
                        abilitySlot = item;
                    }
                    break;

                case ItemType.Armor:
                    if (CanEquip(item, ItemType.Armor))
                    {
                        backpack[index] = armorSlot;
                        armorSlot = item;
                    }
                    break;

                case ItemType.Ring:
                    if (CanEquip(item, ItemType.Ring))
                    {
                        backpack[index] = ringSlot;
                        ringSlot = item;
                    }
                    break;
            }

            RecalculateEquipBonuses();
            OnInventoryUpdated?.Invoke();
            OnEquipmentUpdated?.Invoke();
        }

        public void UnequipToBackpack(ItemType type)
        {
            int emptyIndex = -1;
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                if (backpack[i] == null)
                {
                    emptyIndex = i;
                    break;
                }
            }

            if (emptyIndex == -1) return;

            switch (type)
            {
                case ItemType.Weapon:
                    backpack[emptyIndex] = weaponSlot;
                    weaponSlot = null;
                    break;
                case ItemType.Ability:
                    backpack[emptyIndex] = abilitySlot;
                    abilitySlot = null;
                    break;
                case ItemType.Armor:
                    backpack[emptyIndex] = armorSlot;
                    armorSlot = null;
                    break;
                case ItemType.Ring:
                    backpack[emptyIndex] = ringSlot;
                    ringSlot = null;
                    break;
            }

            RecalculateEquipBonuses();
            OnInventoryUpdated?.Invoke();
            OnEquipmentUpdated?.Invoke();
        }

        // Trove Forging: Adds stars to items!
        public bool ForgeUpgradeItem(ItemData item)
        {
            if (item == null) return false;
            int cost = 50 + (item.stars * 75) + ((int)item.troveRarity * 25);
            if (flux >= cost && item.stars < 5)
            {
                flux -= cost;
                item.UpgradeStar();
                RecalculateEquipBonuses();
                OnInventoryUpdated?.Invoke();
                OnEquipmentUpdated?.Invoke();
                return true;
            }
            return false;
        }

        // Trove Enchantment: Adds a Pearl of Wisdom!
        public bool ApplyPearlToItem(ItemData item)
        {
            if (item == null) return false;
            if (pearlsOfWisdomInventory > 0 && item.pearlsOfWisdom < 5)
            {
                pearlsOfWisdomInventory--;
                item.AddPearl();
                RecalculateEquipBonuses();
                OnInventoryUpdated?.Invoke();
                OnEquipmentUpdated?.Invoke();
                return true;
            }
            return false;
        }

        public void UseConsumable(int backpackIndex)
        {
            if (backpackIndex < 0 || backpackIndex >= INVENTORY_SIZE) return;
            ItemData item = backpack[backpackIndex];
            if (item == null) return;

            bool consumed = false;
            if (item.isPermanentStatPotion)
            {
                if (stats != null)
                {
                    consumed = stats.DrinkStatPotion(item.permanentStat, item.permanentStatAmount);
                }
            }
            else if (item.healHP > 0 || item.healMP > 0)
            {
                if (stats != null)
                {
                    if (item.healHP > 0) stats.Heal(item.healHP);
                    if (item.healMP > 0) stats.RestoreMP(item.healMP);
                    consumed = true;
                }
            }

            if (consumed)
            {
                backpack[backpackIndex] = null;
                OnInventoryUpdated?.Invoke();
            }
        }

        public void UseHealthPotion()
        {
            if (healthPotions > 0 && stats != null)
            {
                if (stats.currentHP < stats.TotalMaxHP && !stats.HasStatus(StatusEffectType.Sick))
                {
                    healthPotions--;
                    stats.Heal(100);
                    OnInventoryUpdated?.Invoke();
                }
            }
        }

        public void UseMagicPotion()
        {
            if (magicPotions > 0 && stats != null)
            {
                if (stats.currentMP < stats.TotalMaxMP && !stats.HasStatus(StatusEffectType.Quiet))
                {
                    magicPotions--;
                    stats.RestoreMP(100);
                    OnInventoryUpdated?.Invoke();
                }
            }
        }

        public void RecalculateEquipBonuses()
        {
            StatBlock total = new StatBlock
            {
                maxHP = 0, maxMP = 0, attack = 0, defense = 0,
                speed = 0, dexterity = 0, vitality = 0, wisdom = 0
            };

            ItemData[] equipped = { weaponSlot, abilitySlot, armorSlot, ringSlot };
            foreach (var eq in equipped)
            {
                if (eq == null || eq.statBonus == null) continue;
                StatBlock eff = eq.GetEffectiveStatBonus();
                total.maxHP += eff.maxHP;
                total.maxMP += eff.maxMP;
                total.attack += eff.attack;
                total.defense += eff.defense;
                total.speed += eff.speed;
                total.dexterity += eff.dexterity;
                total.vitality += eff.vitality;
                total.wisdom += eff.wisdom;
            }

            if (stats != null)
            {
                stats.RecalculateEquipmentBonuses(total);
            }
        }
    }
}
