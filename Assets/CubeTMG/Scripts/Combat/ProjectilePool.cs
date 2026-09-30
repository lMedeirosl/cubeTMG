using System.Collections.Generic;
using UnityEngine;

namespace CubeTMG.Combat
{
    public class ProjectilePool : MonoBehaviour
    {
        public static ProjectilePool Instance { get; private set; }

        private readonly Queue<Projectile> pool = new Queue<Projectile>();
        private GameObject projectileTemplate;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateTemplate();
        }

        private void CreateTemplate()
        {
            projectileTemplate = new GameObject("ProjectileTemplate");
            projectileTemplate.transform.SetParent(transform);

            // 3D Voxel bullet mesh (elongated cube)
            GameObject cubeMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh.name = "BulletMesh";
            cubeMesh.transform.SetParent(projectileTemplate.transform, false);
            cubeMesh.transform.localScale = new Vector3(0.5f, 0.5f, 1.2f);
            Destroy(cubeMesh.GetComponent<BoxCollider>());

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            Material mat = new Material(shader);
            cubeMesh.GetComponent<MeshRenderer>().material = mat;

            SphereCollider col = projectileTemplate.AddComponent<SphereCollider>();
            col.isTrigger = true;
            col.radius = 0.45f;

            Rigidbody rb = projectileTemplate.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            TrailRenderer trail = projectileTemplate.AddComponent<TrailRenderer>();
            trail.time = 0.12f;
            trail.material = new Material(Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default"));

            projectileTemplate.AddComponent<Projectile>();
            projectileTemplate.SetActive(false);
        }

        public Projectile Get()
        {
            while (pool.Count > 0)
            {
                Projectile p = pool.Dequeue();
                if (p != null)
                {
                    return p;
                }
            }

            GameObject obj = Instantiate(projectileTemplate, transform);
            return obj.GetComponent<Projectile>();
        }

        public void Return(Projectile p)
        {
            if (p != null)
            {
                p.gameObject.SetActive(false);
                pool.Enqueue(p);
            }
        }
    }
}
