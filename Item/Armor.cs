using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil
{

    [System.Serializable]
    public struct ArmorData
    {
        public int durability;
        public bool equiped;
        

        public static ArmorData MakeArmorData(Armor armor)
        {
            return new() 
            { 
                durability = armor.durability, 
                equiped = armor.equiped
            };
        }
    }



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

        public int durability;
        private HotbarSlotControl hotbarScript;
        private EntityHealth health;

        public int Durability { get { return fullDurability; } }
        public int RemainingDurability { get { return durability; } }
        public int ArmorID { get { return armorID; } }
        public GameObject ArmorSlot { get { return armorSlot; } }
        public int ArmorValue { get { return armorValue; } } 
        public bool Equiped { get { return equiped; } }


        public virtual void Init(EntityHealth playerHealth)
        {
            hotbarScript = armorSlot.GetComponent<HotbarSlotControl>();
            health = playerHealth;
            if (equiped)
            {
                hotbarScript.Activate();
                hotbarScript.Change(icon);
                gameObject.SetActive(true);
                durabilityScaler.SetBar(durability, fullDurability);
                durabilityScaler.Activate();
                health.Armor = armorValue;
            }
        }
        


        public void BeDamaged(Damages damage)
        {
            if (equiped)
            {
                durability -= damage.toArmor;
                durabilityScaler.SetBar(durability, fullDurability);
                if (durability < 1) BeRemoved();
            }
        }


        public void BeDamaged()
        {
            if (equiped)
            {
                durabilityScaler.SetBar(durability, fullDurability);
            }
        }


        public void BeAcquired()
        {
            hotbarScript.Activate();
            hotbarScript.Change(icon);
            equiped = true;
            gameObject.SetActive(true);
            durability = fullDurability;
            durabilityScaler.SetBar(1.0f);
            durabilityScaler.Activate();
            health.Armor = armorValue;
        }


        public void BeRemoved()
        {
            hotbarScript.Deactivate();
            equiped = false;
            gameObject.SetActive(false);
            durability = 0;
            durabilityScaler.SetBar(0.0f);
            durabilityScaler.Deactivate();
            health.Armor = 0;
        }


        public bool ShouldTake(Armor other)
        {
            return (other == null) || (other.armorValue <= armorValue);
        }

    }

}
