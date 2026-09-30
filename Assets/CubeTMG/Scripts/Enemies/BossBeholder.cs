using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;

namespace CubeTMG.Enemies
{
    public class BossBeholder : EnemyBase
    {
        private Transform eyeIris;

        protected override void Awake()
        {
            base.Awake();
            enemyName = "Beholder";
            rank = EnemyRank.God;
            expReward = 400;
            moveSpeed = 1.9f;
            aggroRadius = 22f;
            attackRadius = 15f;
            attackInterval = 1.3f;
            bulletDamage = 28;
            bulletColor = new Color(0.95f, 0.4f, 0.95f);
            isRingNova = true;
            ringCount = 14;
            inflictedStatus = StatusEffectType.Blind;
            statusDuration = 3.5f;
            statPotionChance = 0.55f; // Drops Defense potion

            stats.baseStats.maxHP = 850;
            stats.baseStats.defense = 6;
            stats.currentHP = 850;

            BuildBeholderModel();
        }

        private void BuildBeholderModel()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            // Main floating sphere-ish eye
            GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "BeholderEye";
            eye.transform.SetParent(transform, false);
            eye.transform.localPosition = new Vector3(0, 2.0f, 0);
            eye.transform.localScale = Vector3.one * 1.8f;
            Destroy(eye.GetComponent<SphereCollider>());

            Material eyeMat = new Material(shader);
            eyeMat.color = new Color(0.85f, 0.25f, 0.8f); // Purple skin
            eye.GetComponent<MeshRenderer>().material = eyeMat;

            // Big Central Sclera & Iris
            GameObject sclera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sclera.transform.SetParent(eye.transform, false);
            sclera.transform.localPosition = new Vector3(0, 0, 0.35f);
            sclera.transform.localScale = new Vector3(0.7f, 0.7f, 0.5f);
            Destroy(sclera.GetComponent<SphereCollider>());
            var scleraMr = sclera.GetComponent<MeshRenderer>();
            Material scleraMat = new Material(shader);
            scleraMat.color = Color.white;
            scleraMr.material = scleraMat;

            GameObject iris = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            iris.transform.SetParent(sclera.transform, false);
            iris.transform.localPosition = new Vector3(0, 0, 0.4f);
            iris.transform.localScale = new Vector3(0.45f, 0.45f, 0.45f);
            Destroy(iris.GetComponent<SphereCollider>());
            var irisMr = iris.GetComponent<MeshRenderer>();
            Material irisMat = new Material(shader);
            irisMat.color = new Color(1f, 0.1f, 0.1f);
            irisMr.material = irisMat;
            eyeIris = iris.transform;

            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.center = new Vector3(0, 2.0f, 0);
            col.radius = 1.1f;
        }

        protected override void Update()
        {
            base.Update();

            // Floating bobbing motion
            float bob = Mathf.Sin(Time.time * 2.5f) * 0.15f;
            transform.position = new Vector3(transform.position.x, 0.1f + bob, transform.position.z);
        }
    }
}
