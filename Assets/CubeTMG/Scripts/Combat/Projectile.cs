using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Core;

namespace CubeTMG.Combat
{
    public class Projectile : MonoBehaviour
    {
        public bool isPlayerOwned = true;
        public int damage = 20;
        public float speed = 15f;
        public float maxRange = 10f;
        public bool isPiercing = false;
        public bool isArmorPiercing = false;
        public bool isWavy = false;
        public float waveFrequency = 12f;
        public float waveAmplitude = 0.5f;

        public StatusEffectType inflictedStatus = StatusEffectType.None;
        public float statusDuration = 0f;

        private Vector3 spawnPosition;
        private Vector3 direction;
        private Vector3 lateralDir;
        private float distanceTraveled;
        private readonly HashSet<Collider> piercedTargets = new HashSet<Collider>();

        private MeshRenderer meshRenderer;
        private TrailRenderer trailRenderer;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
        }

        public void Launch(Vector3 origin, Vector3 dir, int dmg, float spd, float range, bool playerOwned,
            Color color, float size = 0.35f, bool pierce = false, bool armorPierce = false, bool wavy = false,
            StatusEffectType status = StatusEffectType.None, float statusDur = 0f)
        {
            transform.position = origin;
            spawnPosition = origin;
            direction = new Vector3(dir.x, 0, dir.z).normalized;
            lateralDir = Vector3.Cross(direction, Vector3.up).normalized;

            damage = dmg;
            speed = spd;
            maxRange = range;
            isPlayerOwned = playerOwned;
            isPiercing = pierce;
            isArmorPiercing = armorPierce;
            isWavy = wavy;
            inflictedStatus = status;
            statusDuration = statusDur;

            distanceTraveled = 0f;
            piercedTargets.Clear();

            transform.localScale = Vector3.one * size;
            transform.rotation = Quaternion.LookRotation(direction);

            if (meshRenderer != null)
            {
                meshRenderer.material.color = color;
            }

            if (trailRenderer != null)
            {
                trailRenderer.Clear();
                trailRenderer.startColor = color;
                Color end = color;
                end.a = 0f;
                trailRenderer.endColor = end;
                trailRenderer.startWidth = size * 0.8f;
                trailRenderer.endWidth = 0.05f;
            }

            gameObject.SetActive(true);
        }

        private void Update()
        {
            float step = speed * Time.deltaTime;
            distanceTraveled += step;

            if (distanceTraveled >= maxRange)
            {
                Expire();
                return;
            }

            Vector3 nextPos = spawnPosition + direction * distanceTraveled;
            if (isWavy)
            {
                float offset = Mathf.Sin(distanceTraveled * waveFrequency) * waveAmplitude;
                nextPos += lateralDir * offset;
            }

            transform.position = nextPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (piercedTargets.Contains(other)) return;

            if (isPlayerOwned)
            {
                // Check if hit enemy
                var enemyStats = other.GetComponentInParent<CharacterStats>();
                if (enemyStats != null && enemyStats.gameObject != null && !enemyStats.CompareTag("Player"))
                {
                    piercedTargets.Add(other);
                    int dealt = enemyStats.TakeDamage(damage, isArmorPiercing);
                    DamageTextManager.Instance?.SpawnDamage(enemyStats.transform.position, dealt, false);
                    SoundManager.Instance?.PlayHitEnemy(enemyStats.transform.position);

                    if (inflictedStatus != StatusEffectType.None)
                    {
                        enemyStats.ApplyStatus(inflictedStatus, statusDuration);
                        DamageTextManager.Instance?.SpawnStatus(enemyStats.transform.position, inflictedStatus.ToString().ToUpper() + "!", Color.yellow);
                    }

                    SpawnVoxelImpactSparks(transform.position, meshRenderer != null ? meshRenderer.material.color : Color.yellow);

                    if (!isPiercing)
                    {
                        Expire();
                    }
                }
            }
            else
            {
                // Enemy projectile hitting player
                if (other.CompareTag("Player") || other.GetComponentInParent<CharacterStats>()?.CompareTag("Player") == true)
                {
                    var playerStats = other.GetComponentInParent<CharacterStats>();
                    if (playerStats != null && !playerStats.IsDead)
                    {
                        int dealt = playerStats.TakeDamage(damage, isArmorPiercing);
                        DamageTextManager.Instance?.SpawnDamage(playerStats.transform.position, dealt, true);
                        SoundManager.Instance?.PlayPlayerHurt();

                        if (inflictedStatus != StatusEffectType.None)
                        {
                            playerStats.ApplyStatus(inflictedStatus, statusDuration);
                            DamageTextManager.Instance?.SpawnStatus(playerStats.transform.position, inflictedStatus.ToString().ToUpper() + "!", new Color(1f, 0.3f, 0.8f));
                        }

                        SpawnVoxelImpactSparks(transform.position, Color.red);
                        Expire();
                    }
                }
            }
        }

        private void Expire()
        {
            gameObject.SetActive(false);
        }

        private void SpawnVoxelImpactSparks(Vector3 pos, Color c)
        {
            // Tiny popping voxel particle sparks
            for (int i = 0; i < 4; i++)
            {
                GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Cube);
                spark.transform.position = pos;
                spark.transform.localScale = Vector3.one * 0.12f;
                var mr = spark.GetComponent<MeshRenderer>();
                if (mr != null) mr.material.color = c;
                var rb = spark.AddComponent<Rigidbody>();
                rb.linearVelocity = (Random.insideUnitSphere + Vector3.up) * 4f;
                Destroy(spark.GetComponent<BoxCollider>());
                Destroy(spark, 0.4f);
            }
        }
    }
}
