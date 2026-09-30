using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;
using CubeTMG.Items;
using CubeTMG.Core;
using CubeTMG.Player;

namespace CubeTMG.Enemies
{
    public enum EnemyRank
    {
        Minion,
        God,
        Boss
    }

    [RequireComponent(typeof(CharacterStats))]
    public class EnemyBase : MonoBehaviour
    {
        public string enemyName = "Goblin";
        public EnemyRank rank = EnemyRank.Minion;
        public int expReward = 25;
        public int fluxReward = 15;
        public float moveSpeed = 3.5f;
        public float aggroRadius = 14f;
        public float attackRadius = 10f;
        public float attackInterval = 1.2f;

        [Header("Bullet Hell Pattern")]
        public int bulletsPerBurst = 1;
        public float bulletSpreadAngle = 0f;
        public float bulletSpeed = 12f;
        public float bulletRange = 10f;
        public int bulletDamage = 20;
        public Color bulletColor = Color.red;
        public float bulletSize = 0.35f;
        public bool isRingNova = false;
        public int ringCount = 12;
        public StatusEffectType inflictedStatus = StatusEffectType.None;
        public float statusDuration = 0f;

        [Header("Loot Drop Chances")]
        [Range(0f, 1f)] public float dropBagChance = 0.65f;
        [Range(0f, 1f)] public float statPotionChance = 0.15f;
        [Range(0f, 1f)] public float whiteBagChance = 0.03f;

        protected CharacterStats stats;
        protected Transform targetPlayer;
        protected float attackTimer;
        protected Rigidbody rb;

        protected virtual void Awake()
        {
            stats = GetComponent<CharacterStats>();
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            stats.OnDeath += HandleDeath;
        }

        protected virtual void Start()
        {
            if (PlayerController.Instance != null)
            {
                targetPlayer = PlayerController.Instance.transform;
            }
        }

        protected virtual void Update()
        {
            if (stats.IsDead) return;

            if (targetPlayer == null && PlayerController.Instance != null)
            {
                targetPlayer = PlayerController.Instance.transform;
            }

            attackTimer -= Time.deltaTime;

            if (targetPlayer != null)
            {
                float dist = Vector3.Distance(transform.position, targetPlayer.position);
                if (dist <= aggroRadius)
                {
                    HandleAI(dist);
                }
            }
        }

