using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public abstract class StackableItem : MonoBehaviour, IUsable
    {
        [SerializeField] string itemName;
        public int preferredSlot;
        public int maxStackSize;
        
        public string ItemName { get => itemName; }

        public abstract void OnMobUse(Entity mob);


        public abstract void OnPlayerUse(PlayerAct player, Animator anim);

    }

}
