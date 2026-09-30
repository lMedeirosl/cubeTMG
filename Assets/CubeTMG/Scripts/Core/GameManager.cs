using UnityEngine;
using CubeTMG.Combat;
using CubeTMG.World;
using CubeTMG.Player;
using CubeTMG.UI;
using CubeTMG.Stats;

namespace CubeTMG.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public PlayerClassType defaultClass = PlayerClassType.Wizard;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBoot()
        {
            if (FindAnyObjectByType<GameManager>() == null)
            {
                GameObject gmGO = new GameObject("CubeTMG_GameManager");
                gmGO.AddComponent<GameManager>();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeGame();
        }

        private void InitializeGame()
        {
            SetupLighting();
            SetupManagers();
            SetupWorld();
            SetupPlayer();
        }

        private void SetupLighting()
        {
            // Setup bright vibrant Cube World sunlight
            Light mainLight = FindAnyObjectByType<Light>();
            if (mainLight == null)
            {
                GameObject lightGO = new GameObject("Directional Light");
                mainLight = lightGO.AddComponent<Light>();
                mainLight.type = LightType.Directional;
            }

            mainLight.color = new Color(1f, 0.98f, 0.92f);
            mainLight.intensity = 1.3f;
            mainLight.transform.rotation = Quaternion.Euler(50f, -30f, 0);

            RenderSettings.ambientLight = new Color(0.45f, 0.48f, 0.55f);
        }

        private void SetupManagers()
        {
            if (FindAnyObjectByType<SoundManager>() == null)
            {
                GameObject sm = new GameObject("SoundManager");
                sm.AddComponent<SoundManager>();
            }

            if (FindAnyObjectByType<ProjectilePool>() == null)
            {
                GameObject pp = new GameObject("ProjectilePool");
                pp.AddComponent<ProjectilePool>();
            }

            if (FindAnyObjectByType<DamageTextManager>() == null)
            {
                GameObject dt = new GameObject("DamageTextManager");
                dt.AddComponent<DamageTextManager>();
            }

            if (FindAnyObjectByType<HUDManager>() == null)
            {
                GameObject hud = new GameObject("HUDManager");
                hud.AddComponent<HUDManager>();
            }

            if (FindAnyObjectByType<DeathScreenUI>() == null)
            {
                GameObject ds = new GameObject("DeathScreenUI");
                ds.AddComponent<DeathScreenUI>();
            }
        }

        private void SetupWorld()
        {
            if (FindAnyObjectByType<VoxelWorldGenerator>() == null)
            {
                GameObject world = new GameObject("VoxelWorldGenerator");
                world.AddComponent<VoxelWorldGenerator>();
            }
        }

        private void SetupPlayer()
        {
            if (FindAnyObjectByType<PlayerController>() == null)
            {
                GameObject playerGO = new GameObject("Player");
                playerGO.transform.position = new Vector3(0, 0.5f, 0);

                var stats = playerGO.AddComponent<CharacterStats>();
                stats.characterClass = defaultClass;

                playerGO.AddComponent<Items.Inventory>();
                playerGO.AddComponent<PlayerController>();
            }
        }
    }
}