        protected virtual void HandleAI(float distanceToPlayer)
        {
            Vector3 lookDir = (targetPlayer.position - transform.position);
            lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), 6f * Time.deltaTime);
            }

            if (!stats.HasStatus(StatusEffectType.Paralyzed))
            {
                if (distanceToPlayer > attackRadius * 0.7f)
                {
                    transform.position += lookDir.normalized * moveSpeed * Time.deltaTime;
                }
            }

            if (distanceToPlayer <= attackRadius && attackTimer <= 0f && !stats.HasStatus(StatusEffectType.Stunned))
            {
                attackTimer = attackInterval;
                ExecuteAttackPattern(lookDir.normalized);
            }
        }

        protected virtual void ExecuteAttackPattern(Vector3 forwardDir)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.7f;
            SoundManager.Instance?.PlayEnemyShoot(spawnPos);

            if (isRingNova)
            {
                float step = 360f / ringCount;
                for (int i = 0; i < ringCount; i++)
                {
                    Vector3 dir = Quaternion.Euler(0, i * step, 0) * Vector3.forward;
                    Projectile p = ProjectilePool.Instance.Get();
                    p.Launch(spawnPos, dir, bulletDamage, bulletSpeed, bulletRange, false, bulletColor, bulletSize,
                        false, false, false, inflictedStatus, statusDuration);
                }
            }
            else
            {
                float startAngle = -(bulletsPerBurst - 1) * bulletSpreadAngle * 0.5f;
                for (int i = 0; i < bulletsPerBurst; i++)
                {
                    Vector3 dir = Quaternion.Euler(0, startAngle + i * bulletSpreadAngle, 0) * forwardDir;
                    Projectile p = ProjectilePool.Instance.Get();
                    p.Launch(spawnPos, dir, bulletDamage, bulletSpeed, bulletRange, false, bulletColor, bulletSize,
                        false, false, false, inflictedStatus, statusDuration);
                }
            }
        }

        protected virtual void HandleDeath()
        {
            if (targetPlayer != null)
            {
                var pStats = targetPlayer.GetComponent<CharacterStats>();
                pStats?.AddExp(expReward);

                var pInv = targetPlayer.GetComponent<Inventory>();
                if (pInv != null)
                {
                    pInv.flux += fluxReward;
                    DamageTextManager.Instance?.SpawnText(transform.position + Vector3.up * 0.5f, $"+{fluxReward} Flux", new Color(1f, 0.85f, 0.2f), 1.1f);
                }
            }

            RollAndDropLoot();
            SpawnVoxelExplosion();
            Destroy(gameObject);
        }

        private void RollAndDropLoot()
        {
            if (Random.value > dropBagChance) return;

            List<ItemData> drops = new List<ItemData>();
            BagTier highestTier = BagTier.Brown;

            // Roll White Bag UT / Stellar / Crystal
            if (Random.value <= whiteBagChance)
            {
                highestTier = BagTier.White;
                string[] utPool = { "bow_dbow", "sword_dblade", "staff_ep", "helm_jugg", "ring_pyra" };
                ItemData utItem = ItemDatabase.Get(utPool[Random.Range(0, utPool.Length)]);
                utItem.troveRarity = Random.value > 0.4f ? TroveRarity.Stellar : TroveRarity.Crystal;
                utItem.stars = Random.Range(2, 5);
                utItem.pearlsOfWisdom = Random.Range(1, 4);
                drops.Add(utItem);
            }

            // Roll Stat Potion (Blue Bag)
            if (Random.value <= statPotionChance)
            {
                if (highestTier < BagTier.Blue) highestTier = BagTier.Blue;
                string[] potPool = { "pot_att", "pot_def", "pot_spd", "pot_dex", "pot_vit", "pot_wis" };
                drops.Add(ItemDatabase.Get(potPool[Random.Range(0, potPool.Length)]));
            }

            // Regular gear drops with Trove rarity rolls
            if (rank == EnemyRank.Minion)
            {
                string[] minionPool = { "hp_pot", "mp_pot", "sword_short", "staff_energy", "bow_short", "wand_fire" };
                ItemData item = ItemDatabase.Get(minionPool[Random.Range(0, minionPool.Length)]);
                if (item.itemType != ItemType.Consumable)
                {
                    item.troveRarity = RollTroveRarityForRank(EnemyRank.Minion);
                    item.stars = Random.Range(0, 2);
                }
                drops.Add(item);
            }
            else if (rank == EnemyRank.God)
            {
                if (highestTier < BagTier.Purple) highestTier = BagTier.Purple;
                string[] godPool = { "sword_archon", "staff_destruction", "bow_covert", "ring_def_para", "hp_pot" };
                ItemData item = ItemDatabase.Get(godPool[Random.Range(0, godPool.Length)]);
                if (item.itemType != ItemType.Consumable)
                {
                    item.troveRarity = RollTroveRarityForRank(EnemyRank.God);
                    item.stars = Random.Range(1, 3);
                    if (highestTier < BagTier.Cyan && item.troveRarity >= TroveRarity.Relic) highestTier = BagTier.Cyan;
                }
                drops.Add(item);
            }
            else if (rank == EnemyRank.Boss)
            {
                if (highestTier < BagTier.Cyan) highestTier = BagTier.Cyan;
                string[] bossPool = { "staff_cosmic", "armor_acropolis", "ring_def_para", "pot_life", "pot_mana" };
                ItemData item = ItemDatabase.Get(bossPool[Random.Range(0, bossPool.Length)]);
                if (item.itemType != ItemType.Consumable)
                {
                    item.troveRarity = RollTroveRarityForRank(EnemyRank.Boss);
                    item.stars = Random.Range(2, 4);
                    item.pearlsOfWisdom = Random.Range(1, 3);
                }
                drops.Add(item);
            }

            if (drops.Count > 0)
            {
                LootBag.SpawnBag(transform.position, drops);
            }
        }

        private TroveRarity RollTroveRarityForRank(EnemyRank r)
        {
            float roll = Random.value;
            if (r == EnemyRank.Minion)
            {
                if (roll < 0.60f) return TroveRarity.Common;
                if (roll < 0.85f) return TroveRarity.Uncommon;
                if (roll < 0.96f) return TroveRarity.Rare;
                return TroveRarity.Epic;
            }
            else if (r == EnemyRank.God)
            {
                if (roll < 0.30f) return TroveRarity.Rare;
                if (roll < 0.60f) return TroveRarity.Epic;
                if (roll < 0.85f) return TroveRarity.Legendary;
                if (roll < 0.95f) return TroveRarity.Relic;
                return TroveRarity.Shadow;
            }
            else // Boss
            {
                if (roll < 0.25f) return TroveRarity.Relic;
                if (roll < 0.55f) return TroveRarity.Resplendent;
                if (roll < 0.80f) return TroveRarity.Shadow;
                if (roll < 0.94f) return TroveRarity.Radiant;
                return TroveRarity.Stellar;
            }
        }

        private void SpawnVoxelExplosion()
        {
            Color expColor = bulletColor;
            for (int i = 0; i < 10; i++)
            {
                GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chunk.transform.position = transform.position + Random.insideUnitSphere * 0.4f;
                chunk.transform.localScale = Vector3.one * Random.Range(0.15f, 0.35f);
                var mr = chunk.GetComponent<MeshRenderer>();
                if (mr != null) mr.material.color = expColor;
                var rbd = chunk.AddComponent<Rigidbody>();
                rbd.linearVelocity = (Random.insideUnitSphere + Vector3.up * 1.5f) * 6f;
                Destroy(chunk.GetComponent<BoxCollider>());
                Destroy(chunk, 0.8f);
            }
        }
    }
}
