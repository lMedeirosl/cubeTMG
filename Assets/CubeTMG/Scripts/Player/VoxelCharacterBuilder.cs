using UnityEngine;
using CubeTMG.Stats;

namespace CubeTMG.Player
{
    public class VoxelCharacterBuilder : MonoBehaviour
    {
        [Header("Character Parts")]
        public Transform head;
        public Transform torso;
        public Transform leftArm;
        public Transform rightArm;
        public Transform leftLeg;
        public Transform rightLeg;
        public Transform weaponHolder;

        [Header("Weapon Visual")]
        public GameObject currentWeaponVisual;

        private static Material sharedVoxelMat;

        public static Material GetVoxelMaterial()
        {
            if (sharedVoxelMat == null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard") ?? Shader.Find("Sprites/Default");
                sharedVoxelMat = new Material(shader);
            }
            return sharedVoxelMat;
        }

        public void BuildVoxelCharacter(PlayerClassType pClass)
        {
            // Clear existing children
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            Color skinColor = new Color(0.96f, 0.76f, 0.65f);
            Color hairColor = new Color(0.35f, 0.2f, 0.1f);
            Color shirtColor;
            Color pantsColor = new Color(0.2f, 0.25f, 0.35f);
            Color hatColor;

            switch (pClass)
            {
                case PlayerClassType.Warrior:
                    shirtColor = new Color(0.7f, 0.15f, 0.15f); // Red warrior tunic
                    hatColor = new Color(0.6f, 0.62f, 0.65f);   // Steel horned helm
                    break;
                case PlayerClassType.Priest:
                    shirtColor = new Color(0.92f, 0.92f, 0.95f); // White holy robe
                    hatColor = new Color(0.95f, 0.85f, 0.3f);    // Golden halo/hood
                    break;
                case PlayerClassType.Archer:
                    shirtColor = new Color(0.2f, 0.55f, 0.25f); // Forest green
                    hatColor = new Color(0.15f, 0.45f, 0.2f);   // Archer cap
                    break;
                case PlayerClassType.Knight:
                    shirtColor = new Color(0.35f, 0.45f, 0.6f); // Heavy steel blue
                    hatColor = new Color(0.5f, 0.55f, 0.6f);    // Knight helmet
                    break;
                case PlayerClassType.Wizard:
                default:
                    shirtColor = new Color(0.15f, 0.35f, 0.85f); // Royal blue robe
                    hatColor = new Color(0.12f, 0.25f, 0.7f);    // Wizard pointed hat
                    break;
            }

            // Torso (0.5w x 0.6h x 0.3d)
            torso = CreateVoxelBox("Torso", transform, new Vector3(0, 0.75f, 0), new Vector3(0.55f, 0.6f, 0.35f), shirtColor);

            // Head (0.45w x 0.45h x 0.45d)
            head = CreateVoxelBox("Head", torso, new Vector3(0, 0.55f, 0), new Vector3(0.48f, 0.48f, 0.48f), skinColor);

            // Hair/Hat
            CreateVoxelBox("Hair", head, new Vector3(0, 0.15f, -0.05f), new Vector3(0.5f, 0.25f, 0.5f), hairColor);
            BuildClassHat(head, pClass, hatColor);

            // Cute Eyes (two dark small cubes)
            CreateVoxelBox("LeftEye", head, new Vector3(-0.12f, 0.02f, 0.25f), new Vector3(0.08f, 0.08f, 0.05f), Color.black);
            CreateVoxelBox("RightEye", head, new Vector3(0.12f, 0.02f, 0.25f), new Vector3(0.08f, 0.08f, 0.05f), Color.black);

            // Arms (Pivoting at shoulders)
            leftArm = CreateVoxelBox("LeftArm", torso, new Vector3(-0.38f, 0.05f, 0), new Vector3(0.2f, 0.55f, 0.2f), shirtColor);
            rightArm = CreateVoxelBox("RightArm", torso, new Vector3(0.38f, 0.05f, 0), new Vector3(0.2f, 0.55f, 0.2f), shirtColor);

            // Weapon holder attached to right hand
            GameObject wHolder = new GameObject("WeaponHolder");
            wHolder.transform.SetParent(rightArm, false);
            wHolder.transform.localPosition = new Vector3(0, -0.25f, 0.18f);
            weaponHolder = wHolder.transform;

            // Legs (Pivoting at hips)
            leftLeg = CreateVoxelBox("LeftLeg", transform, new Vector3(-0.16f, 0.25f, 0), new Vector3(0.22f, 0.5f, 0.24f), pantsColor);
            rightLeg = CreateVoxelBox("RightLeg", transform, new Vector3(0.16f, 0.25f, 0), new Vector3(0.22f, 0.5f, 0.24f), pantsColor);

            // Build weapon model
            BuildClassWeapon(pClass);
        }

