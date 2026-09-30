using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;
using CubeTMG.Items;
using CubeTMG.Core;

namespace CubeTMG.Enemies
{
    public class BossMedusa : EnemyBase
    {
        private float bombTimer = 0f;
        public float bombCooldown = 3.5f;

        protected override void Awake()
        {
            base.Awake();
            enemyName = "Medusa";
            rank = EnemyRank.God;
            expReward = 450;
            moveSpeed = 2.4f;
            aggroRadius = 20f;
            attackRadius = 14f;
            attackInterval = 0.9f;
            bulletDamage = 35;
            bulletColor = new Color(0.2f, 0.9f, 0.2f); // Green needle
            bulletsPerBurst = 3;
            bulletSpreadAngle = 8f;
            bulletSpeed = 16f;
            statPotionChance = 0.6f; // Drops Speed pot!

            stats.baseStats.maxHP = 900;
            stats.baseStats.defense = 8;
            stats.currentHP = 900;

            BuildMedusaModel();
        }

        private void BuildMedusaModel()
        {
            // Voxel snake-like torso & serpent head
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "MedusaBody";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0, 1.0f, 0);
            body.transform.localScale = new Vector3(0.8f, 1.4f, 0.7f);
            Destroy(body.GetComponent<BoxCollider>());

            Material mat = new Material(shader);
            mat.color = new Color(0.18f, 0.55f, 0.22f); // Emerald green scales
            body.GetComponent<MeshRenderer>().material = mat;

            // Head with red glowing eyes
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.name = "MedusaHead";
            head.transform.SetParent(transform, false);
            head.transform.localPosition = new Vector3(0, 2.0f, 0.1f);
            head.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            Destroy(head.GetComponent<BoxCollider>());
            head.GetComponent<MeshRenderer>().material = mat;

            // Red Eyes
            CreateEye(head.transform, new Vector3(-0.18f, 0.08f, 0.36f));
            CreateEye(head.transform, new Vector3(0.18f, 0.08f, 0.36f));

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 1.2f, 0);
            col.radius = 1.0f;
        }

        private void CreateEye(Transform parent, Vector3 localPos)
        {
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Cube);
            eye.transform.SetParent(parent, false);
            eye.transform.localPosition = localPos;
            eye.transform.localScale = new Vector3(0.12f, 0.12f, 0.05f);
            Destroy(eye.GetComponent<BoxCollider>());
            var mr = eye.GetComponent<MeshRenderer>();
            Material eyeMat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            eyeMat.color = Color.red;
            mr.material = eyeMat;
        }

        protected override void Update()
        {
            base.Update();

            if (stats.IsDead || targetPlayer == null) return;

            bombTimer -= Time.deltaTime;
            float dist = Vector3.Distance(transform.position, targetPlayer.position);
            if (dist <= attackRadius && bombTimer <= 0f)
            {
                bombTimer = bombCooldown;
                ThrowMedusaBomb(targetPlayer.position);
            }
        }

        private void ThrowMedusaBomb(Vector3 targetPos)
        {
            // Classic RotMG Medusa Bomb: Ticking red circle on ground that detonates after 1.2 seconds!
            GameObject bomb = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bomb.name = "MedusaBombMarker";
            bomb.transform.position = new Vector3(targetPos.x, 0.05f, targetPos.z);
            bomb.transform.localScale = new Vector3(3.5f, 0.02f, 3.5f);
            Destroy(bomb.GetComponent<Collider>());

            var mr = bomb.GetComponent<MeshRenderer>();
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default"));
            mat.color = new Color(1f, 0.1f, 0.1f, 0.6f);
            mr.material = mat;

            StartCoroutine(DetonateBomb(bomb, targetPos));
        }

        private System.Collections.IEnumerator DetonateBomb(GameObject bombMarker, Vector3 targetPos)
        {
            float timer = 1.2f;
            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                if (bombMarker != null)
                {
                    float pulse = 1f + Mathf.PingPong(Time.time * 6f, 0.25f);
                    bombMarker.transform.localScale = new Vector3(3.5f * pulse, 0.02f, 3.5f * pulse);
                }
                yield return null;
            }

            if (bombMarker != null)
            {
                Destroy(bombMarker);
            }

            // Boom!
            SoundManager.Instance?.PlaySpellbomb(targetPos);
            DamageTextManager.Instance?.SpawnStatus(targetPos, "MEDUSA BOMB!", Color.red);

            // Explode radial burst
            int ring = 12;
            float step = 360f / ring;
            for (int i = 0; i < ring; i++)
            {
                Vector3 dir = Quaternion.Euler(0, i * step, 0) * Vector3.forward;
                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(targetPos + Vector3.up * 0.5f, dir, 55, 14f, 2.5f, false, new Color(1f, 0.2f, 0.1f), 0.5f, true, true);
            }

            // Damage player if inside radius
            if (targetPlayer != null && Vector3.Distance(targetPlayer.position, targetPos) < 3.2f)
            {
                var pStats = targetPlayer.GetComponent<CharacterStats>();
                if (pStats != null && !pStats.IsDead)
                {
                    pStats.TakeDamage(75, false);
                    pStats.ApplyStatus(StatusEffectType.Sick, 4.0f); // Medusa bomb causes Sick!
                    DamageTextManager.Instance?.SpawnStatus(targetPlayer.position, "SICK! NO HEAL!", Color.magenta);
                }
            }
        }
    }
}
