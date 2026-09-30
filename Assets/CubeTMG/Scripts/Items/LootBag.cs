using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Core;

namespace CubeTMG.Items
{
    public class LootBag : MonoBehaviour
    {
        public const int BAG_SLOT_COUNT = 8;
        public BagTier bagTier = BagTier.Brown;
        public List<ItemData> items = new List<ItemData>();
        public float lifetime = 60f;

        private float currentLifetime;
        private Transform visualTransform;

        public static List<LootBag> ActiveBags = new List<LootBag>();

        private void OnEnable()
        {
            ActiveBags.Add(this);
        }

        private void OnDisable()
        {
            ActiveBags.Remove(this);
        }

        private void Awake()
        {
            currentLifetime = lifetime;
            visualTransform = transform.Find("Visual");
            if (visualTransform == null)
            {
                visualTransform = transform;
            }
        }

        private void Update()
        {
            currentLifetime -= Time.deltaTime;
            if (currentLifetime <= 0f)
            {
                DestroyBag();
                return;
            }

            // Gentle bobbing and rotating animation like classic Cube World / RotMG
            if (visualTransform != null)
            {
                float bob = Mathf.Sin(Time.time * 3f) * 0.08f;
                visualTransform.localPosition = new Vector3(0, bob + 0.2f, 0);
                visualTransform.Rotate(Vector3.up, 45f * Time.deltaTime, Space.World);
            }
        }

        public void Initialize(BagTier tier, List<ItemData> droppedItems)
        {
            bagTier = tier;
            items = new List<ItemData>(droppedItems);

            // Re-evaluate highest tier in bag
            foreach (var item in items)
            {
                if (item.bagTier > bagTier)
                {
                    bagTier = item.bagTier;
                }
            }

            ApplyBagVisualColor();

            if (bagTier == BagTier.White)
            {
                // Play white bag fanfare!
                SoundManager.Instance?.PlayWhiteBagDrop(transform.position);
            }
        }

        public void ApplyBagVisualColor()
        {
            var renderer = GetComponentInChildren<MeshRenderer>();
            if (renderer == null) return;

            Color bagColor = Color.white;
            switch (bagTier)
            {
                case BagTier.Brown:
                    bagColor = new Color(0.45f, 0.28f, 0.15f);
                    break;
                case BagTier.Purple:
                    bagColor = new Color(0.6f, 0.15f, 0.75f);
                    break;
                case BagTier.Cyan:
                    bagColor = new Color(0.1f, 0.75f, 0.85f);
                    break;
                case BagTier.Blue:
                    bagColor = new Color(0.15f, 0.4f, 0.95f);
                    break;
                case BagTier.White:
                    bagColor = new Color(0.95f, 0.95f, 0.98f);
                    break;
            }

            Material mat = new Material(renderer.sharedMaterial != null ? renderer.sharedMaterial.shader : Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
            mat.color = bagColor;
            renderer.material = mat;
        }

        public ItemData TakeItem(int index)
        {
            if (index < 0 || index >= items.Count) return null;
            ItemData taken = items[index];
            items.RemoveAt(index);

            if (items.Count == 0)
            {
                DestroyBag();
            }
            return taken;
        }

        public void DestroyBag()
        {
            Destroy(gameObject);
        }

        public static LootBag SpawnBag(Vector3 position, List<ItemData> dropItems)
        {
            if (dropItems == null || dropItems.Count == 0) return null;

            BagTier highestTier = BagTier.Brown;
            foreach (var item in dropItems)
            {
                if (item.bagTier > highestTier)
                {
                    highestTier = item.bagTier;
                }
            }

            GameObject bagGO = new GameObject($"LootBag_{highestTier}");
            bagGO.transform.position = position;

            // Generate procedural voxel bag mesh
            GameObject visualGO = new GameObject("Visual");
            visualGO.transform.SetParent(bagGO.transform, false);

            MeshFilter mf = visualGO.AddComponent<MeshFilter>();
            MeshRenderer mr = visualGO.AddComponent<MeshRenderer>();
            mf.sharedMesh = CreateBagMesh(highestTier);

            Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
            mr.material = new Material(shader);

            SphereCollider col = bagGO.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 2.0f;

            LootBag lootBag = bagGO.AddComponent<LootBag>();
            lootBag.Initialize(highestTier, dropItems);

            return lootBag;
        }

        private static Mesh CreateBagMesh(BagTier tier)
        {
            // Simple voxel-styled pouch/chest mesh
            Mesh mesh = new Mesh();
            mesh.name = "VoxelBagMesh";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Color> colors = new List<Color>();

            Color mainColor = Color.white;
            switch (tier)
            {
                case BagTier.Brown: mainColor = new Color(0.5f, 0.32f, 0.18f); break;
                case BagTier.Purple: mainColor = new Color(0.65f, 0.2f, 0.85f); break;
                case BagTier.Cyan: mainColor = new Color(0.15f, 0.85f, 0.95f); break;
                case BagTier.Blue: mainColor = new Color(0.2f, 0.45f, 1f); break;
                case BagTier.White: mainColor = new Color(0.98f, 0.98f, 1f); break;
            }

            Color ribbonColor = tier == BagTier.White ? new Color(0.2f, 0.5f, 1f) : new Color(0.9f, 0.8f, 0.2f);

            // Base pouch box: 0.6w x 0.5h x 0.5d
            AddBox(verts, tris, colors, new Vector3(0, 0.25f, 0), new Vector3(0.6f, 0.5f, 0.5f), mainColor);
            // Top knot/ribbon
            AddBox(verts, tris, colors, new Vector3(0, 0.55f, 0), new Vector3(0.25f, 0.15f, 0.25f), ribbonColor);
            AddBox(verts, tris, colors, new Vector3(0, 0.65f, 0), new Vector3(0.35f, 0.08f, 0.15f), ribbonColor);

            mesh.SetVertices(verts);
            mesh.SetTriangles(tris, 0);
            mesh.SetColors(colors);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void AddBox(List<Vector3> verts, List<int> tris, List<Color> colors, Vector3 center, Vector3 size, Color c)
        {
            Vector3 h = size * 0.5f;
            Vector3[] p = {
                center + new Vector3(-h.x, -h.y, -h.z),
                center + new Vector3( h.x, -h.y, -h.z),
                center + new Vector3( h.x,  h.y, -h.z),
                center + new Vector3(-h.x,  h.y, -h.z),
                center + new Vector3(-h.x, -h.y,  h.z),
                center + new Vector3( h.x, -h.y,  h.z),
                center + new Vector3( h.x,  h.y,  h.z),
                center + new Vector3(-h.x,  h.y,  h.z),
            };

            int[][] faces = {
                new int[] { 0, 2, 1, 0, 3, 2 }, // Front
                new int[] { 5, 6, 4, 4, 6, 7 }, // Back
                new int[] { 3, 7, 2, 2, 7, 6 }, // Top
                new int[] { 0, 1, 4, 1, 5, 4 }, // Bottom
                new int[] { 4, 7, 0, 0, 7, 3 }, // Left
                new int[] { 1, 2, 5, 5, 2, 6 }  // Right
            };

            int start = verts.Count;
            for (int i = 0; i < 8; i++)
            {
                verts.Add(p[i]);
                colors.Add(c);
            }

            foreach (var face in faces)
            {
                foreach (var idx in face)
                {
                    tris.Add(start + idx);
                }
            }
        }
    }
}
