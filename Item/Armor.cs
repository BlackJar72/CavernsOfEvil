using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil
{

    public class Armor : MonoBehaviour
    {
        [SerializeField] string itemName;
        [SerializeField] int armorID;
        [SerializeField] GameObject armorSlot;
        [SerializeField] int armorValue;
        [SerializeField] Sprite icon;
        public bool equiped = false;
        [SerializeField] protected int fullDurability;
        [SerializeField] protected AdvancedBarScaler durabilityScaler;

        private int durability;
        private HotbarSlotControl hotbarScript;
        private EntityHealth health;

        private static bool[] worn = new bool[5];
        private static int savedDurability;

        public int Durability { get { return fullDurability; } }
        public int RemainingDurability { get { return durability; } }
        public int ArmorID { get { return armorID; } }
        public GameObject ArmorSlot { get { return armorSlot; } }
        public int ArmorValue { get { return armorValue; } } 
        public bool Equiped { get { return equiped; } }


        public virtual void Init(EntityHealth playerHealth)
        {
            hotbarScript = armorSlot.GetComponent<HotbarSlotControl>();
            equiped = worn[armorID];
            health = playerHealth;
            if (equiped)
            {
                hotbarScript.Activate();
                hotbarScript.Change(icon);
                gameObject.SetActive(true);
                durability = savedDurability;
                durabilityScaler.SetBar(durability, fullDurability);
                durabilityScaler.Activate();
                health.Armor = armorValue;
                worn[armorID] = true;
            }
        }


        public static void Init()
        {
            worn[0] = false;
            worn[1] = false;
            worn[2] = false;
            worn[3] = false;
            worn[4] = false;
            savedDurability = 0;
        }


        public void BeDamaged(Damages damage)
        {
            if (equiped)
            {
                savedDurability = durability = Mathf.Clamp(durability - damage.toArmor,
                        0, fullDurability);
                durabilityScaler.SetBar(durability, fullDurability);
                if (durability < 1) BeRemoved();
            }
        }


        public void BeAcquired()
        {
            hotbarScript.Activate();
            hotbarScript.Change(icon);
            equiped = true;
            gameObject.SetActive(true);
            savedDurability = durability = fullDurability;
            durabilityScaler.SetBar(1.0f);
            durabilityScaler.Activate();
            health.Armor = armorValue;
            worn[armorID] = true;
        }


        public void BeRemoved()
        {
            hotbarScript.Deactivate();
            equiped = false;
            gameObject.SetActive(false);
            savedDurability = durability = 0;
            durabilityScaler.SetBar(0.0f);
            durabilityScaler.Deactivate();
            health.Armor = 0;
            worn[armorID] = false;
        }


        public bool ShouldTake(Armor other)
        {
            return (other == null) || (other.armorValue <= armorValue);
        }

    }

}
