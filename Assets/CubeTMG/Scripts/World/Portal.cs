using UnityEngine;
using CubeTMG.Core;
using CubeTMG.Player;

namespace CubeTMG.World
{
    public class Portal : MonoBehaviour
    {
        public string destinationName = "The Realm";
        public Vector3 targetPosition = new Vector3(0, 0.5f, 35f);
        public Color portalColor = new Color(0.2f, 0.8f, 1f);

        private void Start()
        {
            BuildPortalVisual();
        }

        private void BuildPortalVisual()
        {
            Shader s = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");

            // Portal Frame (Stone pillars and lintel)
            Color stoneColor = new Color(0.45f, 0.45f, 0.5f);
            CreateBox("PillarL", transform, new Vector3(-1.2f, 1.6f, 0), new Vector3(0.5f, 3.2f, 0.5f), stoneColor, s);
            CreateBox("PillarR", transform, new Vector3(1.2f, 1.6f, 0), new Vector3(0.5f, 3.2f, 0.5f), stoneColor, s);
            CreateBox("Lintel", transform, new Vector3(0, 3.2f, 0), new Vector3(2.9f, 0.5f, 0.6f), stoneColor, s);
            CreateBox("Pedestal", transform, new Vector3(0, 0.15f, 0), new Vector3(3.2f, 0.3f, 1.5f), stoneColor, s);

            // Swirling portal vortex center
            GameObject vortex = GameObject.CreatePrimitive(PrimitiveType.Quad);
            vortex.name = "Vortex";
            vortex.transform.SetParent(transform, false);
            vortex.transform.localPosition = new Vector3(0, 1.6f, 0);
            vortex.transform.localScale = new Vector3(1.9f, 2.7f, 1f);
            Destroy(vortex.GetComponent<Collider>());

            Shader unlit = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Sprites/Default");
            Material vortexMat = new Material(unlit);
            vortexMat.color = portalColor;
            vortex.GetComponent<MeshRenderer>().material = vortexMat;

            // Trigger Collider
            BoxCollider col = gameObject.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.center = new Vector3(0, 1.5f, 0);
            col.size = new Vector3(2.0f, 3.0f, 1.5f);
        }

        private static void CreateBox(string name, Transform parent, Vector3 localPos, Vector3 size, Color c, Shader shader)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPos;
            cube.transform.localScale = size;
            Destroy(cube.GetComponent<BoxCollider>());
            Material mat = new Material(shader);
            mat.color = c;
            cube.GetComponent<MeshRenderer>().material = mat;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.GetComponentInParent<PlayerController>() != null)
            {
                var player = PlayerController.Instance;
                if (player != null)
                {
                    player.transform.position = targetPosition;
                    SoundManager.Instance?.PlayNexusRecall();
                }
            }
        }
    }
}
