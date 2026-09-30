using UnityEngine;
using CubeTMG.Player;
using CubeTMG.Stats;
using CubeTMG.Core;

namespace CubeTMG.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        public static DeathScreenUI Instance { get; private set; }

        private bool isDead = false;
        private int finalLevel = 1;
        private int finalFame = 0;

        private Texture2D darkOverlay;
        private GUIStyle deathHeaderStyle;
        private GUIStyle deathBodyStyle;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            darkOverlay = new Texture2D(2, 2);
            Color[] pix = { new Color(0, 0, 0, 0.85f), new Color(0, 0, 0, 0.85f), new Color(0, 0, 0, 0.85f), new Color(0, 0, 0, 0.85f) };
            darkOverlay.SetPixels(pix);
            darkOverlay.Apply();
        }

        private void InitStyles()
        {
            if (deathHeaderStyle != null) return;

            deathHeaderStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 32,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(0.95f, 0.15f, 0.15f) }
            };

            deathBodyStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
        }

        public void ShowDeathScreen(int level, int fame)
        {
            isDead = true;
            finalLevel = level;
            finalFame = fame;
        }

        private void OnGUI()
        {
            if (!isDead) return;

            InitStyles();

            // Fullscreen dark overlay
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), darkOverlay);

            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            GUI.Label(new Rect(cx - 250f, cy - 140f, 500f, 50f), "YOU HAVE DIED!", deathHeaderStyle);
            GUI.Label(new Rect(cx - 250f, cy - 80f, 500f, 30f), $"Slain in the Realm of the Cube Gods", deathBodyStyle);
            GUI.Label(new Rect(cx - 250f, cy - 40f, 500f, 30f), $"Reached Level {finalLevel}", deathBodyStyle);
            GUI.Label(new Rect(cx - 250f, cy, 500f, 30f), $"<color=#ffd700>Earned {finalFame} Fame ★</color>", deathBodyStyle);

            // Respawn Button
            if (GUI.Button(new Rect(cx - 150f, cy + 60f, 300f, 40f), "RESPAWN IN NEXUS"))
            {
                Respawn();
            }

            // Quick Class switch buttons
            float bWidth = 80f;
            float bGap = 10f;
            float startX = cx - (5 * bWidth + 4 * bGap) * 0.5f;
            PlayerClassType[] classes = { PlayerClassType.Wizard, PlayerClassType.Warrior, PlayerClassType.Priest, PlayerClassType.Archer, PlayerClassType.Knight };

            GUI.Label(new Rect(cx - 150f, cy + 115f, 300f, 25f), "Or Respawn as New Class:", deathBodyStyle);
            for (int i = 0; i < classes.Length; i++)
            {
                if (GUI.Button(new Rect(startX + i * (bWidth + bGap), cy + 145f, bWidth, 32f), classes[i].ToString()))
                {
                    RespawnAsClass(classes[i]);
                }
            }
        }

        private void Respawn()
        {
            isDead = false;
            var player = PlayerController.Instance;
            if (player != null)
            {
                var stats = player.GetComponent<CharacterStats>();
                player.SwitchClass(stats.characterClass);
                player.TeleportToNexus();
            }
        }

        private void RespawnAsClass(PlayerClassType pClass)
        {
            isDead = false;
            var player = PlayerController.Instance;
            if (player != null)
            {
                player.SwitchClass(pClass);
                player.TeleportToNexus();
            }
        }
    }
}
