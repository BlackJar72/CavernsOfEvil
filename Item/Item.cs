using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;


namespace DLD {

    public abstract class Item : MonoBehaviour, IInventoryItem
    {
        [SerializeField] string itemName;
        public int preferredSlot;
        public GameObject itemViewSlot;
        public int idForAnimator;
        public bool isMelee = false;
        public Sprite icon;
        public bool equiped = false;

        public Transform aimTransform;

        protected HotbarSlotControl hotbarScript;

        public int soundRadius = 0;

        public static bool[] acquired = new bool[9];

        public virtual string ItemName { get => itemName; }


        public virtual void Init()
        {
            equiped = acquired[preferredSlot];
            hotbarScript = itemViewSlot.GetComponent<HotbarSlotControl>();
            if(equiped) hotbarScript.Activate();
        }


        public abstract void OnMobUse(Entity mob);


        public static void StaticInit()
        {
            acquired[0] = true;
            acquired[1] = true;
            acquired[2] = false;
            acquired[3] = false;
            acquired[4] = false;
            acquired[5] = false;
            acquired[6] = false;
            acquired[7] = false;
            acquired[8] = false;
        }


        public virtual void OnPlayerSelect(PlayerAct player)
        {
            if (aimTransform != null)
            {
                AimIK aimIk = player.gameObject.GetComponent<AimIK>();
                aimIk.solver.transform = aimTransform;
            }

            gameObject.SetActive(true);
            hotbarScript.Select();
        }


        public virtual void OnPlayerDeselect(PlayerAct player)
        {
            gameObject.SetActive(false);
            hotbarScript.Deselect();
        }


        public abstract void OnPlayerUse(PlayerAct player, Animator anim);


        ///<summary>
        /// Used for items that have continuous effects that should not be restarted 
        /// each frame.
        ///</summary>
        public virtual void OnPlayerEndUse() {/*For most things, do nothing*/}


        public virtual int GetIdForAnim()
        {
            return idForAnimator;
        }


        public virtual void BeAcquired()
        {
            hotbarScript.Activate();
            equiped = true;
            acquired[preferredSlot] = true;
        }


        public virtual void BeRemoved()
        {
            hotbarScript.Deactivate();
            equiped = false;
            acquired[preferredSlot] = false;
            gameObject.SetActive(false);
        }


        public virtual void AlertListeningMobs(Player user)
        {
            Collider[] colliders = Physics.OverlapSphere(user.transform.position, soundRadius, GameConstants.MobMask);
            foreach (Collider collider in colliders)
            {
                EntityHealth health = collider.gameObject.GetComponent<EntityHealth>();
                if ((health != null) && (health.Owner != null) && (health.Owner is EntityMob))
                {
                    (health.Owner as EntityMob).ListenForPlayer(user.gameObject);
                }
            }
        }


    }


}