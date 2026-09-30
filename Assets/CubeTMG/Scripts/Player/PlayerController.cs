using System.Collections.Generic;
using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Combat;
using CubeTMG.Items;
using CubeTMG.Core;
using CubeTMG.UI;

namespace CubeTMG.Player
{
    [RequireComponent(typeof(CharacterStats), typeof(Inventory))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Cube World Free 3D Camera")]
        public Transform cameraPivot;
        public float cameraDistance = 10f;
        public float minCameraDistance = 3f;
        public float maxCameraDistance = 24f;
        public float mouseSensitivityX = 3.0f;
        public float mouseSensitivityY = 2.5f;
        public float minPitch = -25f;
        public float maxPitch = 75f;

        [Header("Aiming & Crosshair")]
        public Vector3 aimWorldPoint;
        public Vector3 aimDirection;
        public bool isCursorMode = false; // Toggled with Tab or Alt for UI interaction

        private float currentYaw = 0f;
        private float currentPitch = 30f;

        private CharacterStats stats;
        private Inventory inventory;
        private VoxelCharacterBuilder voxelVisual;
        private Rigidbody rb;

        private float shootTimer = 0f;
        private float abilityCooldownTimer = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            stats = GetComponent<CharacterStats>();
            inventory = GetComponent<Inventory>();
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.isKinematic = true;
            }

            gameObject.tag = "Player";

            CapsuleCollider col = GetComponent<CapsuleCollider>();
            if (col == null)
            {
                col = gameObject.AddComponent<CapsuleCollider>();
                col.center = new Vector3(0, 0.9f, 0);
                col.radius = 0.5f;
                col.height = 1.8f;
            }

            GameObject visualGO = new GameObject("VoxelVisual");
            visualGO.transform.SetParent(transform, false);
            voxelVisual = visualGO.AddComponent<VoxelCharacterBuilder>();

