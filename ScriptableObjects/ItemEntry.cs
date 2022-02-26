using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace CevarnsOfEvil

{
    public enum ItemCategory
    {
        ammo,
        health,
        armor,
        weapon
    }


    [CreateAssetMenu(menuName = "DLD/Item Entry", fileName = "ItemEntry", order = 200)]
    public class ItemEntry : ScriptableObject
    {

        [SerializeField] GameObject worldPrefab;
        [SerializeField] ItemPickup pickUp;
        [SerializeField] int ammoValue;
        [SerializeField] int healthValue;
        [SerializeField] float level;
        [SerializeField] float rarity;
        [SerializeField] ItemCategory category;


        public GameObject WorldPrefab { get { return worldPrefab; } }
        public ItemPickup Pickup { get { return pickUp; } }
        public int AmmoValue { get { return ammoValue; } }
        public int HealthValue { get { return healthValue; } }
        public float Level { get { return level; } }
        public float Rarity { get { return rarity; } }
        public ItemCategory Category { get { return category; } }

    }

}