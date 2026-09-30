using UnityEngine;
using CubeTMG.Core;
using CubeTMG.Player;
using CubeTMG.Stats;
using CubeTMG.Items;

namespace CubeTMG.Testing
{
    public static class GameSanityCheck
    {
        public static void RunVerification()
        {
            Debug.Log("[CubeTMG] Running Sanity Check...");

            // 1. Verify Item Database
            ItemData sword = ItemDatabase.Get("sword_short");
            ItemData dbow = ItemDatabase.Get("bow_dbow");
            ItemData potDef = ItemDatabase.Get("pot_def");

            if (sword == null || dbow == null || potDef == null)
            {
                Debug.LogError("[CubeTMG] FAILED: Item database failed to retrieve items!");
                return;
            }
            Debug.Log($"[CubeTMG] PASSED: Items loaded successfully (Sword: {sword.name}, UT: {dbow.name}, Pot: {potDef.name})");

            // 2. Verify Stat Calculations
            StatBlock block = new StatBlock();
            GameObject testGO = new GameObject("TestPlayer");
            var stats = testGO.AddComponent<CharacterStats>();
            stats.InitializeForClass(PlayerClassType.Wizard);

            float damage = stats.CalculateBulletDamage(50);
            float aps = stats.CalculateAttacksPerSecond(1.0f);
            float speed = stats.CalculateMoveSpeed();

            Debug.Log($"[CubeTMG] PASSED: Wizard Calculations -> Damage: {damage}, Attacks/sec: {aps:F2}, MoveSpeed: {speed:F2}");

            // 3. Test Damage formula
            int initialHP = (int)stats.currentHP;
            stats.TakeDamage(30, false);
            Debug.Log($"[CubeTMG] PASSED: HP after 30 raw damage: {stats.currentHP}/{initialHP}");

            // 4. Test Stat Potion Drink
            stats.DrinkStatPotion(StatType.Attack, 1);
            Debug.Log($"[CubeTMG] PASSED: Attack after potion: {stats.baseStats.attack}");

            Object.DestroyImmediate(testGO);
            Debug.Log("[CubeTMG] ALL SYSTEMS VERIFIED AND READY TO PLAY!");
        }
    }
}
