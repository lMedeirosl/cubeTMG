using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;

namespace CubeTMG.Enemies
{
    // ==========================================
    // CANDORIA (CANDYLAND) ENEMIES
    // ==========================================
    public class CandyGummyBear : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            enemyName = "Gummy Bear";
            rank = EnemyRank.Minion;
            expReward = 55;
            fluxReward = 20;
            moveSpeed = 4.8f;
            aggroRadius = 16f;
            attackRadius = 8f;
            attackInterval = 0.9f;
            bulletDamage = 22;
            bulletColor = new Color(1f, 0.35f, 0.7f); // Bubblegum pink
            bulletSpeed = 15f;
            stats.baseStats.maxHP = 220;
            stats.baseStats.defense = 4;
            stats.currentHP = 220;

            BuildGummyBearModel();
        }

        private void BuildGummyBearModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Color gummyColor = new Color(1f, 0.25f, 0.55f);

            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "GummyBody";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0, 0.6f, 0);
            body.transform.localScale = new Vector3(0.7f, 0.9f, 0.5f);
            Destroy(body.GetComponent<BoxCollider>());
            Material m = new Material(s) { color = gummyColor };
            body.GetComponent<MeshRenderer>().material = m;

            // Cute bear ears
            CreateEar(body.transform, new Vector3(-0.25f, 0.55f, 0), gummyColor, s);
            CreateEar(body.transform, new Vector3(0.25f, 0.55f, 0), gummyColor, s);

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 0.6f, 0);
            col.radius = 0.6f;
        }

        private void CreateEar(Transform parent, Vector3 localPos, Color c, Shader s)
        {
            GameObject ear = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ear.transform.SetParent(parent, false);
            ear.transform.localPosition = localPos;
            ear.transform.localScale = new Vector3(0.2f, 0.2f, 0.15f);
            Destroy(ear.GetComponent<BoxCollider>());
            ear.GetComponent<MeshRenderer>().material = new Material(s) { color = c };
        }
    }

    public class BossCupcakeKing : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            enemyName = "The Cupcake King";
            rank = EnemyRank.Boss;
            expReward = 950;
            fluxReward = 120;
            moveSpeed = 1.6f;
            aggroRadius = 24f;
            attackRadius = 18f;
            statPotionChance = 0.9f;
            whiteBagChance = 0.09f;

            stats.baseStats.maxHP = 2600;
            stats.baseStats.defense = 14;
            stats.currentHP = 2600;

            BuildCupcakeModel();
        }

        private void BuildCupcakeModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            // Base cupcake pastry
            GameObject basePastry = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            basePastry.name = "Pastry";
            basePastry.transform.SetParent(transform, false);
            basePastry.transform.localPosition = new Vector3(0, 0.7f, 0);
            basePastry.transform.localScale = new Vector3(2.4f, 0.7f, 2.4f);
            Destroy(basePastry.GetComponent<Collider>());
            Material pMat = new Material(s) { color = new Color(0.85f, 0.65f, 0.4f) };
            basePastry.GetComponent<MeshRenderer>().material = pMat;

            // Swirl pink frosting dome
            GameObject frosting = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            frosting.name = "Frosting";
            frosting.transform.SetParent(transform, false);
            frosting.transform.localPosition = new Vector3(0, 1.8f, 0);
            frosting.transform.localScale = new Vector3(2.6f, 1.8f, 2.6f);
            Destroy(frosting.GetComponent<Collider>());
            Material fMat = new Material(s) { color = new Color(1f, 0.4f, 0.75f) };
            frosting.GetComponent<MeshRenderer>().material = fMat;

            // Cherry on top
            GameObject cherry = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cherry.transform.SetParent(transform, false);
            cherry.transform.localPosition = new Vector3(0, 2.9f, 0);
            cherry.transform.localScale = Vector3.one * 0.7f;
            Destroy(cherry.GetComponent<Collider>());
            cherry.GetComponent<MeshRenderer>().material = new Material(s) { color = Color.red };

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 1.6f, 0);
            col.radius = 1.6f;
        }

        protected override void ExecuteAttackPattern(Vector3 forwardDir)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 1.8f;
            Core.SoundManager.Instance?.PlayEnemyShoot(spawnPos);

            // Phase 1: 18-bullet Sugar Star Nova Ring
            int count = 18;
            float step = 360f / count;
            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Quaternion.Euler(0, i * step + Time.time * 30f, 0) * Vector3.forward;
                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(spawnPos, dir, 35, 11f, 16f, false, new Color(1f, 0.3f, 0.8f), 0.5f);
            }

            // Phase 2: Chocolate Cone Blast
            int cone = 4;
            float gap = 12f;
            float startAngle = -(cone - 1) * gap * 0.5f;
            for (int i = 0; i < cone; i++)
            {
                Vector3 dir = Quaternion.Euler(0, startAngle + i * gap, 0) * forwardDir;
                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(spawnPos, dir, 52, 14f, 18f, false, new Color(0.45f, 0.25f, 0.1f), 0.6f, false, true);
            }
        }
    }

    // ==========================================
    // NEON CITY ENEMIES
    // ==========================================
    public class NeonDrone : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            enemyName = "Neon Cyber Drone";
            rank = EnemyRank.Minion;
            expReward = 70;
            fluxReward = 25;
            moveSpeed = 4.2f;
            aggroRadius = 18f;
            attackRadius = 12f;
            attackInterval = 0.8f;
            bulletDamage = 26;
            bulletColor = Color.cyan;
            bulletSpeed = 22f; // Fast lasers!
            stats.baseStats.maxHP = 260;
            stats.baseStats.defense = 6;
            stats.currentHP = 260;

            BuildDroneModel();
        }

        private void BuildDroneModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            GameObject core = GameObject.CreatePrimitive(PrimitiveType.Cube);
            core.transform.SetParent(transform, false);
            core.transform.localPosition = new Vector3(0, 1.2f, 0);
            core.transform.localScale = new Vector3(0.8f, 0.3f, 0.8f);
            Destroy(core.GetComponent<BoxCollider>());
            core.GetComponent<MeshRenderer>().material = new Material(s) { color = new Color(0.1f, 0.12f, 0.15f) };

            // Glowing cyan ring
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.transform.SetParent(core.transform, false);
            eye.transform.localPosition = new Vector3(0, 0, 0.35f);
            eye.transform.localScale = Vector3.one * 0.45f;
            Destroy(eye.GetComponent<SphereCollider>());
            eye.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? s) { color = Color.cyan };

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 1.2f, 0);
            col.radius = 0.6f;
        }

        protected override void Update()
        {
            base.Update();
            float bob = Mathf.Sin(Time.time * 4f) * 0.12f;
            transform.position = new Vector3(transform.position.x, 0.1f + bob, transform.position.z);
        }
    }

    // ==========================================
    // FAE WILDS ENEMIES
    // ==========================================
    public class FaeTrickster : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            enemyName = "Fae Trickster";
            rank = EnemyRank.Minion;
            expReward = 65;
            fluxReward = 22;
            moveSpeed = 3.6f;
            aggroRadius = 17f;
            attackRadius = 11f;
            attackInterval = 1.1f;
            bulletDamage = 24;
            bulletColor = new Color(0.8f, 0.2f, 1f);
            bulletSpeed = 14f;
            inflictedStatus = StatusEffectType.Dazed;
            statusDuration = 2.5f;
            stats.baseStats.maxHP = 200;
            stats.baseStats.defense = 3;
            stats.currentHP = 200;

            BuildFaeModel();
        }

        private void BuildFaeModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0, 0.7f, 0);
            body.transform.localScale = new Vector3(0.4f, 0.8f, 0.3f);
            Destroy(body.GetComponent<BoxCollider>());
            body.GetComponent<MeshRenderer>().material = new Material(s) { color = new Color(0.5f, 0.2f, 0.75f) };

            // Glowing wings
            GameObject wings = GameObject.CreatePrimitive(PrimitiveType.Quad);
            wings.transform.SetParent(body.transform, false);
            wings.transform.localPosition = new Vector3(0, 0.1f, -0.2f);
            wings.transform.localScale = new Vector3(1.2f, 0.8f, 1f);
            Destroy(wings.GetComponent<Collider>());
            wings.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? s) { color = new Color(0.3f, 0.9f, 1f, 0.8f) };

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 0.7f, 0);
            col.radius = 0.5f;
        }
    }

    // ==========================================
    // PERMAFROST ENEMIES
    // ==========================================
    public class FrostIceGolem : EnemyBase
    {
        protected override void Awake()
        {
            base.Awake();
            enemyName = "Frost Golem";
            rank = EnemyRank.Minion;
            expReward = 75;
            fluxReward = 28;
            moveSpeed = 2.8f;
            aggroRadius = 15f;
            attackRadius = 9f;
            attackInterval = 1.3f;
            bulletDamage = 30;
            bulletColor = new Color(0.5f, 0.85f, 1f);
            bulletSpeed = 14f;
            inflictedStatus = StatusEffectType.Paralyzed;
            statusDuration = 2.0f;
            stats.baseStats.maxHP = 350;
            stats.baseStats.defense = 12;
            stats.currentHP = 350;

            BuildGolemModel();
        }

        private void BuildGolemModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0, 1.0f, 0);
            body.transform.localScale = new Vector3(1.2f, 1.5f, 0.8f);
            Destroy(body.GetComponent<BoxCollider>());
            body.GetComponent<MeshRenderer>().material = new Material(s) { color = new Color(0.6f, 0.85f, 0.95f) };

            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 1.0f, 0);
            col.size = new Vector3(1.4f, 2.0f, 1.0f);
        }
    }
}