        private void BuildClassHat(Transform parentHead, PlayerClassType pClass, Color color)
        {
            switch (pClass)
            {
                case PlayerClassType.Wizard:
                    // Pointed wizard hat
                    CreateVoxelBox("Brim", parentHead, new Vector3(0, 0.28f, 0), new Vector3(0.7f, 0.08f, 0.7f), color);
                    CreateVoxelBox("Cone1", parentHead, new Vector3(0, 0.42f, 0), new Vector3(0.45f, 0.22f, 0.45f), color);
                    CreateVoxelBox("Cone2", parentHead, new Vector3(0, 0.62f, 0), new Vector3(0.25f, 0.22f, 0.25f), color);
                    CreateVoxelBox("Tip", parentHead, new Vector3(0.04f, 0.8f, 0.04f), new Vector3(0.12f, 0.16f, 0.12f), new Color(1f, 0.85f, 0.2f));
                    break;

                case PlayerClassType.Warrior:
                    // Horned helmet
                    CreateVoxelBox("HelmBase", parentHead, new Vector3(0, 0.28f, 0), new Vector3(0.55f, 0.22f, 0.55f), color);
                    CreateVoxelBox("HornL", parentHead, new Vector3(-0.35f, 0.4f, 0), new Vector3(0.12f, 0.25f, 0.12f), Color.white);
                    CreateVoxelBox("HornR", parentHead, new Vector3(0.35f, 0.4f, 0), new Vector3(0.12f, 0.25f, 0.12f), Color.white);
                    break;

                case PlayerClassType.Priest:
                    // Golden Angelic Halo
                    CreateVoxelBox("Halo", parentHead, new Vector3(0, 0.45f, 0), new Vector3(0.45f, 0.06f, 0.45f), new Color(1f, 0.9f, 0.2f));
                    break;

                case PlayerClassType.Archer:
                    // Hunting Cap with Red Feather
                    CreateVoxelBox("Cap", parentHead, new Vector3(0, 0.3f, 0), new Vector3(0.52f, 0.16f, 0.55f), color);
                    CreateVoxelBox("Feather", parentHead, new Vector3(0.24f, 0.45f, -0.1f), new Vector3(0.06f, 0.25f, 0.06f), Color.red);
                    break;

                case PlayerClassType.Knight:
                    // Knight Greathelm
                    CreateVoxelBox("Greathelm", parentHead, new Vector3(0, 0.1f, 0.05f), new Vector3(0.54f, 0.5f, 0.52f), color);
                    CreateVoxelBox("VisorSlit", parentHead, new Vector3(0, 0.04f, 0.32f), new Vector3(0.38f, 0.06f, 0.04f), Color.black);
                    break;
            }
        }

