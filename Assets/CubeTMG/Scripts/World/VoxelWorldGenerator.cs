using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Enemies;
using CubeTMG.Stats;

namespace CubeTMG.World
{
    public class VoxelWorldGenerator : MonoBehaviour
    {
        public static VoxelWorldGenerator Instance { get; private set; }

        private Material terrainMaterial;
        private Shader litShader;
        private Shader unlitShader;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            GenerateWorld();
        }

        public void GenerateWorld()
        {
            litShader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            unlitShader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            terrainMaterial = new Material(litShader);

            BuildNexusPlaza();
            BuildRotMGRealm();
            BuildCandoriaBiome();
            BuildNeonCityBiome();
            BuildFaeWildsBiome();
            BuildPermafrostBiome();
        }

        // ==========================================
        // NEXUS PLAZA & MULTIVERSE PORTALS
        // ==========================================
        private void BuildNexusPlaza()
        {
            GameObject nexus = new GameObject("NexusZone");
            nexus.transform.SetParent(transform);

            // Marble floor: 50w x 45d
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "NexusFloor";
            floor.transform.SetParent(nexus.transform, false);
            floor.transform.position = new Vector3(0, -0.5f, 0f);
            floor.transform.localScale = new Vector3(50f, 1.0f, 45f);

            Material floorMat = new Material(terrainMaterial);
            floorMat.color = new Color(0.92f, 0.90f, 0.85f);
            floor.GetComponent<MeshRenderer>().material = floorMat;

            // Healing fountain at center (0, 0, 0)
            GameObject fountain = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            fountain.name = "HealingFountain";
            fountain.transform.SetParent(nexus.transform, false);
            fountain.transform.position = new Vector3(0, 0.4f, 0f);
            fountain.transform.localScale = new Vector3(6f, 0.6f, 6f);
            var fMat = new Material(terrainMaterial) { color = new Color(0.2f, 0.6f, 1f) };
            fountain.GetComponent<MeshRenderer>().material = fMat;

            // Target Dummies
            SpawnTargetDummy(new Vector3(-8f, 0f, 0f));
            SpawnTargetDummy(new Vector3(8f, 0f, 0f));

            // Class Selection Altars (at South side Z = -12)
            SpawnClassAltar(new Vector3(-12f, 0f, -12f), PlayerClassType.Wizard, new Color(0.2f, 0.4f, 1f));
            SpawnClassAltar(new Vector3(-6f, 0f, -12f), PlayerClassType.Warrior, new Color(0.8f, 0.2f, 0.2f));
            SpawnClassAltar(new Vector3(0f, 0f, -12f), PlayerClassType.Priest, new Color(0.95f, 0.95f, 0.4f));
            SpawnClassAltar(new Vector3(6f, 0f, -12f), PlayerClassType.Archer, new Color(0.2f, 0.8f, 0.3f));
            SpawnClassAltar(new Vector3(12f, 0f, -12f), PlayerClassType.Knight, new Color(0.4f, 0.5f, 0.7f));

            // Nexus Multiverse Portals (at North side Z = +14 in an arc!)
            SpawnNexusPortal(new Vector3(0f, 0f, 16f), "RotMG Realm", new Vector3(0, 0.5f, 40f), new Color(0.1f, 0.8f, 1f));
            SpawnNexusPortal(new Vector3(-12f, 0f, 14f), "Candoria (Candyland)", new Vector3(-140f, 0.5f, 40f), new Color(1f, 0.35f, 0.75f));
            SpawnNexusPortal(new Vector3(-20f, 0f, 10f), "Fae Wilds", new Vector3(-140f, 0.5f, 120f), new Color(0.75f, 0.2f, 0.95f));
            SpawnNexusPortal(new Vector3(12f, 0f, 14f), "Neon City", new Vector3(140f, 0.5f, 40f), new Color(0.1f, 0.95f, 0.95f));
            SpawnNexusPortal(new Vector3(20f, 0f, 10f), "Permafrost", new Vector3(140f, 0.5f, 120f), new Color(0.6f, 0.85f, 1f));
        }

