using UnityEngine;
using TMPro;


namespace CevarnsOfEvil {

    public class ItemStack : Item
    {
        [SerializeField] StackableItem item;
        [SerializeField] TMP_Text amountText;
        [SerializeField] float useTime = 0.5f;

        // This works because it is only ever used for potions
        static int stackSize = 0; 
        private float usableTime;


        public void Start()
        {
            usableTime = Time.time;
        }


        public static void PotionInit()
        {
            stackSize = 0;
        }


        public override void Init()
        {
            equiped = stackSize > 0;
            hotbarScript = itemViewSlot.GetComponent<HotbarSlotControl>();
            if (equiped)
            {
                amountText.text = stackSize.ToString();
                hotbarScript.Activate();
            }
        }


        public override string ItemName { get => item.ItemName; }


        public override void OnMobUse(Entity mob)
        {
            usableTime = Time.time + useTime;
            if ((stackSize > 0) && (Time.time > usableTime))
            {
                item.OnMobUse(mob);
                stackSize--;
            }
        }


        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if ((stackSize > 0) && (Time.time > usableTime))
            {
                usableTime = Time.time + useTime;
                item.OnPlayerUse(player, anim);
                stackSize--;
                amountText.text = stackSize.ToString();
            }
            if(stackSize < 1)
            {
                BeRemoved();
                anim.SetInteger("Item", -1);
                anim.SetTrigger("Swap");
            }
        }


        public bool CanAddItem()
        {
            return stackSize < item.maxStackSize;
        }


        public bool CanAddItems(int number)
        {
            return stackSize <= item.maxStackSize + number;
        }


        public bool AddItem()
        {
            if(CanAddItem())
            {
                stackSize++;
                amountText.text = stackSize.ToString();
                return true;
            }
            return false;
        }


        public bool AddItems(int number)
        {
            if (CanAddItems(number))
            {
                stackSize +=  number;
                amountText.text = stackSize.ToString();
                return true;
            }
            return false;
        }


        public override void BeAcquired()
        {
            hotbarScript.Activate();
            equiped = true;
            AddItem();
        }


        public override void BeRemoved()
        {
            hotbarScript.Deactivate();
            equiped = false;
            gameObject.SetActive(false);
            stackSize = 0;
            amountText.text = stackSize.ToString();
        }
    }

}