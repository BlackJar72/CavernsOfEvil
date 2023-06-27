using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/Pickup List", fileName = "PickupList", order = 201)]
    public class PickupList : ScriptableObject
    {
        [SerializeField] ItemEntry[] items;
        public ItemEntry[] Items { get { return items; } }
    }


    [System.Serializable]
    public class PickupLists
    {
        [SerializeField] PickupList weapons;
        [SerializeField] PickupList armors;
        [SerializeField] PickupList ammo;
        [SerializeField] PickupList healing;

        public PickupList Weapons { get { return weapons; } }
        public PickupList Armors { get { return armors; } } 
        public PickupList Ammo { get { return ammo; } }
        public PickupList Healing { get { return healing; } }
    }

}