        private void SpawnNexusPortal(Vector3 pos, string label, Vector3 targetPos, Color portalColor)
        {
            GameObject portalGO = new GameObject($"Portal_{label}");
            portalGO.transform.position = pos;
            var p = portalGO.AddComponent<Portal>();
            p.destinationName = label;
            p.targetPosition = targetPos;
            p.portalColor = portalColor;

            // 3D Text Label over portal
            GameObject textGO = new GameObject("PortalLabel");
            textGO.transform.SetParent(portalGO.transform, false);
            textGO.transform.localPosition = new Vector3(0, 3.8f, 0);
            var tm = textGO.AddComponent<TextMesh>();
            tm.text = label;
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.fontSize = 24;
            tm.characterSize = 0.12f;
            tm.color = portalColor;
        }

        private void SpawnReturnPortal(Vector3 pos, string fromBiome)
        {
            GameObject portalGO = new GameObject($"ReturnPortal_{fromBiome}");
            portalGO.transform.position = pos;
            var p = portalGO.AddComponent<Portal>();
            p.destinationName = "Nexus";
            p.targetPosition = new Vector3(0, 0.5f, 0);
            p.portalColor = new Color(0.95f, 0.85f, 0.2f);

            GameObject textGO = new GameObject("PortalLabel");
            textGO.transform.SetParent(portalGO.transform, false);
            textGO.transform.localPosition = new Vector3(0, 3.8f, 0);
            var tm = textGO.AddComponent<TextMesh>();
            tm.text = "Nexus Portal";
            tm.alignment = TextAlignment.Center;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.fontSize = 22;
            tm.characterSize = 0.12f;
            tm.color = new Color(0.95f, 0.85f, 0.2f);
        }

        private void SpawnTargetDummy(Vector3 pos)
        {
            GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummy.name = "TargetDummy";
            dummy.transform.position = pos + Vector3.up * 0.9f;
            dummy.transform.localScale = new Vector3(0.8f, 1.8f, 0.8f);

            Material dMat = new Material(terrainMaterial) { color = new Color(0.8f, 0.55f, 0.3f) };
            dummy.GetComponent<MeshRenderer>().material = dMat;

            var stats = dummy.AddComponent<CharacterStats>();
            stats.baseStats.maxHP = 999999;
            stats.baseStats.defense = 0;
            stats.currentHP = 999999;
        }

        private void SpawnClassAltar(Vector3 pos, PlayerClassType pClass, Color c)
        {
            GameObject altar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            altar.name = $"Altar_{pClass}";
            altar.transform.position = pos + Vector3.up * 0.2f;
            altar.transform.localScale = new Vector3(2.4f, 0.4f, 2.4f);

            Material mat = new Material(terrainMaterial) { color = c };
            altar.GetComponent<MeshRenderer>().material = mat;

            BoxCollider col = altar.GetComponent<BoxCollider>();
            col.isTrigger = true;

            var trigger = altar.AddComponent<ClassAltarTrigger>();
            trigger.targetClass = pClass;
        }

        // ==========================================
        // 1. ROTMG REALM (Lowlands, Midlands, Godlands)
        // ==========================================
        private void BuildRotMGRealm()
        {
            CreateBiomeChunk("Lowlands", new Vector3(0, -0.5f, 55f), new Vector3(90f, 1.0f, 45f), new Color(0.28f, 0.72f, 0.22f));
            CreateBiomeChunk("Midlands", new Vector3(0, -0.5f, 100f), new Vector3(90f, 1.0f, 45f), new Color(0.45f, 0.55f, 0.35f));
            CreateBiomeChunk("Godlands", new Vector3(0, -0.5f, 145f), new Vector3(90f, 1.0f, 45f), new Color(0.18f, 0.16f, 0.22f));

            SpawnReturnPortal(new Vector3(0, 0f, 35f), "Realm");

            // Trees, Ruins, and Spires
            for (int i = 0; i < 18; i++) SpawnVoxelTree(new Vector3(Random.Range(-38f, 38f), 0, Random.Range(42f, 75f)));
            for (int i = 0; i < 14; i++) SpawnVoxelRuin(new Vector3(Random.Range(-38f, 38f), 0, Random.Range(82f, 118f)));
            for (int i = 0; i < 16; i++) SpawnObsidianSpire(new Vector3(Random.Range(-38f, 38f), 0, Random.Range(128f, 165f)));

            // Spawn RotMG Minions & Gods
            for (int i = 0; i < 8; i++) SpawnEnemy<MinionEnemy>(new Vector3(Random.Range(-35f, 35f), 0f, Random.Range(45f, 72f)), e => e.minionType = MinionType.Goblin);
            for (int i = 0; i < 6; i++) SpawnEnemy<MinionEnemy>(new Vector3(Random.Range(-35f, 35f), 0f, Random.Range(50f, 74f)), e => e.minionType = MinionType.Scorpion);
            for (int i = 0; i < 8; i++) SpawnEnemy<MinionEnemy>(new Vector3(Random.Range(-35f, 35f), 0f, Random.Range(85f, 115f)), e => e.minionType = MinionType.Skeleton);
            for (int i = 0; i < 4; i++) SpawnEnemy<BossMedusa>(new Vector3(Random.Range(-30f, 30f), 0f, Random.Range(130f, 155f)));
            for (int i = 0; i < 4; i++) SpawnEnemy<BossBeholder>(new Vector3(Random.Range(-30f, 30f), 0f, Random.Range(135f, 160f)));
            SpawnEnemy<BossCubeGod>(new Vector3(0f, 0f, 150f));
        }

