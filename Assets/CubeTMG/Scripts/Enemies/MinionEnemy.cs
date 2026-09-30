using UnityEngine;
using CubeTMG.Stats;

namespace CubeTMG.Enemies
{
    public enum MinionType
    {
        Goblin,
        Skeleton,
        Scorpion
    }

    public class MinionEnemy : EnemyBase
    {
        public MinionType minionType = MinionType.Goblin;

        protected override void Awake()
        {
            base.Awake();
            rank = EnemyRank.Minion;

            switch (minionType)
            {
                case MinionType.Goblin:
                    enemyName = "Forest Goblin";
                    expReward = 30;
                    moveSpeed = 4.2f;
                    aggroRadius = 14f;
                    attackRadius = 7f;
                    attackInterval = 1.0f;
                    bulletDamage = 16;
                    bulletColor = new Color(0.9f, 0.6f, 0.1f);
                    bulletSpeed = 14f;
                    stats.baseStats.maxHP = 120;
                    stats.baseStats.defense = 2;
                    stats.currentHP = 120;
                    BuildGoblinModel();
                    break;

                case MinionType.Skeleton:
                    enemyName = "Skeleton Archer";
                    expReward = 50;
                    moveSpeed = 3.2f;
                    aggroRadius = 18f;
                    attackRadius = 12f;
                    attackInterval = 1.4f;
                    bulletDamage = 24;
                    bulletColor = new Color(0.9f, 0.9f, 0.9f);
                    bulletSpeed = 16f;
                    bulletsPerBurst = 2;
                    bulletSpreadAngle = 8f;
                    stats.baseStats.maxHP = 180;
                    stats.baseStats.defense = 4;
                    stats.currentHP = 180;
                    BuildSkeletonModel();
                    break;

                case MinionType.Scorpion:
                    enemyName = "Sand Scorpion";
                    expReward = 40;
                    moveSpeed = 5.0f;
                    aggroRadius = 15f;
                    attackRadius = 6f;
                    attackInterval = 0.8f;
                    bulletDamage = 18;
                    bulletColor = new Color(0.8f, 0.1f, 0.1f);
                    bulletSpeed = 15f;
                    inflictedStatus = StatusEffectType.Bleeding;
                    statusDuration = 2.5f;
                    stats.baseStats.maxHP = 150;
                    stats.baseStats.defense = 3;
                    stats.currentHP = 150;
                    BuildScorpionModel();
                    break;
            }
        }

        private void BuildGoblinModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            // Green body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0, 0.5f, 0);
            body.transform.localScale = new Vector3(0.55f, 0.7f, 0.4f);
            Destroy(body.GetComponent<BoxCollider>());
            Material mat = new Material(s);
            mat.color = new Color(0.2f, 0.65f, 0.2f);
            body.GetComponent<MeshRenderer>().material = mat;

            // Head with pointed ears
            GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
            head.transform.SetParent(body.transform, false);
            head.transform.localPosition = new Vector3(0, 0.65f, 0);
            head.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            Destroy(head.GetComponent<BoxCollider>());
            head.GetComponent<MeshRenderer>().material = mat;

            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.6f, 0);
            col.size = new Vector3(0.8f, 1.2f, 0.8f);
        }

        private void BuildSkeletonModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            GameObject rib = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rib.name = "Ribs";
            rib.transform.SetParent(transform, false);
            rib.transform.localPosition = new Vector3(0, 0.7f, 0);
            rib.transform.localScale = new Vector3(0.45f, 0.65f, 0.3f);
            Destroy(rib.GetComponent<BoxCollider>());
            Material boneMat = new Material(s);
            boneMat.color = new Color(0.88f, 0.88f, 0.82f);
            rib.GetComponent<MeshRenderer>().material = boneMat;

            // Skull
            GameObject skull = GameObject.CreatePrimitive(PrimitiveType.Cube);
            skull.transform.SetParent(rib.transform, false);
            skull.transform.localPosition = new Vector3(0, 0.6f, 0);
            skull.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            Destroy(skull.GetComponent<BoxCollider>());
            skull.GetComponent<MeshRenderer>().material = boneMat;

            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.75f, 0);
            col.size = new Vector3(0.7f, 1.5f, 0.7f);
        }

        private void BuildScorpionModel()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            GameObject carapace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            carapace.name = "Carapace";
            carapace.transform.SetParent(transform, false);
            carapace.transform.localPosition = new Vector3(0, 0.3f, 0);
            carapace.transform.localScale = new Vector3(0.7f, 0.35f, 0.9f);
            Destroy(carapace.GetComponent<BoxCollider>());
            Material mat = new Material(s);
            mat.color = new Color(0.6f, 0.35f, 0.15f);
            carapace.GetComponent<MeshRenderer>().material = mat;

            // Tail stinger
            GameObject stinger = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stinger.transform.SetParent(carapace.transform, false);
            stinger.transform.localPosition = new Vector3(0, 0.6f, -0.4f);
            stinger.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
            Destroy(stinger.GetComponent<BoxCollider>());
            Material stingerMat = new Material(s);
            stingerMat.color = Color.red;
            stinger.GetComponent<MeshRenderer>().material = stingerMat;

            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 0.4f, 0);
            col.size = new Vector3(1.0f, 0.8f, 1.2f);
        }
    }
}
