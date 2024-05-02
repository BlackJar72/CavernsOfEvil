using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public enum PickupType
    {
        Hotbar,
        Ammo,
        Armor
    }


    public class ItemPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] string itemName;
        [SerializeField] string translationKey;
        [SerializeField] int itemID;
        [SerializeField] int ammoAmount;
        [SerializeField] PickupType type = PickupType.Hotbar;
        [SerializeField] new TransformData transform;

        [HideInInspector] public bool touched = false;

        public string ItemName => itemName;
        public string LocalKey => translationKey;
        public int ItemID { get { return itemID; } }
        public int AmmoAmount { get { return ammoAmount; } }
        public PickupType Type { get { return type; } }
        public TransformData TransformData { get { return transform; } }
        public Vector3 Position { get { return transform.position; } }
        public Quaternion Quaternion { get { return transform.rotation; } }
        public Vector3 Scale { get { return transform.scale; } }


        public void OnInteract(Player player)
        {
            player.Actor.TakePickup(this);
        }
    }
}