            stats.OnDeath += HandlePlayerDeath;
        }

        private void Start()
        {
            SetupCamera();
            RebuildVisuals();
            SetCursorLock(false); // Start unlocked so user sees UI, click game to lock!
        }

        private void OnDestroy()
        {
            if (stats != null)
            {
                stats.OnDeath -= HandlePlayerDeath;
            }
        }

        public void SwitchClass(PlayerClassType newClass)
        {
            stats.InitializeForClass(newClass);
            inventory.EquipStarterKit(newClass);
            RebuildVisuals();
        }

        public void RebuildVisuals()
        {
            if (voxelVisual != null)
            {
                voxelVisual.BuildVoxelCharacter(stats.characterClass);
            }
        }

        private void SetupCamera()
        {
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camGO = new GameObject("Main Camera");
                camGO.tag = "MainCamera";
                mainCam = camGO.AddComponent<Camera>();
                camGO.AddComponent<AudioListener>();
            }

            if (cameraPivot == null)
            {
                GameObject pivotGO = new GameObject("CameraPivot");
                cameraPivot = pivotGO.transform;
            }

            mainCam.transform.SetParent(cameraPivot);
            mainCam.transform.localPosition = new Vector3(0, 1.2f, -cameraDistance);
            mainCam.transform.localRotation = Quaternion.identity;

            currentYaw = transform.eulerAngles.y;
            currentPitch = 30f;
            UpdateCameraTransform();
        }

        private void Update()
        {
            if (stats.IsDead) return;

            HandleCursorToggle();
            HandleFreeCameraOrbit();
            HandleAiming();
            HandleCombat();
            HandleInput();
        }

        private void FixedUpdate()
        {
            if (stats.IsDead) return;
            HandleMovement();
        }

        private void HandleCursorToggle()
        {
            // Toggle cursor mode with Tab or Left Alt
            if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isCursorMode = !isCursorMode;
                SetCursorLock(!isCursorMode);
            }

            // Click into game window to engage free camera lock
            if (Input.GetMouseButtonDown(0) && isCursorMode)
            {
                // If clicking outside side panel UI, lock camera
                if (Input.mousePosition.x < Screen.width - 260f)
                {
                    isCursorMode = false;
                    SetCursorLock(true);
                }
            }
        }

        private void SetCursorLock(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        private void HandleFreeCameraOrbit()
        {
            // Zoom with scroll wheel
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.001f)
            {
                cameraDistance = Mathf.Clamp(cameraDistance - scroll * 5.0f, minCameraDistance, maxCameraDistance);
                if (Camera.main != null)
                {
                    Camera.main.transform.localPosition = new Vector3(0, 1.2f, -cameraDistance);
                }
            }

            // Orbit camera when cursor is locked or when Right Mouse Button is held
            bool shouldRotate = !isCursorMode || Input.GetMouseButton(1);
            if (shouldRotate)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

                currentYaw += mouseX;
                currentPitch = Mathf.Clamp(currentPitch - mouseY, minPitch, maxPitch);
            }

            UpdateCameraTransform();
        }

        private void UpdateCameraTransform()
        {
            if (cameraPivot == null) return;
            cameraPivot.position = transform.position + Vector3.up * 1.0f;
            cameraPivot.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }

        private void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector3 inputDir = new Vector3(h, 0, v).normalized;

            if (inputDir.sqrMagnitude > 0.01f)
            {
                // Cube World Movement: Movement is relative to current camera yaw!
                Vector3 camForward = Quaternion.Euler(0, currentYaw, 0) * Vector3.forward;
                Vector3 camRight = Quaternion.Euler(0, currentYaw, 0) * Vector3.right;
                Vector3 moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;

                float speed = stats.CalculateMoveSpeed();
                transform.position += moveDir * speed * Time.fixedDeltaTime;

                // Turn character toward move direction smoothly
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 14f * Time.fixedDeltaTime);

                voxelVisual.AnimateWalk(1.0f);
            }
            else
            {
                voxelVisual.AnimateIdle();
            }
        }

        private void HandleAiming()
        {
            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray;
            if (!isCursorMode)
            {
                // Aim directly through center crosshair!
                ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            }
            else
            {
                // Aim through mouse cursor in 3D
                ray = cam.ScreenPointToRay(Input.mousePosition);
            }

            // Raycast into scene or ground plane
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, ~0, QueryTriggerInteraction.Ignore))
            {
                aimWorldPoint = hit.point;
            }
            else
            {
                Plane ground = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
                if (ground.Raycast(ray, out float enter))
                {
                    aimWorldPoint = ray.GetPoint(enter);
                }
                else
                {
                    aimWorldPoint = transform.position + ray.direction * 30f;
                }
            }

            Vector3 flatAim = new Vector3(aimWorldPoint.x, transform.position.y, aimWorldPoint.z);
            aimDirection = (flatAim - transform.position).normalized;

            // When firing or cursor locked, align hero toward crosshair
            if (!isCursorMode || Input.GetMouseButton(0))
            {
                if (aimDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(aimDirection), 16f * Time.deltaTime);
                }
            }
        }

        private void HandleCombat()
        {
            shootTimer -= Time.deltaTime;
            abilityCooldownTimer -= Time.deltaTime;

            if (stats.HasStatus(StatusEffectType.Stunned)) return;

            // Autofire: Hold Left Click (only when not interacting with UI)
            if (Input.GetMouseButton(0))
            {
                // Check if mouse is hovering over the right UI panel in cursor mode
                if (isCursorMode && Input.mousePosition.x > Screen.width - 260f)
                {
                    return;
                }

                ItemData weapon = inventory.weaponSlot;
                if (weapon != null && shootTimer <= 0f)
                {
                    FireWeapon(weapon);
                }
            }

            // Ability: Spacebar or Right Click (when cursor locked)
            if (Input.GetKeyDown(KeyCode.Space) || (!isCursorMode && Input.GetMouseButtonDown(1)))
            {
                ItemData ability = inventory.abilitySlot;
                if (ability != null && abilityCooldownTimer <= 0f)
                {
                    CastAbility(ability);
                }
            }
        }

        private void FireWeapon(ItemData weapon)
        {
            float aps = stats.CalculateAttacksPerSecond(weapon.rateOfFire);
            shootTimer = 1.0f / aps;

            Vector3 spawnPos = transform.position + Vector3.up * 0.9f + transform.forward * 0.4f;
            int count = weapon.numProjectiles;
            float startAngle = -(count - 1) * weapon.arcGapAngle * 0.5f;

            for (int i = 0; i < count; i++)
            {
                float angle = startAngle + i * weapon.arcGapAngle;
                Vector3 dir = Quaternion.Euler(0, angle, 0) * aimDirection;
                int rawDmg = weapon.RollDamage();
                int finalDmg = Mathf.RoundToInt(stats.CalculateBulletDamage(rawDmg));

                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(spawnPos, dir, finalDmg, weapon.projectileSpeed, weapon.range,
                    true, weapon.bulletColor, weapon.bulletSize, weapon.isPiercing, weapon.isArmorPiercing, weapon.isWavy);
            }

            SoundManager.Instance?.PlayShoot(transform.position);
        }

        private void CastAbility(ItemData ability)
        {
            if (stats.ConsumeMP(ability.manaCost))
            {
                abilityCooldownTimer = ability.cooldown;

                switch (ability.abilityType)
                {
                    case AbilityType.Spellbomb:
                        ExecuteSpellbomb(ability);
                        break;
                    case AbilityType.Helm:
                        ExecuteHelm(ability);
                        break;
                    case AbilityType.Tome:
                        ExecuteTome(ability);
                        break;
                    case AbilityType.Quiver:
                        ExecuteQuiver(ability);
                        break;
                    case AbilityType.Shield:
                        ExecuteShield(ability);
                        break;
                }
            }
        }

        private void ExecuteSpellbomb(ItemData ability)
        {
            Vector3 target = aimWorldPoint;
            target.y = 0.5f;
            SoundManager.Instance?.PlaySpellbomb(target);

            int numBullets = 16;
            float step = 360f / numBullets;
            for (int i = 0; i < numBullets; i++)
            {
                Vector3 dir = Quaternion.Euler(0, i * step, 0) * Vector3.forward;
                int dmg = Mathf.RoundToInt(stats.CalculateBulletDamage(ability.abilityPower));

                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(target, dir, dmg, 12f, ability.abilityRadius, true, new Color(0.9f, 0.2f, 1f), 0.45f, true, true);
            }

            DamageTextManager.Instance?.SpawnStatus(target, "SPELLBOMB!", Color.magenta);
        }

        private void ExecuteHelm(ItemData ability)
        {
            stats.ApplyStatus(StatusEffectType.Berserk, ability.statusDuration);
            stats.ApplyStatus(StatusEffectType.Speedy, ability.statusDuration);
            if (ability.id == "helm_jugg")
            {
                stats.ApplyStatus(StatusEffectType.Armored, ability.statusDuration);
            }

            SoundManager.Instance?.PlayLevelUp();
            DamageTextManager.Instance?.SpawnStatus(transform.position, "BERSERK!", Color.red);
        }

        private void ExecuteTome(ItemData ability)
        {
            stats.Heal(ability.abilityPower);
            stats.ApplyStatus(StatusEffectType.Healing, ability.statusDuration);
            SoundManager.Instance?.PlayPotionDrink();
            DamageTextManager.Instance?.SpawnHeal(transform.position, ability.abilityPower);
        }

        private void ExecuteQuiver(ItemData ability)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.9f;
            int dmg = Mathf.RoundToInt(stats.CalculateBulletDamage(ability.abilityPower));

            Projectile p = ProjectilePool.Instance.Get();
            p.Launch(spawnPos, aimDirection, dmg, 24f, 12f, true, new Color(0.1f, 1f, 0.9f), 0.55f,
                true, false, false, StatusEffectType.Paralyzed, ability.statusDuration);

            SoundManager.Instance?.PlayShoot(transform.position);
            DamageTextManager.Instance?.SpawnStatus(transform.position, "PARALYZE SHOT!", Color.cyan);
        }

        private void ExecuteShield(ItemData ability)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.9f;
            int count = 5;
            float step = 12f;
            float startAngle = -(count - 1) * step * 0.5f;

            for (int i = 0; i < count; i++)
            {
                Vector3 dir = Quaternion.Euler(0, startAngle + i * step, 0) * aimDirection;
                int dmg = Mathf.RoundToInt(stats.CalculateBulletDamage(ability.abilityPower));

                Projectile p = ProjectilePool.Instance.Get();
                p.Launch(spawnPos, dir, dmg, 18f, 4.5f, true, new Color(0.3f, 0.4f, 1f), 0.5f,
                    false, true, false, StatusEffectType.Stunned, ability.statusDuration);
            }

            SoundManager.Instance?.PlaySpellbomb(transform.position);
            DamageTextManager.Instance?.SpawnStatus(transform.position, "SHIELD BASH!", Color.blue);
        }

        private void HandleInput()
        {
            // R: Instant Nexus Recall
            if (Input.GetKeyDown(KeyCode.R))
            {
                TeleportToNexus();
            }

            // Quick Potions: F for Health, V for Magic
            if (Input.GetKeyDown(KeyCode.F))
            {
                inventory.UseHealthPotion();
                SoundManager.Instance?.PlayPotionDrink();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                inventory.UseMagicPotion();
                SoundManager.Instance?.PlayPotionDrink();
            }

            // Number keys 1-8 for Backpack Quick-Use
            for (int i = 0; i < Inventory.INVENTORY_SIZE; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    inventory.EquipFromBackpack(i);
                }
            }

            // E Key: Loot nearest bag or interact
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryLootNearestBag();
            }
        }

        public void TeleportToNexus()
        {
            transform.position = new Vector3(0, 0.5f, 0);
            stats.Heal(stats.TotalMaxHP);
            stats.RestoreMP(stats.TotalMaxMP);
            SoundManager.Instance?.PlayNexusRecall();
            DamageTextManager.Instance?.SpawnStatus(transform.position, "RECALLED TO NEXUS!", Color.cyan);
        }

        public void TryLootNearestBag()
        {
            LootBag nearest = null;
            float minDist = 3.0f;

            foreach (var bag in LootBag.ActiveBags)
            {
                if (bag == null) continue;
                float d = Vector3.Distance(transform.position, bag.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = bag;
                }
            }

            if (nearest != null && nearest.items.Count > 0)
            {
                ItemData item = nearest.TakeItem(0);
                if (item != null)
                {
                    if (inventory.AddItem(item))
                    {
                        DamageTextManager.Instance?.SpawnText(transform.position, $"Looted: {item.name}", Color.green);
                        SoundManager.Instance?.PlayHitEnemy(transform.position);
                    }
                    else
                    {
                        nearest.items.Insert(0, item);
                        DamageTextManager.Instance?.SpawnText(transform.position, "Inventory Full!", Color.red);
                    }
                }
            }
        }

        private void HandlePlayerDeath()
        {
            GameObject grave = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grave.name = "Tombstone";
            grave.transform.position = transform.position + Vector3.up * 0.5f;
            grave.transform.localScale = new Vector3(0.6f, 0.9f, 0.3f);
            var mr = grave.GetComponent<MeshRenderer>();
            mr.material.color = new Color(0.5f, 0.5f, 0.55f);

            SetCursorLock(false);
            DeathScreenUI.Instance?.ShowDeathScreen(stats.level, stats.totalFame);
        }
    }
}