        public void BuildClassWeapon(PlayerClassType pClass)
        {
            if (currentWeaponVisual != null)
            {
                Destroy(currentWeaponVisual);
            }

            currentWeaponVisual = new GameObject("WeaponVisual");
            currentWeaponVisual.transform.SetParent(weaponHolder, false);

            switch (pClass)
            {
                case PlayerClassType.Wizard:
                    // Arcane Staff: wooden rod + glowing crystal top
                    CreateVoxelBox("Rod", currentWeaponVisual.transform, new Vector3(0, 0.2f, 0), new Vector3(0.08f, 0.9f, 0.08f), new Color(0.45f, 0.25f, 0.1f));
                    CreateVoxelBox("Gem", currentWeaponVisual.transform, new Vector3(0, 0.72f, 0), new Vector3(0.22f, 0.22f, 0.22f), new Color(0.2f, 0.8f, 1f));
                    break;

                case PlayerClassType.Warrior:
                case PlayerClassType.Knight:
                    // Broadsword: steel blade + gold crossguard
                    CreateVoxelBox("Blade", currentWeaponVisual.transform, new Vector3(0, 0.4f, 0), new Vector3(0.12f, 0.8f, 0.04f), new Color(0.85f, 0.88f, 0.92f));
                    CreateVoxelBox("Guard", currentWeaponVisual.transform, new Vector3(0, 0.02f, 0), new Vector3(0.35f, 0.08f, 0.1f), new Color(0.9f, 0.75f, 0.1f));
                    CreateVoxelBox("Hilt", currentWeaponVisual.transform, new Vector3(0, -0.15f, 0), new Vector3(0.08f, 0.25f, 0.08f), new Color(0.3f, 0.18f, 0.08f));
                    break;

                case PlayerClassType.Priest:
                    // Holy Wand: golden shaft + radiant sun orb
                    CreateVoxelBox("Shaft", currentWeaponVisual.transform, new Vector3(0, 0.2f, 0), new Vector3(0.08f, 0.7f, 0.08f), new Color(0.85f, 0.75f, 0.2f));
                    CreateVoxelBox("Orb", currentWeaponVisual.transform, new Vector3(0, 0.6f, 0), new Vector3(0.2f, 0.2f, 0.2f), new Color(1f, 1f, 0.6f));
                    break;

                case PlayerClassType.Archer:
                    // Wooden recurve bow
                    CreateVoxelBox("BowMiddle", currentWeaponVisual.transform, new Vector3(0, 0.1f, 0.1f), new Vector3(0.08f, 0.3f, 0.08f), new Color(0.5f, 0.3f, 0.12f));
                    CreateVoxelBox("BowTop", currentWeaponVisual.transform, new Vector3(0, 0.38f, 0.02f), new Vector3(0.07f, 0.32f, 0.07f), new Color(0.5f, 0.3f, 0.12f));
                    CreateVoxelBox("BowBottom", currentWeaponVisual.transform, new Vector3(0, -0.18f, 0.02f), new Vector3(0.07f, 0.32f, 0.07f), new Color(0.5f, 0.3f, 0.12f));
                    break;
            }
        }

        private static Transform CreateVoxelBox(string name, Transform parent, Vector3 localPos, Vector3 size, Color color)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPos;
            cube.transform.localScale = size;
            Destroy(cube.GetComponent<BoxCollider>()); // Clean character mesh without extra physics colliders

            Material mat = new Material(GetVoxelMaterial());
            mat.color = color;
            cube.GetComponent<MeshRenderer>().material = mat;

            return cube.transform;
        }

        public void AnimateWalk(float speedRatio)
        {
            if (torso == null || leftLeg == null || rightLeg == null) return;

            float walkCycle = Time.time * 12f * Mathf.Clamp01(speedRatio);
            float legAngle = Mathf.Sin(walkCycle) * 35f;
            float armAngle = -legAngle;
            float bobbing = Mathf.Abs(Mathf.Sin(walkCycle)) * 0.08f;

            // Torso bobbing
            torso.localPosition = new Vector3(0, 0.75f + bobbing, 0);

            // Legs swinging
            leftLeg.localRotation = Quaternion.Euler(legAngle, 0, 0);
            rightLeg.localRotation = Quaternion.Euler(-legAngle, 0, 0);

            // Arms swinging
            if (leftArm != null) leftArm.localRotation = Quaternion.Euler(armAngle, 0, 0);
            if (rightArm != null) rightArm.localRotation = Quaternion.Euler(-armAngle * 0.5f, 0, 0);
        }

        public void AnimateIdle()
        {
            if (torso == null) return;
            float breathe = Mathf.Sin(Time.time * 2.5f) * 0.02f;
            torso.localPosition = new Vector3(0, 0.75f + breathe, 0);
            if (leftLeg != null) leftLeg.localRotation = Quaternion.identity;
            if (rightLeg != null) rightLeg.localRotation = Quaternion.identity;
            if (leftArm != null) leftArm.localRotation = Quaternion.identity;
            if (rightArm != null) rightArm.localRotation = Quaternion.identity;
        }
    }
}
