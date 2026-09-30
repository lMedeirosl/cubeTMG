using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubeTMG.Stats
{
    [System.Serializable]
    public class StatBlock
    {
        public int maxHP = 100;
        public int maxMP = 100;
        public int attack = 10;
        public int defense = 0;
        public int speed = 10;
        public int dexterity = 10;
        public int vitality = 10;
        public int wisdom = 10;

        public StatBlock Clone()
        {
            return (StatBlock)MemberwiseClone();
        }
    }

    public class CharacterStats : MonoBehaviour
    {
        [Header("Class & Level")]
        public PlayerClassType characterClass = PlayerClassType.Wizard;
        public int level = 1;
        public int currentExp = 0;
        public int expToNextLevel = 100;
        public int totalFame = 0;

        [Header("Base Stats")]
        public StatBlock baseStats = new StatBlock();
        public StatBlock statCaps = new StatBlock();

        [Header("Bonus Equipment Stats")]
        public StatBlock equipBonus = new StatBlock();

        [Header("Current Vitals")]
        public float currentHP;
        public float currentMP;

        [Header("Status Effects")]
        public StatusEffectType activeStatuses = StatusEffectType.None;
        private readonly Dictionary<StatusEffectType, float> statusTimers = new Dictionary<StatusEffectType, float>();

        // Events
        public event Action<float, float> OnHPChanged;
        public event Action<float, float> OnMPChanged;
        public event Action<int> OnLevelUp;
        public event Action<int, bool, bool> OnDamageTaken; // damage, isCrit, isArmorPiercing
        public event Action<int> OnHealed;
        public event Action OnDeath;
        public event Action OnStatsRecalculated;

        public bool IsDead => currentHP <= 0;

        public int TotalMaxHP => baseStats.maxHP + equipBonus.maxHP;
        public int TotalMaxMP => baseStats.maxMP + equipBonus.maxMP;
        public int TotalAttack => baseStats.attack + equipBonus.attack;
        public int TotalDefense => baseStats.defense + equipBonus.defense;
        public int TotalSpeed => baseStats.speed + equipBonus.speed;
        public int TotalDexterity => baseStats.dexterity + equipBonus.dexterity;
        public int TotalVitality => baseStats.vitality + equipBonus.vitality;
        public int TotalWisdom => baseStats.wisdom + equipBonus.wisdom;

        private void Awake()
        {
            InitializeForClass(characterClass);
        }

        private void Update()
        {
            if (IsDead) return;

            UpdateStatusEffects(Time.deltaTime);
            HandleRegeneration(Time.deltaTime);
        }

        public void InitializeForClass(PlayerClassType pClass)
        {
            characterClass = pClass;
            level = 1;
            currentExp = 0;
            expToNextLevel = 100;
            statusTimers.Clear();
            activeStatuses = StatusEffectType.None;

            switch (pClass)
            {
                case PlayerClassType.Wizard:
                    baseStats.maxHP = 100;
                    baseStats.maxMP = 100;
                    baseStats.attack = 12;
                    baseStats.defense = 0;
                    baseStats.speed = 10;
                    baseStats.dexterity = 12;
                    baseStats.vitality = 10;
                    baseStats.wisdom = 12;

                    statCaps.maxHP = 670;
                    statCaps.maxMP = 385;
                    statCaps.attack = 75;
                    statCaps.defense = 25;
                    statCaps.speed = 50;
                    statCaps.dexterity = 75;
                    statCaps.vitality = 40;
                    statCaps.wisdom = 60;
                    break;

                case PlayerClassType.Warrior:
                    baseStats.maxHP = 150;
                    baseStats.maxMP = 80;
                    baseStats.attack = 15;
                    baseStats.defense = 5;
                    baseStats.speed = 8;
                    baseStats.dexterity = 10;
                    baseStats.vitality = 12;
                    baseStats.wisdom = 8;

                    statCaps.maxHP = 770;
                    statCaps.maxMP = 252;
                    statCaps.attack = 75;
                    statCaps.defense = 40;
                    statCaps.speed = 50;
                    statCaps.dexterity = 50;
                    statCaps.vitality = 75;
                    statCaps.wisdom = 50;
                    break;

                case PlayerClassType.Priest:
                    baseStats.maxHP = 100;
                    baseStats.maxMP = 120;
                    baseStats.attack = 10;
                    baseStats.defense = 0;
                    baseStats.speed = 10;
                    baseStats.dexterity = 10;
                    baseStats.vitality = 12;
                    baseStats.wisdom = 15;

                    statCaps.maxHP = 670;
                    statCaps.maxMP = 385;
                    statCaps.attack = 50;
                    statCaps.defense = 25;
                    statCaps.speed = 55;
                    statCaps.dexterity = 55;
                    statCaps.vitality = 55;
                    statCaps.wisdom = 75;
                    break;

                case PlayerClassType.Archer:
                    baseStats.maxHP = 110;
                    baseStats.maxMP = 90;
                    baseStats.attack = 12;
                    baseStats.defense = 0;
                    baseStats.speed = 12;
                    baseStats.dexterity = 12;
                    baseStats.vitality = 10;
                    baseStats.wisdom = 10;

                    statCaps.maxHP = 700;
                    statCaps.maxMP = 252;
                    statCaps.attack = 75;
                    statCaps.defense = 25;
                    statCaps.speed = 55;
                    statCaps.dexterity = 50;
                    statCaps.vitality = 40;
                    statCaps.wisdom = 50;
                    break;

                case PlayerClassType.Knight:
                    baseStats.maxHP = 160;
                    baseStats.maxMP = 70;
                    baseStats.attack = 14;
                    baseStats.defense = 8;
                    baseStats.speed = 7;
                    baseStats.dexterity = 10;
                    baseStats.vitality = 12;
                    baseStats.wisdom = 8;

                    statCaps.maxHP = 770;
                    statCaps.maxMP = 252;
                    statCaps.attack = 50;
                    statCaps.defense = 40;
                    statCaps.speed = 50;
                    statCaps.dexterity = 50;
                    statCaps.vitality = 75;
                    statCaps.wisdom = 50;
                    break;
            }

            currentHP = TotalMaxHP;
            currentMP = TotalMaxMP;
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);
            OnMPChanged?.Invoke(currentMP, TotalMaxMP);
            OnStatsRecalculated?.Invoke();
        }

        private void HandleRegeneration(float dt)
        {
            // RotMG HP Regeneration: (1 + 0.12 * VIT) hp/sec
            // Sick blocks all healing. Bleeding causes continuous 20 damage/sec
            if (HasStatus(StatusEffectType.Bleeding))
            {
                TakeDamageWithoutArmor(Mathf.RoundToInt(20f * dt));
            }

            if (!HasStatus(StatusEffectType.Sick))
            {
                float hpRegenRate = 1f + (0.12f * TotalVitality);
                if (HasStatus(StatusEffectType.Healing))
                {
                    hpRegenRate += 20f;
                }

                if (currentHP < TotalMaxHP)
                {
                    currentHP = Mathf.Min(TotalMaxHP, currentHP + hpRegenRate * dt);
                    OnHPChanged?.Invoke(currentHP, TotalMaxHP);
                }
            }

            // RotMG MP Regeneration: (0.5 + 0.06 * WIS) mp/sec
            // Quiet blocks MP regen and drains MP to 0
            if (HasStatus(StatusEffectType.Quiet))
            {
                currentMP = 0f;
                OnMPChanged?.Invoke(currentMP, TotalMaxMP);
            }
            else
            {
                float mpRegenRate = 0.5f + (0.06f * TotalWisdom);
                if (currentMP < TotalMaxMP)
                {
                    currentMP = Mathf.Min(TotalMaxMP, currentMP + mpRegenRate * dt);
                    OnMPChanged?.Invoke(currentMP, TotalMaxMP);
                }
            }
        }

        private void UpdateStatusEffects(float dt)
        {
            var keys = new List<StatusEffectType>(statusTimers.Keys);
            foreach (var status in keys)
            {
                statusTimers[status] -= dt;
                if (statusTimers[status] <= 0f)
                {
                    statusTimers.Remove(status);
                    activeStatuses &= ~status;
                }
            }
        }

        public void ApplyStatus(StatusEffectType status, float duration)
        {
            if (status == StatusEffectType.None) return;

            if (statusTimers.ContainsKey(status))
            {
                statusTimers[status] = Mathf.Max(statusTimers[status], duration);
            }
            else
            {
                statusTimers[status] = duration;
            }

            activeStatuses |= status;

            if (status == StatusEffectType.Quiet)
            {
                currentMP = 0f;
                OnMPChanged?.Invoke(currentMP, TotalMaxMP);
            }
        }

        public bool HasStatus(StatusEffectType status)
        {
            return (activeStatuses & status) != 0;
        }

        public int TakeDamage(int rawDamage, bool armorPiercing = false)
        {
            if (IsDead) return 0;
            if (HasStatus(StatusEffectType.Invulnerable)) return 0;

            int effectiveDef = armorPiercing ? 0 : (HasStatus(StatusEffectType.Armored) ? TotalDefense * 2 : TotalDefense);
            // RotMG damage formula: min damage is 15% of incoming damage or at least 1
            int minDamage = Mathf.Max(1, Mathf.RoundToInt(rawDamage * 0.15f));
            int finalDamage = Mathf.Max(minDamage, rawDamage - effectiveDef);

            currentHP = Mathf.Max(0, currentHP - finalDamage);
            OnDamageTaken?.Invoke(finalDamage, false, armorPiercing);
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);

            if (currentHP <= 0)
            {
                Die();
            }

            return finalDamage;
        }

        public void TakeDamageWithoutArmor(int damage)
        {
            if (IsDead || damage <= 0) return;
            currentHP = Mathf.Max(0, currentHP - damage);
            OnDamageTaken?.Invoke(damage, false, true);
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);

            if (currentHP <= 0)
            {
                Die();
            }
        }

        public void Heal(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (HasStatus(StatusEffectType.Sick)) return; // Sick prevents healing

            currentHP = Mathf.Min(TotalMaxHP, currentHP + amount);
            OnHealed?.Invoke(amount);
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);
        }

        public bool ConsumeMP(int amount)
        {
            if (currentMP >= amount)
            {
                currentMP -= amount;
                OnMPChanged?.Invoke(currentMP, TotalMaxMP);
                return true;
            }
            return false;
        }

        public void RestoreMP(int amount)
        {
            if (IsDead || amount <= 0) return;
            if (HasStatus(StatusEffectType.Quiet)) return;

            currentMP = Mathf.Min(TotalMaxMP, currentMP + amount);
            OnMPChanged?.Invoke(currentMP, TotalMaxMP);
        }

        public void AddExp(int amount)
        {
            if (level >= 20)
            {
                totalFame += Mathf.RoundToInt(amount * 0.05f);
                return;
            }

            currentExp += amount;
            while (currentExp >= expToNextLevel && level < 20)
            {
                currentExp -= expToNextLevel;
                LevelUp();
            }
        }

        private void LevelUp()
        {
            level++;
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * 1.35f);

            // RotMG stat growths per level
            baseStats.maxHP += UnityEngine.Random.Range(20, 32);
            baseStats.maxMP += UnityEngine.Random.Range(10, 18);
            baseStats.attack += UnityEngine.Random.Range(1, 3);
            baseStats.defense += UnityEngine.Random.Range(0, 2);
            baseStats.speed += UnityEngine.Random.Range(1, 3);
            baseStats.dexterity += UnityEngine.Random.Range(1, 3);
            baseStats.vitality += UnityEngine.Random.Range(1, 2);
            baseStats.wisdom += UnityEngine.Random.Range(1, 2);

            ClampToBaseCaps();

            // Fully restore HP & MP upon level up
            currentHP = TotalMaxHP;
            currentMP = TotalMaxMP;

            OnLevelUp?.Invoke(level);
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);
            OnMPChanged?.Invoke(currentMP, TotalMaxMP);
            OnStatsRecalculated?.Invoke();
        }

        public bool DrinkStatPotion(StatType stat, int amount = 1)
        {
            bool success = false;
            switch (stat)
            {
                case StatType.MaxHP:
                    if (baseStats.maxHP < statCaps.maxHP)
                    {
                        baseStats.maxHP = Mathf.Min(statCaps.maxHP, baseStats.maxHP + 5 * amount);
                        currentHP += 5 * amount;
                        success = true;
                    }
                    break;
                case StatType.MaxMP:
                    if (baseStats.maxMP < statCaps.maxMP)
                    {
                        baseStats.maxMP = Mathf.Min(statCaps.maxMP, baseStats.maxMP + 5 * amount);
                        currentMP += 5 * amount;
                        success = true;
                    }
                    break;
                case StatType.Attack:
                    if (baseStats.attack < statCaps.attack)
                    {
                        baseStats.attack = Mathf.Min(statCaps.attack, baseStats.attack + amount);
                        success = true;
                    }
                    break;
                case StatType.Defense:
                    if (baseStats.defense < statCaps.defense)
                    {
                        baseStats.defense = Mathf.Min(statCaps.defense, baseStats.defense + amount);
                        success = true;
                    }
                    break;
                case StatType.Speed:
                    if (baseStats.speed < statCaps.speed)
                    {
                        baseStats.speed = Mathf.Min(statCaps.speed, baseStats.speed + amount);
                        success = true;
                    }
                    break;
                case StatType.Dexterity:
                    if (baseStats.dexterity < statCaps.dexterity)
                    {
                        baseStats.dexterity = Mathf.Min(statCaps.dexterity, baseStats.dexterity + amount);
                        success = true;
                    }
                    break;
                case StatType.Vitality:
                    if (baseStats.vitality < statCaps.vitality)
                    {
                        baseStats.vitality = Mathf.Min(statCaps.vitality, baseStats.vitality + amount);
                        success = true;
                    }
                    break;
                case StatType.Wisdom:
                    if (baseStats.wisdom < statCaps.wisdom)
                    {
                        baseStats.wisdom = Mathf.Min(statCaps.wisdom, baseStats.wisdom + amount);
                        success = true;
                    }
                    break;
            }

            if (success)
            {
                OnStatsRecalculated?.Invoke();
                OnHPChanged?.Invoke(currentHP, TotalMaxHP);
                OnMPChanged?.Invoke(currentMP, TotalMaxMP);
            }
            return success;
        }

        public bool IsStatMaxed(StatType stat)
        {
            switch (stat)
            {
                case StatType.MaxHP: return baseStats.maxHP >= statCaps.maxHP;
                case StatType.MaxMP: return baseStats.maxMP >= statCaps.maxMP;
                case StatType.Attack: return baseStats.attack >= statCaps.attack;
                case StatType.Defense: return baseStats.defense >= statCaps.defense;
                case StatType.Speed: return baseStats.speed >= statCaps.speed;
                case StatType.Dexterity: return baseStats.dexterity >= statCaps.dexterity;
                case StatType.Vitality: return baseStats.vitality >= statCaps.vitality;
                case StatType.Wisdom: return baseStats.wisdom >= statCaps.wisdom;
                default: return false;
            }
        }

        public int CountMaxedStats()
        {
            int count = 0;
            if (IsStatMaxed(StatType.MaxHP)) count++;
            if (IsStatMaxed(StatType.MaxMP)) count++;
            if (IsStatMaxed(StatType.Attack)) count++;
            if (IsStatMaxed(StatType.Defense)) count++;
            if (IsStatMaxed(StatType.Speed)) count++;
            if (IsStatMaxed(StatType.Dexterity)) count++;
            if (IsStatMaxed(StatType.Vitality)) count++;
            if (IsStatMaxed(StatType.Wisdom)) count++;
            return count; // 8/8 maxed!
        }

        private void ClampToBaseCaps()
        {
            baseStats.maxHP = Mathf.Min(baseStats.maxHP, statCaps.maxHP);
            baseStats.maxMP = Mathf.Min(baseStats.maxMP, statCaps.maxMP);
            baseStats.attack = Mathf.Min(baseStats.attack, statCaps.attack);
            baseStats.defense = Mathf.Min(baseStats.defense, statCaps.defense);
            baseStats.speed = Mathf.Min(baseStats.speed, statCaps.speed);
            baseStats.dexterity = Mathf.Min(baseStats.dexterity, statCaps.dexterity);
            baseStats.vitality = Mathf.Min(baseStats.vitality, statCaps.vitality);
            baseStats.wisdom = Mathf.Min(baseStats.wisdom, statCaps.wisdom);
        }

        public float CalculateBulletDamage(int baseWeaponDamage)
        {
            // RotMG Formula: Damage * (0.5 + Attack / 50)
            float dmg = baseWeaponDamage * (0.5f + (TotalAttack / 50f));
            if (HasStatus(StatusEffectType.Damaging)) dmg *= 1.5f;
            return dmg;
        }

        public float CalculateAttacksPerSecond(float weaponBaseRateOfFire)
        {
            // RotMG Formula: BaseRoF * (1.5 + 6.5 * (Dexterity / 75))
            float aps = weaponBaseRateOfFire * (1.5f + 6.5f * (TotalDexterity / 75f));
            if (HasStatus(StatusEffectType.Berserk)) aps *= 1.5f;
            if (HasStatus(StatusEffectType.Dazed)) aps *= 0.5f;
            return aps;
        }

        public float CalculateMoveSpeed()
        {
            if (HasStatus(StatusEffectType.Paralyzed)) return 0f;

            // RotMG tiles/second scaled to Unity units (~4.5f base to 10f max)
            float speed = 5.0f + (TotalSpeed / 75f) * 6.0f;
            if (HasStatus(StatusEffectType.Speedy)) speed *= 1.5f;
            return speed;
        }

        private void Die()
        {
            currentHP = 0;
            totalFame += level * 10 + CountMaxedStats() * 50;
            OnDeath?.Invoke();
        }

        public void RecalculateEquipmentBonuses(StatBlock bonus)
        {
            equipBonus = bonus.Clone();
            currentHP = Mathf.Min(currentHP, TotalMaxHP);
            currentMP = Mathf.Min(currentMP, TotalMaxMP);
            OnStatsRecalculated?.Invoke();
            OnHPChanged?.Invoke(currentHP, TotalMaxHP);
            OnMPChanged?.Invoke(currentMP, TotalMaxMP);
        }
    }
}
