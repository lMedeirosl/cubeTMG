using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;
using CubeTMG.Items;
using CubeTMG.Core;

namespace CubeTMG.Enemies
{
    public class BossCubeGod : EnemyBase
    {
        [Header("Cube God Phases")]
        public float phaseDuration = 4f;
        private float phaseTimer = 0f;
        private int currentPhase = 0;

        private Transform coreCube;
        private readonly List<Transform> orbitCubes = new List<Transform>();

        protected override void Awake()
        {
            base.Awake();
            enemyName = "The Cube God";
            rank = EnemyRank.Boss;
            expReward = 850;
            moveSpeed = 1.8f;
            aggroRadius = 25f;
            attackRadius = 18f;
            whiteBagChance = 0.08f; // Higher chance for Demon Blade White Bag!
            statPotionChance = 0.85f; // Almost always drops Def pot

            // Setup Boss Stats
            stats.baseStats.maxHP = 2200;
            stats.baseStats.defense = 15;
            stats.currentHP = 2200;

            BuildCubeGodModel();
        }

        private void BuildCubeGodModel()
        {
            // Giant central translucent / glowing blue voxel cube
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
            core.name = "CubeGodCore";
            core.transform.SetParent(transform, false);
            core.transform.localPosition = new Vector3(0, 1.8f, 0);
            core.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            Destroy(core.GetComponent<BoxCollider>());

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            Material mat = new Material(shader);
            mat.color = new Color(0.1f, 0.6f, 1f, 0.9f);
            core.GetComponent<MeshRenderer>().material = mat;
            coreCube = core.transform;

            // 6 Orbiting guardian mini-cubes
            for (int i = 0; i < 6; i++)
            {
                GameObject mini = GameObject.CreatePrimitive(PrimitiveType.Cube);
                mini.name = $"OrbitCube_{i}";
                mini.transform.SetParent(transform, false);
                mini.transform.localScale = Vector3.one * 0.65f;
                Destroy(mini.GetComponent<BoxCollider>());

                Material miniMat = new Material(shader);
                miniMat.color = new Color(0.3f, 0.85f, 1f);
                mini.GetComponent<MeshRenderer>().material = miniMat;
                orbitCubes.Add(mini.transform);
            }

            // Big boss collider
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 1.8f, 0);
            col.radius = 1.6f;
        }

        protected override void Update()
        {
            base.Update();

            // Animate orbiting cubes and core rotation
            if (coreCube != null)
            {
                coreCube.Rotate(new Vector3(15f, 35f, 20f) * Time.deltaTime, Space.Self);
            }

            float orbitAngle = Time.time * 90f;
            for (int i = 0; i < orbitCubes.Count; i++)
            {
                float angle = orbitAngle + i * (360f / orbitCubes.Count);
                float rad = angle * Mathf.Deg2Rad;
                Vector3 orbitPos = new Vector3(Mathf.Cos(rad) * 2.8f, 1.8f + Mathf.Sin(Time.time * 2f + i) * 0.4f, Mathf.Sin(rad) * 2.8f);
                orbitCubes[i].localPosition = orbitPos;
                orbitCubes[i].Rotate(Vector3.up, 180f * Time.deltaTime);
            }

            // Phase Switching
            phaseTimer += Time.deltaTime;
            if (phaseTimer >= phaseDuration)
            {
                phaseTimer = 0f;
                currentPhase = (currentPhase + 1) % 3;
            }
        }

        protected override void ExecuteAttackPattern(Vector3 forwardDir)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.8f;
            SoundManager.Instance?.PlayEnemyShoot(spawnPos);

            switch (currentPhase)
            {
                case 0:
                    // Phase 0: 20-bullet Expanding Azure Nova Ring
                    float step = 360f / 20;
                    for (int i = 0; i < 20; i++)
                    {
                        Vector3 dir = Quaternion.Euler(0, i * step + Time.time * 20f, 0) * Vector3.forward;
                        Projectile p = ProjectilePool.Instance.Get();
                        p.Launch(spawnPos, dir, 32, 10f, 14f, false, new Color(0.1f, 0.7f, 1f), 0.45f);
                    }
                    break;

                case 1:
                    // Phase 1: High-damage shotgun burst aimed directly at player
                    int count = 5;
                    float gap = 9f;
                    float startAngle = -(count - 1) * gap * 0.5f;
                    for (int i = 0; i < count; i++)
                    {
                        Vector3 dir = Quaternion.Euler(0, startAngle + i * gap, 0) * forwardDir;
                        Projectile p = ProjectilePool.Instance.Get();
                        p.Launch(spawnPos, dir, 50, 15f, 16f, false, new Color(0.8f, 0.2f, 1f), 0.55f, false, true);
                    }
                    break;

                case 2:
                    // Phase 2: Double Spiral Laser Barrage
                    for (int s = 0; s < 2; s++)
                    {
                        float spiralOffset = (s * 180f) + (Time.time * 180f);
                        Vector3 dir = Quaternion.Euler(0, spiralOffset, 0) * Vector3.forward;
                        Projectile p = ProjectilePool.Instance.Get();
                        p.Launch(spawnPos, dir, 28, 12f, 14f, false, Color.cyan, 0.4f, false, false, true);
                    }
                    break;
            }
        }
    }
}
