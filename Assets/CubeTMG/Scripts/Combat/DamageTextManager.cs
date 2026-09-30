using System.Collections.Generic;
using UnityEngine;

namespace CubeTMG.Combat
{
    public class DamageTextManager : MonoBehaviour
    {
        public static DamageTextManager Instance { get; private set; }

        private class ActiveText
        {
            public GameObject gameObject;
            public TextMesh textMesh;
            public Vector3 position;
            public Vector3 velocity;
            public float life;
            public float maxLife;
            public Color color;
        }

        private readonly List<ActiveText> activeTexts = new List<ActiveText>();
        private readonly Queue<GameObject> pool = new Queue<GameObject>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            Camera cam = Camera.main;
            Quaternion camRot = cam != null ? cam.transform.rotation : Quaternion.identity;

            for (int i = activeTexts.Count - 1; i >= 0; i--)
            {
                var at = activeTexts[i];
                at.life -= Time.deltaTime;
                if (at.life <= 0f)
                {
                    at.gameObject.SetActive(false);
                    pool.Enqueue(at.gameObject);
                    activeTexts.RemoveAt(i);
                    continue;
                }

                at.position += at.velocity * Time.deltaTime;
                at.velocity.y -= 2.0f * Time.deltaTime; // slight gravity
                at.gameObject.transform.position = at.position;
                at.gameObject.transform.rotation = camRot; // billboard

                float alpha = Mathf.Clamp01(at.life / at.maxLife);
                Color c = at.color;
                c.a = alpha;
                at.textMesh.color = c;
            }
        }

        public void SpawnDamage(Vector3 pos, int damage, bool isPlayer = false, bool isCrit = false)
        {
            Color color = isPlayer ? new Color(1f, 0.2f, 0.2f) : (isCrit ? new Color(1f, 0.6f, 0f) : new Color(0.95f, 0.95f, 0.95f));
            SpawnText(pos, damage.ToString(), color, isCrit ? 1.4f : 1.0f);
        }

        public void SpawnHeal(Vector3 pos, int amount)
        {
            SpawnText(pos, $"+{amount}", new Color(0.2f, 1f, 0.3f), 1.2f);
        }

        public void SpawnMana(Vector3 pos, int amount)
        {
            SpawnText(pos, $"+{amount}", new Color(0.3f, 0.6f, 1f), 1.1f);
        }

        public void SpawnStatus(Vector3 pos, string statusName, Color color)
        {
            SpawnText(pos, statusName, color, 1.3f, 1.2f);
        }

        public void SpawnText(Vector3 pos, string text, Color color, float scale = 1f, float duration = 0.8f)
        {
            GameObject obj;
            TextMesh tm;
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
                obj.SetActive(true);
                tm = obj.GetComponent<TextMesh>();
            }
            else
            {
                obj = new GameObject("FloatingText");
                obj.transform.SetParent(transform);
                tm = obj.AddComponent<TextMesh>();
                tm.alignment = TextAlignment.Center;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.fontSize = 28;
                tm.characterSize = 0.1f;
            }

            tm.text = text;
            tm.color = color;
            obj.transform.localScale = Vector3.one * scale;

            Vector3 startPos = pos + new Vector3(Random.Range(-0.3f, 0.3f), 1.0f, Random.Range(-0.3f, 0.3f));
            Vector3 vel = new Vector3(Random.Range(-0.8f, 0.8f), 3.2f, Random.Range(-0.4f, 0.4f));

            activeTexts.Add(new ActiveText
            {
                gameObject = obj,
                textMesh = tm,
                position = startPos,
                velocity = vel,
                life = duration,
                maxLife = duration,
                color = color
            });
        }
    }
}
