using UnityEngine;
using CubeTMG.Stats;
using CubeTMG.Items;
using CubeTMG.Player;
using CubeTMG.Core;
using CubeTMG.Combat;

namespace CubeTMG.UI
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        private PlayerController player;
        private CharacterStats stats;
        private Inventory inventory;

        private Texture2D redTexture;
        private Texture2D darkRedTexture;
        private Texture2D blueTexture;
        private Texture2D darkBlueTexture;
        private Texture2D greenTexture;
        private Texture2D panelTexture;
        private Texture2D crosshairTexture;

        private GUIStyle headerStyle;
        private GUIStyle barTextStyle;
        private GUIStyle statTextStyle;
        private GUIStyle slotTextStyle;
        private GUIStyle tooltipStyle;

        private string currentTooltip = "";
        private ItemData selectedItemForForge = null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateTextures();
        }

        private void Start()
        {
            player = PlayerController.Instance;
            if (player != null)
            {
                stats = player.GetComponent<CharacterStats>();
                inventory = player.GetComponent<Inventory>();
            }
        }

        private void CreateTextures()
        {
            redTexture = MakeTex(2, 2, new Color(0.85f, 0.15f, 0.15f));
            darkRedTexture = MakeTex(2, 2, new Color(0.35f, 0.05f, 0.05f));
            blueTexture = MakeTex(2, 2, new Color(0.15f, 0.45f, 0.95f));
            darkBlueTexture = MakeTex(2, 2, new Color(0.05f, 0.15f, 0.35f));
            greenTexture = MakeTex(2, 2, new Color(0.2f, 0.75f, 0.25f));
            panelTexture = MakeTex(2, 2, new Color(0.12f, 0.12f, 0.15f, 0.94f));
            crosshairTexture = MakeTex(2, 2, new Color(1f, 1f, 1f, 0.85f));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void InitStyles()
        {
            if (headerStyle != null) return;

            headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            barTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            statTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.9f, 0.9f, 0.9f) },
                richText = true
            };

            slotTextStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                richText = true
            };

            tooltipStyle = new GUIStyle(GUI.skin.box)
            {
                fontSize = 11,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                richText = true,
                normal = { background = panelTexture, textColor = Color.white }
            };
        }

        private void OnGUI()
        {
            InitStyles();

            if (player == null)
            {
                player = PlayerController.Instance;
                if (player != null)
                {
                    stats = player.GetComponent<CharacterStats>();
                    inventory = player.GetComponent<Inventory>();
                }
                return;
            }

            if (stats == null || inventory == null) return;

            currentTooltip = "";

            // 1. Draw Cube World Center Crosshair (when cursor is locked in free camera mode)
            if (!player.isCursorMode)
            {
                DrawCenterCrosshair();
            }

            // 2. Draw Top Left Guide & Controls Box
            DrawControlsOverlay();

            // 3. Draw Right Side Panel (RotMG + Trove Forge Layout)
            float panelWidth = 250f;
            float panelHeight = Screen.height - 20f;
            Rect sidePanelRect = new Rect(Screen.width - panelWidth - 10f, 10f, panelWidth, panelHeight);
            GUI.DrawTexture(sidePanelRect, panelTexture);

            float curY = 15f;

            // Character Header & Class
            string headerText = $"{stats.characterClass.ToString().ToUpper()} (Lvl {stats.level})";
            GUI.Label(new Rect(sidePanelRect.x, curY, panelWidth, 20f), headerText, headerStyle);
            curY += 22f;

            // Fame & Trove Flux Currencies
            string curLabel = $"Fame: {stats.totalFame} ★  |  <color=#ffd700>Flux: {inventory.flux} ⬡</color>";
            GUI.Label(new Rect(sidePanelRect.x + 8f, curY, panelWidth - 16f, 18f), curLabel, statTextStyle);
            curY += 20f;

            // HP Bar
            DrawBar(sidePanelRect.x + 10f, curY, panelWidth - 20f, 18f, stats.currentHP, stats.TotalMaxHP,
                redTexture, darkRedTexture, $"HP: {Mathf.CeilToInt(stats.currentHP)} / {stats.TotalMaxHP}");
            curY += 22f;

            // MP Bar
            DrawBar(sidePanelRect.x + 10f, curY, panelWidth - 20f, 18f, stats.currentMP, stats.TotalMaxMP,
                blueTexture, darkBlueTexture, $"MP: {Mathf.CeilToInt(stats.currentMP)} / {stats.TotalMaxMP}");
            curY += 22f;

            // EXP Bar
            string expText = stats.level >= 20 ? "MAX LEVEL 20" : $"EXP: {stats.currentExp} / {stats.expToNextLevel}";
            DrawBar(sidePanelRect.x + 10f, curY, panelWidth - 20f, 15f, stats.currentExp, stats.expToNextLevel,
                greenTexture, darkRedTexture, expText);
            curY += 24f;

            // 8 Stats Section
            GUI.Box(new Rect(sidePanelRect.x + 8f, curY, panelWidth - 16f, 95f), GUIContent.none);
            DrawStatText(sidePanelRect.x + 12f, curY + 2f, StatType.Attack, "ATT", stats.baseStats.attack, stats.equipBonus.attack, stats.statCaps.attack);
            DrawStatText(sidePanelRect.x + 125f, curY + 2f, StatType.Defense, "DEF", stats.baseStats.defense, stats.equipBonus.defense, stats.statCaps.defense);

            DrawStatText(sidePanelRect.x + 12f, curY + 24f, StatType.Speed, "SPD", stats.baseStats.speed, stats.equipBonus.speed, stats.statCaps.speed);
            DrawStatText(sidePanelRect.x + 125f, curY + 24f, StatType.Dexterity, "DEX", stats.baseStats.dexterity, stats.equipBonus.dexterity, stats.statCaps.dexterity);

            DrawStatText(sidePanelRect.x + 12f, curY + 46f, StatType.Vitality, "VIT", stats.baseStats.vitality, stats.equipBonus.vitality, stats.statCaps.vitality);
            DrawStatText(sidePanelRect.x + 125f, curY + 46f, StatType.Wisdom, "WIS", stats.baseStats.wisdom, stats.equipBonus.wisdom, stats.statCaps.wisdom);

            int maxed = stats.CountMaxedStats();
            string maxedLabel = maxed > 0 ? $"{maxed}/8 MAXED" : "0/8 MAXED";
            GUI.Label(new Rect(sidePanelRect.x + 12f, curY + 70f, panelWidth - 24f, 18f), $"Stats Status: {maxedLabel}", headerStyle);

            curY += 105f;

            // Equipment Slots (4 slots)
            GUI.Label(new Rect(sidePanelRect.x + 10f, curY, panelWidth - 20f, 18f), "EQUIPMENT", headerStyle);
            curY += 20f;

            float slotSize = 52f;
            float slotGap = 5f;
            float eqStartX = sidePanelRect.x + (panelWidth - (4 * slotSize + 3 * slotGap)) * 0.5f;

            DrawEquipSlot(new Rect(eqStartX, curY, slotSize, slotSize), inventory.weaponSlot, ItemType.Weapon, "Wep");
            DrawEquipSlot(new Rect(eqStartX + slotSize + slotGap, curY, slotSize, slotSize), inventory.abilitySlot, ItemType.Ability, "Abil");
            DrawEquipSlot(new Rect(eqStartX + (slotSize + slotGap) * 2, curY, slotSize, slotSize), inventory.armorSlot, ItemType.Armor, "Arm");
            DrawEquipSlot(new Rect(eqStartX + (slotSize + slotGap) * 3, curY, slotSize, slotSize), inventory.ringSlot, ItemType.Ring, "Ring");

            curY += slotSize + 10f;

            // Backpack Slots (8 slots)
            GUI.Label(new Rect(sidePanelRect.x + 10f, curY, panelWidth - 20f, 18f), "INVENTORY (1-8)", headerStyle);
            curY += 20f;

            for (int r = 0; r < 2; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    int index = r * 4 + c;
                    Rect slotRect = new Rect(eqStartX + c * (slotSize + slotGap), curY + r * (slotSize + slotGap), slotSize, slotSize);
                    DrawBackpackSlot(slotRect, index);
                }
            }

            curY += (slotSize + slotGap) * 2 + 6f;

            // Quick Potions (Health F, Magic V)
            Rect hpPotRect = new Rect(sidePanelRect.x + 12f, curY, 110f, 26f);
            if (GUI.Button(hpPotRect, $"HP Pot ({inventory.healthPotions}) [F]"))
            {
                inventory.UseHealthPotion();
            }
            Rect mpPotRect = new Rect(sidePanelRect.x + 128f, curY, 110f, 26f);
            if (GUI.Button(mpPotRect, $"MP Pot ({inventory.magicPotions}) [V]"))
            {
                inventory.UseMagicPotion();
            }

            curY += 32f;

            // Trove Forge Station Section
            DrawForgeSection(sidePanelRect.x + 10f, curY, panelWidth - 20f);
            curY += 88f;

            // Ground Loot Bag Container (if player is near a bag)
            LootBag nearBag = GetNearestLootBag();
            if (nearBag != null && nearBag.items.Count > 0)
            {
                GUI.Label(new Rect(sidePanelRect.x + 10f, curY, panelWidth - 20f, 18f), $"GROUND LOOT ({nearBag.bagTier.ToString().ToUpper()} BAG)", headerStyle);
                curY += 20f;

                for (int i = 0; i < nearBag.items.Count && i < 4; i++)
                {
                    ItemData groundItem = nearBag.items[i];
                    Rect gSlot = new Rect(eqStartX + i * (slotSize + slotGap), curY, slotSize, slotSize);
                    DrawGroundSlot(gSlot, nearBag, i, groundItem);
                }
                curY += slotSize + 8f;
            }

            // Nexus Recall Button
            Rect nexusBtnRect = new Rect(sidePanelRect.x + 15f, panelHeight - 38f, panelWidth - 30f, 30f);
            GUI.backgroundColor = new Color(0.9f, 0.75f, 0.1f);
            if (GUI.Button(nexusBtnRect, "NEXUS RECALL [R]"))
            {
                player.TeleportToNexus();
            }
            GUI.backgroundColor = Color.white;

            // Draw Tooltip on top
            if (!string.IsNullOrEmpty(currentTooltip))
            {
                Vector2 mouse = Event.current.mousePosition;
                Rect tipRect = new Rect(mouse.x - 240f, mouse.y - 110f, 230f, 105f);
                GUI.Box(tipRect, currentTooltip, tooltipStyle);
            }
        }

        private void DrawCenterCrosshair()
        {
            float cx = Screen.width * 0.5f;
            float cy = Screen.height * 0.5f;

            // 4-tick Cube World crosshair
            GUI.DrawTexture(new Rect(cx - 10f, cy - 1f, 7f, 2f), crosshairTexture);
            GUI.DrawTexture(new Rect(cx + 3f, cy - 1f, 7f, 2f), crosshairTexture);
            GUI.DrawTexture(new Rect(cx - 1f, cy - 10f, 2f, 7f), crosshairTexture);
            GUI.DrawTexture(new Rect(cx - 1f, cy + 3f, 2f, 7f), crosshairTexture);
        }

        private void DrawControlsOverlay()
        {
            Rect boxRect = new Rect(15f, 15f, 260f, 155f);
            GUI.DrawTexture(boxRect, panelTexture);

            GUI.Label(new Rect(25f, 18f, 240f, 20f), "CUBE WORLD x ROTMG x TROVE", headerStyle);
            GUI.Label(new Rect(25f, 38f, 240f, 18f), "WASD: Move  |  Mouse: Free Look", statTextStyle);
            GUI.Label(new Rect(25f, 56f, 240f, 18f), "Scroll Wheel: Zoom Camera In/Out", statTextStyle);
            GUI.Label(new Rect(25f, 74f, 240f, 18f), "Left Click: Autofire  |  Space: Ability", statTextStyle);
            GUI.Label(new Rect(25f, 92f, 240f, 18f), "<color=#00ffff>[TAB / ALT]: Free Cursor for UI/Forge</color>", statTextStyle);
            GUI.Label(new Rect(25f, 110f, 240f, 18f), "R: Nexus  |  E: Loot Bag  |  F/V: Potions", statTextStyle);
            GUI.Label(new Rect(25f, 128f, 240f, 18f), "Portals: Realm, Candoria, Neon, Fae, Frost", statTextStyle);
        }

        private void DrawForgeSection(float x, float y, float w)
        {
            GUI.Box(new Rect(x, y, w, 84f), GUIContent.none);
            GUI.Label(new Rect(x, y + 2f, w, 18f), "TROVE FORGE STATION", headerStyle);

            string targetName = selectedItemForForge != null ? $"{selectedItemForForge.name} {selectedItemForForge.GetStarString()}" : "(Select item in backpack)";
            GUI.Label(new Rect(x + 5f, y + 20f, w - 10f, 18f), targetName, statTextStyle);

            int forgeCost = selectedItemForForge != null ? 50 + (selectedItemForForge.stars * 75) + ((int)selectedItemForForge.troveRarity * 25) : 0;
            string forgeBtnText = selectedItemForForge != null && selectedItemForForge.stars < 5 ? $"Forge Star ({forgeCost} Flux)" : "Forge (5★ Max)";

            GUI.backgroundColor = new Color(1f, 0.75f, 0.1f);
            if (GUI.Button(new Rect(x + 5f, y + 40f, (w - 15f) * 0.65f, 24f), forgeBtnText))
            {
                if (selectedItemForForge != null)
                {
                    if (inventory.ForgeUpgradeItem(selectedItemForForge))
                    {
                        SoundManager.Instance?.PlayLevelUp();
                        DamageTextManager.Instance?.SpawnStatus(player.transform.position, $"FORGED {selectedItemForForge.GetStarString()}!", Color.yellow);
                    }
                    else
                    {
                        DamageTextManager.Instance?.SpawnStatus(player.transform.position, "Not Enough Flux!", Color.red);
                    }
                }
            }

            GUI.backgroundColor = new Color(0.7f, 0.4f, 1f);
            string pearlBtnText = $"Pearl ({inventory.pearlsOfWisdomInventory} 🔮)";
            if (GUI.Button(new Rect(x + 10f + (w - 15f) * 0.65f, y + 40f, (w - 15f) * 0.35f, 24f), pearlBtnText))
            {
                if (selectedItemForForge != null)
                {
                    if (inventory.ApplyPearlToItem(selectedItemForForge))
                    {
                        SoundManager.Instance?.PlayLevelUp();
                        DamageTextManager.Instance?.SpawnStatus(player.transform.position, "PEARL ENCHANTED!", Color.magenta);
                    }
                }
            }
            GUI.backgroundColor = Color.white;

            GUI.Label(new Rect(x + 5f, y + 66f, w - 10f, 16f), "Stars give +20% stats, Pearls give +10%!", statTextStyle);
        }

        private void DrawBar(float x, float y, float w, float h, float cur, float max, Texture2D fillTex, Texture2D bgTex, string label)
        {
            Rect barRect = new Rect(x, y, w, h);
            GUI.DrawTexture(barRect, bgTex);

            float pct = Mathf.Clamp01(cur / Mathf.Max(1f, max));
            Rect fillRect = new Rect(x, y, w * pct, h);
            GUI.DrawTexture(fillRect, fillTex);

            GUI.Label(barRect, label, barTextStyle);
        }

        private void DrawStatText(float x, float y, StatType stat, string name, int baseVal, int bonusVal, int cap)
        {
            bool isMaxed = baseVal >= cap;
            string colorHex = isMaxed ? "#ffd700" : (bonusVal > 0 ? "#00ff66" : "#ffffff");
            string bonusStr = bonusVal > 0 ? $" (+{bonusVal})" : "";
            string maxStr = isMaxed ? " [MAX]" : "";
            string label = $"<b>{name}:</b> <color={colorHex}>{baseVal}{bonusStr}{maxStr}</color>";

            GUI.Label(new Rect(x, y, 110f, 20f), label, statTextStyle);
        }

        private void DrawEquipSlot(Rect rect, ItemData item, ItemType type, string placeholder)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = item != null ? TroveRarityColors.GetColor(item.troveRarity) : new Color(0.3f, 0.3f, 0.35f);

            string btnText = item != null ? $"{item.name}\n{item.GetStarString()}" : $"[{placeholder}]";
            if (GUI.Button(rect, btnText, slotTextStyle))
            {
                if (item != null)
                {
                    selectedItemForForge = item;
                    // Right click or double click unequipped
                    if (Event.current.button == 1)
                    {
                        inventory.UnequipToBackpack(type);
                    }
                }
            }

            GUI.backgroundColor = prev;

            if (rect.Contains(Event.current.mousePosition) && item != null)
            {
                currentTooltip = BuildTooltip(item);
            }
        }

        private void DrawBackpackSlot(Rect rect, int index)
        {
            ItemData item = inventory.backpack[index];
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = item != null ? TroveRarityColors.GetColor(item.troveRarity) : new Color(0.2f, 0.2f, 0.25f);

            string text = item != null ? $"{item.name}\n{item.GetStarString()}" : $"{index + 1}";
            if (GUI.Button(rect, text, slotTextStyle))
            {
                if (item != null)
                {
                    selectedItemForForge = item;
                    inventory.EquipFromBackpack(index);
                }
            }

            GUI.backgroundColor = prev;

            if (rect.Contains(Event.current.mousePosition) && item != null)
            {
                currentTooltip = BuildTooltip(item);
            }
        }

        private void DrawGroundSlot(Rect rect, LootBag bag, int itemIndex, ItemData item)
        {
            Color prev = GUI.backgroundColor;
            GUI.backgroundColor = TroveRarityColors.GetColor(item.troveRarity);

            if (GUI.Button(rect, $"Take\n{item.name}", slotTextStyle))
            {
                ItemData taken = bag.TakeItem(itemIndex);
                if (taken != null)
                {
                    if (!inventory.AddItem(taken))
                    {
                        bag.items.Insert(itemIndex, taken);
                    }
                    else
                    {
                        SoundManager.Instance?.PlayHitEnemy(player.transform.position);
                    }
                }
            }

            GUI.backgroundColor = prev;

            if (rect.Contains(Event.current.mousePosition))
            {
                currentTooltip = BuildTooltip(item);
            }
        }

        private string BuildTooltip(ItemData item)
        {
            string rarityTag = item.GetTroveRarityTag();
            string stars = item.GetStarString();
            string pearls = item.GetPearlString();
            float mult = item.GetTrovePowerMultiplier();

            return $"<b>{item.name}</b> {rarityTag}{pearls}\n" +
                   $"Power Mult: {mult:F2}x | Tier: {item.GetTierString()}\n" +
                   $"{item.description}";
        }

        private LootBag GetNearestLootBag()
        {
            if (player == null) return null;
            LootBag nearest = null;
            float minDist = 3.0f;
            foreach (var bag in LootBag.ActiveBags)
            {
                if (bag == null) continue;
                float d = Vector3.Distance(player.transform.position, bag.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    nearest = bag;
                }
            }
            return nearest;
        }
    }
}