        // ==========================================
        // 2. CANDORIA (TROVE CANDYLAND)
        // ==========================================
        private void BuildCandoriaBiome()
        {
            Vector3 center = new Vector3(-140f, 0, 55f);

            // Pink marshmallow & chocolate frosting ground
            CreateBiomeChunk("CandoriaGround", center - new Vector3(0, 0.5f, 0), new Vector3(80f, 1.0f, 70f), new Color(0.96f, 0.65f, 0.82f));
            CreateBiomeChunk("ChocolateRiver", center + new Vector3(0, -0.45f, 0), new Vector3(80f, 0.95f, 12f), new Color(0.38f, 0.22f, 0.12f));

            SpawnReturnPortal(center - new Vector3(0, 0, 25f), "Candoria");

            // Lollipop Trees & Gumdrop Boulders
            for (int i = 0; i < 16; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-32f, 32f), 0, Random.Range(-25f, 25f));
                SpawnLollipopTree(pos);
            }

            for (int i = 0; i < 14; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-32f, 32f), 0, Random.Range(-25f, 25f));
                SpawnGumdropBoulder(pos);
            }

            // Candoria Monsters & Boss
            for (int i = 0; i < 10; i++)
            {
                Vector3 p = center + new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-20f, 25f));
                SpawnEnemy<CandyGummyBear>(p);
            }
            SpawnEnemy<BossCupcakeKing>(center + new Vector3(0, 0, 20f));
        }

        private void SpawnLollipopTree(Vector3 pos)
        {
            GameObject tree = new GameObject("LollipopTree");
            tree.transform.position = pos;
            tree.transform.SetParent(transform);

            // Candy cane stick
            GameObject stick = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stick.transform.SetParent(tree.transform, false);
            stick.transform.localPosition = new Vector3(0, 1.8f, 0);
            stick.transform.localScale = new Vector3(0.35f, 1.8f, 0.35f);
            stick.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = Color.white };

            // Swirl candy swirl disk
            GameObject candy = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            candy.transform.SetParent(tree.transform, false);
            candy.transform.localPosition = new Vector3(0, 3.8f, 0);
            candy.transform.localScale = new Vector3(2.4f, 0.3f, 2.4f);
            candy.transform.rotation = Quaternion.Euler(90f, 0, 0);
            Color candyColor = Random.value > 0.5f ? new Color(1f, 0.2f, 0.5f) : new Color(0.2f, 0.9f, 0.95f);
            candy.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = candyColor };
        }

        private void SpawnGumdropBoulder(Vector3 pos)
        {
            GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            drop.name = "Gumdrop";
            drop.transform.position = pos + Vector3.up * 0.7f;
            drop.transform.localScale = new Vector3(1.4f, 1.6f, 1.4f);
            drop.transform.SetParent(transform);

            Color c = Random.value > 0.5f ? new Color(0.95f, 0.3f, 0.1f) : new Color(0.4f, 0.9f, 0.2f);
            drop.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = c };
        }

        // ==========================================
        // 3. NEON CITY (TROVE CYBER GRID)
        // ==========================================
        private void BuildNeonCityBiome()
        {
            Vector3 center = new Vector3(140f, 0, 55f);

            // Dark tech alloy ground
            CreateBiomeChunk("NeonCityGround", center - new Vector3(0, 0.5f, 0), new Vector3(80f, 1.0f, 70f), new Color(0.1f, 0.11f, 0.14f));

            // Glowing neon circuit strips
            CreateBiomeChunk("NeonStrip1", center + new Vector3(-15f, -0.45f, 0), new Vector3(2f, 0.95f, 65f), Color.cyan);
            CreateBiomeChunk("NeonStrip2", center + new Vector3(15f, -0.45f, 0), new Vector3(2f, 0.95f, 65f), new Color(1f, 0.1f, 0.8f));

            SpawnReturnPortal(center - new Vector3(0, 0, 25f), "NeonCity");

            // Cyber Data Pillars
            for (int i = 0; i < 14; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-32f, 32f), 0, Random.Range(-25f, 25f));
                SpawnCyberPillar(pos);
            }

            // Neon Drones
            for (int i = 0; i < 10; i++)
            {
                Vector3 p = center + new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-20f, 25f));
                SpawnEnemy<NeonDrone>(p);
            }
        }

        private void SpawnCyberPillar(Vector3 pos)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "CyberPillar";
            pillar.transform.position = pos + Vector3.up * 2.5f;
            pillar.transform.localScale = new Vector3(1.2f, 5.0f, 1.2f);
            pillar.transform.SetParent(transform);
            pillar.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.2f, 0.22f, 0.26f) };

            // Glowing node
            GameObject node = GameObject.CreatePrimitive(PrimitiveType.Cube);
            node.transform.SetParent(pillar.transform, false);
            node.transform.localPosition = new Vector3(0, 0.3f, 0);
            node.transform.localScale = new Vector3(1.1f, 0.2f, 1.1f);
            node.GetComponent<MeshRenderer>().material = new Material(unlitShader) { color = Color.cyan };
        }

        // ==========================================
        // 4. FAE WILDS (TROVE ENCHANTED FOREST)
        // ==========================================
        private void BuildFaeWildsBiome()
        {
            Vector3 center = new Vector3(-140f, 0, 135f);

            // Indigo & teal enchanted soil
            CreateBiomeChunk("FaeGround", center - new Vector3(0, 0.5f, 0), new Vector3(80f, 1.0f, 70f), new Color(0.15f, 0.18f, 0.28f));

            SpawnReturnPortal(center - new Vector3(0, 0, 25f), "FaeWilds");

            // Giant Glowing Fae Mushrooms & Weeping Trees
            for (int i = 0; i < 14; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-32f, 32f), 0, Random.Range(-25f, 25f));
                SpawnFaeMushroom(pos);
            }

            // Fae Tricksters
            for (int i = 0; i < 8; i++)
            {
                Vector3 p = center + new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-20f, 25f));
                SpawnEnemy<FaeTrickster>(p);
            }
        }

        private void SpawnFaeMushroom(Vector3 pos)
        {
            GameObject mush = new GameObject("FaeMushroom");
            mush.transform.position = pos;
            mush.transform.SetParent(transform);

            // White stem
            GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stem.transform.SetParent(mush.transform, false);
            stem.transform.localPosition = new Vector3(0, 1.2f, 0);
            stem.transform.localScale = new Vector3(0.6f, 1.2f, 0.6f);
            stem.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.85f, 0.85f, 0.9f) };

            // Glowing purple cap
            GameObject cap = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            cap.transform.SetParent(mush.transform, false);
            cap.transform.localPosition = new Vector3(0, 2.5f, 0);
            cap.transform.localScale = new Vector3(2.5f, 1.0f, 2.5f);
            cap.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.7f, 0.2f, 0.95f) };
        }

        // ==========================================
        // 5. PERMAFROST (TROVE ICE BIOME)
        // ==========================================
        private void BuildPermafrostBiome()
        {
            Vector3 center = new Vector3(140f, 0, 135f);

            // Glacial packed ice & snow
            CreateBiomeChunk("PermafrostGround", center - new Vector3(0, 0.5f, 0), new Vector3(80f, 1.0f, 70f), new Color(0.78f, 0.92f, 0.98f));

            SpawnReturnPortal(center - new Vector3(0, 0, 25f), "Permafrost");

            // Frost Ice Spires
            for (int i = 0; i < 14; i++)
            {
                Vector3 pos = center + new Vector3(Random.Range(-32f, 32f), 0, Random.Range(-25f, 25f));
                SpawnFrostSpire(pos);
            }

            // Frost Golems
            for (int i = 0; i < 8; i++)
            {
                Vector3 p = center + new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-20f, 25f));
                SpawnEnemy<FrostIceGolem>(p);
            }
        }

        private void SpawnFrostSpire(Vector3 pos)
        {
            GameObject spire = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spire.name = "FrostSpire";
            spire.transform.position = pos + Vector3.up * 2.5f;
            spire.transform.localScale = new Vector3(1.2f, 5.0f, 1.2f);
            spire.transform.rotation = Quaternion.Euler(Random.Range(-8f, 8f), Random.Range(0, 360f), Random.Range(-8f, 8f));
            spire.transform.SetParent(transform);
            spire.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.55f, 0.85f, 1f) };
        }

        private void CreateBiomeChunk(string name, Vector3 pos, Vector3 size, Color color)
        {
            GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chunk.name = name;
            chunk.transform.SetParent(transform, false);
            chunk.transform.position = pos;
            chunk.transform.localScale = size;
            chunk.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = color };
        }

        private void SpawnVoxelTree(Vector3 pos)
        {
            GameObject tree = new GameObject("VoxelTree");
            tree.transform.position = pos;
            tree.transform.SetParent(transform);

            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trunk.name = "Trunk";
            trunk.transform.SetParent(tree.transform, false);
            trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
            trunk.transform.localScale = new Vector3(0.7f, 3.0f, 0.7f);
            trunk.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.42f, 0.26f, 0.12f) };

            Color leafColor = new Color(0.2f, 0.75f, 0.25f);
            CreateLeafLayer(tree.transform, new Vector3(0, 3.2f, 0), new Vector3(3.2f, 1.2f, 3.2f), leafColor);
            CreateLeafLayer(tree.transform, new Vector3(0, 4.2f, 0), new Vector3(2.4f, 1.0f, 2.4f), leafColor * 1.08f);
            CreateLeafLayer(tree.transform, new Vector3(0, 5.0f, 0), new Vector3(1.4f, 0.8f, 1.4f), leafColor * 1.15f);
        }

        private void CreateLeafLayer(Transform parent, Vector3 localPos, Vector3 size, Color c)
        {
            GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaf.name = "Leaves";
            leaf.transform.SetParent(parent, false);
            leaf.transform.localPosition = localPos;
            leaf.transform.localScale = size;
            Destroy(leaf.GetComponent<BoxCollider>());
            leaf.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = c };
        }

        private void SpawnVoxelRuin(Vector3 pos)
        {
            GameObject ruin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ruin.name = "VoxelRuin";
            ruin.transform.position = pos + Vector3.up * 2f;
            ruin.transform.localScale = new Vector3(1.2f, 4f, 1.2f);
            ruin.transform.SetParent(transform);
            ruin.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.6f, 0.62f, 0.65f) };
        }

        private void SpawnObsidianSpire(Vector3 pos)
        {
            GameObject spire = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spire.name = "ObsidianSpire";
            spire.transform.position = pos + Vector3.up * 2.5f;
            spire.transform.localScale = new Vector3(1.4f, 5f, 1.4f);
            spire.transform.rotation = Quaternion.Euler(Random.Range(-5f, 5f), Random.Range(0, 360f), Random.Range(-5f, 5f));
            spire.transform.SetParent(transform);
            spire.GetComponent<MeshRenderer>().material = new Material(terrainMaterial) { color = new Color(0.12f, 0.1f, 0.15f) };
        }

        private T SpawnEnemy<T>(Vector3 pos, System.Action<T> onConfigure = null) where T : Component
        {
            GameObject go = new GameObject(typeof(T).Name);
            go.transform.position = pos;
            T comp = go.AddComponent<T>();
            onConfigure?.Invoke(comp);
            return comp;
        }
    }

    public class ClassAltarTrigger : MonoBehaviour
    {
        public PlayerClassType targetClass;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponentInParent<CubeTMG.Player.PlayerController>() != null)
            {
                var player = CubeTMG.Player.PlayerController.Instance;
                if (player != null && player.GetComponent<CharacterStats>().characterClass != targetClass)
                {
                    player.SwitchClass(targetClass);
                    CubeTMG.Core.SoundManager.Instance?.PlayLevelUp();
                    CubeTMG.Combat.DamageTextManager.Instance?.SpawnStatus(player.transform.position, $"SWITCHED TO {targetClass.ToString().ToUpper()}!", Color.green);
                }
            }
        }
    }
}
