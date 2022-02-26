using UnityEngine;
using UnityEngine.InputSystem;
using RootMotion.FinalIK;
using TMPro;


namespace CevarnsOfEvil
{
    public static class InventoryHolder
    {
        static public int activeSlot = 0;
        static public int activeArmor = 0;

        static public AmmoType[] ammoTypes = new AmmoType[4];
        static public AmmoData[] ammo = new AmmoData[4];
    }

    
    public class PlayerAct : MonoBehaviour, IHaveInventory
    {
        private static bool newGame = true;

        [SerializeField] GameObject playerBody;

        [SerializeField] public Item[] inventory = new Item[9];
        [SerializeField] int activeSlot = 0;
        [SerializeField] public Item[] otherItems;

        [SerializeField] public AmmoType[] ammoTypes = new AmmoType[4];
        private AmmoData[] ammo = new AmmoData[4];
        [SerializeField] public TMP_Text[] ammoText = new TMP_Text[5];

        [SerializeField] public Armor[] armors = new Armor[5];
        private int activeArmor = 0;

        [SerializeField] GameObject startingSword;

        private Camera cam;
        private Animator animator;
        private MovePlayer moveScript;
        private Player player;
        private ToastController toastController;
        private Item[] allItems;
        private float slotScroll;

        [HideInInspector] public bool usingItem = false;

        public const float baseStamina = 100f;
        private float stamina;

        public int ActiveArmorSlot { get { return activeArmor; } }
        public Armor ActiveArmor { get { return armors[activeArmor]; } }
        public Player PlayerScript { get { return player; } }
        public float Stamina { get { return stamina; } set { stamina = value; } }

        // Input System
        private PlayerInput input;
        private InputAction useItemAction;
        private InputAction useObjectAction;
        private InputAction nextItemAction;
        private InputAction backItemAction;
        private InputAction scrollItemAction;
        private InputAction item1Action;
        private InputAction item2Action;
        private InputAction item3Action;
        private InputAction item4Action;
        private InputAction item5Action;
        private InputAction item6Action;
        private InputAction item7Action;
        private InputAction item8Action;
        private InputAction item9Action;


        private void Awake()
        {
            InitInput();
        }


        public void Start()
        {
            cam = gameObject.GetComponentInChildren<Camera>();
            moveScript = gameObject.GetComponent<MovePlayer>();
            player = gameObject.GetComponent<Player>();
            animator = playerBody.GetComponent<Animator>();
            toastController = playerBody.GetComponent<ToastController>();
            allItems = new Item[inventory.Length + otherItems.Length + 1];
            for (int i = 0; i < inventory.Length; i++)
            {
                allItems[i] = inventory[i];
                if ((inventory[i] != null) && (inventory[i].gameObject != null))
                {
                    inventory[i].Init();
                }
            }
            for (int i = 0; i < otherItems.Length; i++)
            {
                allItems[10 + i] = otherItems[i];
                if ((otherItems[i] != null) && (otherItems[i].gameObject != null))
                {
                    otherItems[i].Init();
                }
            }
            for (int i = 0; i < ammo.Length; i++)
            {
                ammo[i] = new AmmoData(ammoTypes[i]);
                ammoText[i].text = ammo[i].Amount.ToString();
            }
            for (int i = 0; i < armors.Length; i++)
            {
                armors[i].Init(player.Health);
            }
            if (newGame)
            {
                InventoryHolder.activeSlot = activeSlot;
                InventoryHolder.ammoTypes = ammoTypes;
                InventoryHolder.ammo = ammo;
                InventoryHolder.activeArmor = activeArmor;
                newGame = false;
                stamina = baseStamina;
            }
            else
            {
                activeSlot = InventoryHolder.activeSlot;
                ammoTypes = InventoryHolder.ammoTypes;
                ammo = InventoryHolder.ammo;
                activeArmor = InventoryHolder.activeArmor;
                for (int i = 0; i < ammo.Length; i++)
                {
                    ammoText[i].text = ammo[i].Amount.ToString();
                }
                SetSword(Sword.Held);
            }
            newGame = false;
        }


        public static void Init()
        {
            newGame = true;
        }


        private void Update()
        {
            ScrollInventory();
            if (usingItem && (inventory[activeSlot] != null) && ((activeSlot != 0) || (stamina > 10)))
            {
                inventory[activeSlot].OnPlayerUse(this, animator);
            }

        }


        public void EndLevel()
        {
            InventoryHolder.activeSlot = activeSlot;
            InventoryHolder.activeArmor = activeArmor;
        }


        public void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Pickup")) 
            {
                other.GetComponent<ItemPickup>().touched = true;
                if (PickupItem(other.GetComponent<ItemPickup>(), false))
                {
                    Destroy(other.gameObject);
                    toastController.Toast(other.GetComponent<ItemPickup>().ItemName);
                }
            }
        }


        public void TakePickup(ItemPickup pickup)
        {
            pickup.touched = true;
            PickupItem(pickup, true);
            Destroy(pickup.gameObject);
        }


        internal void Die()
        {
            EndUseItem();
            gameObject.GetComponent<AimIK>().enabled = false;
            animator.SetTrigger("Die");
        }


        private void UseItem(InputAction.CallbackContext context)
        {
            usingItem = true;
        }


        private void UseObject(InputAction.CallbackContext context)
        {
            AimParams aim;
            GetAimParams(out aim);
            RaycastHit hit;
            if (Physics.Raycast(aim.from, aim.toward, out hit, 2f, GameConstants.InteractMask))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null) interactable.OnInteract(player);
            }
        }


        public void TakeHealingPotion(int amountHealed)
        {
            player.TakeHealingPoition(amountHealed);
        }


        public bool UseAmmo(int type)
        {
            return ammo[type].Use(ammoText[type]);
        }


        private void EndUseItem(InputAction.CallbackContext context)
        {
            if(animator != null) animator.SetInteger("Action", 0);
            if (inventory[activeSlot] != null)
            {
                inventory[activeSlot].OnPlayerEndUse();
            }
            usingItem = false;
        }


        private void EndUseItem()
        {
            animator.SetInteger("Action", 0);
            if (inventory[activeSlot] != null)
            {
                inventory[activeSlot].OnPlayerEndUse();
            }
            usingItem = false;
        }


        public void GetAimParams(out AimParams aim)
        {
            aim.from = cam.transform.position;
            aim.toward = cam.transform.forward;
        }


        public Transform GetTransform()
        {
            return transform;
        }


        #region Input
        private void InitInput()
        {
            input = GetComponent<PlayerInput>();

            useItemAction = input.actions["Use Item"];
            useObjectAction = input.actions["Use Object"];
            nextItemAction = input.actions["Next Item"];
            backItemAction = input.actions["Back Item"];
            scrollItemAction = input.actions["Scroll Items"];
            item1Action = input.actions["Item 1"];
            item2Action = input.actions["Item 2"];
            item3Action = input.actions["Item 3"];
            item4Action = input.actions["Item 4"];
            item5Action = input.actions["Item 5"];
            item6Action = input.actions["Item 6"];
            item7Action = input.actions["Item 7"];
            item8Action = input.actions["Item 8"];
            item9Action = input.actions["Item 9"];

            useItemAction.started += UseItem;
            useItemAction.canceled += EndUseItem;
            useObjectAction.started += UseObject;
            nextItemAction.started += IncrementActiveSlot;
            backItemAction.started += DecrementActiveSlot;
            item1Action.started += SetSlotTo1;
            item2Action.started += SetSlotTo2;
            item3Action.started += SetSlotTo3;
            item4Action.started += SetSlotTo4;
            item5Action.started += SetSlotTo5;
            item6Action.started += SetSlotTo6;
            item7Action.started += SetSlotTo7;
            item8Action.started += SetSlotTo8;
            item9Action.started += SetSlotTo9;
        }


        #endregion



        #region Inventory Management
        /*****************************************************************/
        /*          METHODS FOR DEALING WITH THE PLAYERS INVENTORY       */
        /*****************************************************************/


        private void SetSlotTo1(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 0;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo2(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 1;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo3(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 2;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo4(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 3;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo5(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 4;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo6(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 5;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo7(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 6;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo8(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 7;
                SetInventorySlot(previous, 0);
        }


        private void SetSlotTo9(InputAction.CallbackContext context)
        {
                int previous = activeSlot;
                activeSlot = 8;
                SetInventorySlot(previous, 0);
        }


        private void SetInventorySlot(int previous, int direction)
            {
                if (activeSlot >= inventory.Length) activeSlot = 0;
            else if (activeSlot < 0) activeSlot = inventory.Length - 1;

            if (direction != 0)
            {
                while ((inventory[activeSlot] == null)
                     || !inventory[activeSlot].equiped
                     && (activeSlot != previous))
                {
                    activeSlot += direction;
                    if (activeSlot >= inventory.Length) activeSlot = 0;
                    else if (activeSlot < 0) activeSlot = inventory.Length - 1;
                }
            }

            if (activeSlot != previous)
            {
                animator.SetTrigger("Swap");
                animator.SetInteger("Item", 0);
                if ((inventory[activeSlot] != null) && inventory[activeSlot].equiped)
                {
                    inventory[previous].OnPlayerDeselect(this);
                    inventory[activeSlot].OnPlayerSelect(this);
                    animator.SetInteger("Item", inventory[activeSlot].idForAnimator);
                    gameObject.GetComponent<AimIK>().enabled = !inventory[activeSlot].isMelee;
                }
                else
                {
                    activeSlot = previous;
                }
                slotScroll = activeSlot;
            }
        }


        private void ScrollInventory()
        {
            int previous = activeSlot;
            int direction = 0;

            slotScroll += (scrollItemAction.ReadValue<float>() * 10);
            if (Mathf.Floor(slotScroll) > activeSlot)
            {
                activeSlot++;
                direction = 1;
            }
            else if (Mathf.Ceil(slotScroll) < activeSlot)
            {
                activeSlot--;
                direction = -1;
            }
            SetInventorySlot(previous, direction);
        }


        private void IncrementActiveSlot(InputAction.CallbackContext context)
        {
            int previous = activeSlot;
            activeSlot++;
            SetInventorySlot(previous, +1);
        }


        private void DecrementActiveSlot(InputAction.CallbackContext context)
        {
            int previous = activeSlot;
            activeSlot--;
            SetInventorySlot(previous, -1);
        }


        public bool PickupItem(ItemPickup pickup, bool always)
        {
            switch(pickup.Type)
            {
                case PickupType.Hotbar: 
                    return AddToInventory(pickup);
                case PickupType.Ammo:
                    return AddToAmmo(pickup);
                case PickupType.Armor:
                    return AddToArmor(pickup, always);
                default: return false;
            }
        }


        public bool AddToInventory(ItemPickup pickup)
        {
            Item item = allItems[pickup.ItemID];
            if (item is Sword)
            {
                Sword oldSword = (Sword)inventory[item.preferredSlot];
                Sword newSword = (Sword)item;
                if (newSword.Damage > oldSword.Damage)
                {
                    inventory[item.preferredSlot] = newSword;
                    oldSword.equiped = false;
                    newSword.BeAcquired();
                    Sword.SetSword(pickup.ItemID);
                    if (activeSlot == item.preferredSlot)
                    {
                        oldSword.OnPlayerDeselect(this);
                        newSword.OnPlayerSelect(this);
                    }
                    return true;
                }
                return false;
            }
            else if (item is Gun)
            {
                if (inventory[item.preferredSlot].equiped
                    && ammo[((Gun)item).AmmoTypeID].Full())
                {
                    return false;
                }
                inventory[item.preferredSlot].BeAcquired();
                ammo[((Gun)item).AmmoTypeID]
                    .Add(pickup.AmmoAmount, ammoText[((Gun)item).AmmoTypeID]);
                return true;
            }
            else if (item is Wand)
            {
                Wand wand = (Wand)item;
                if (wand.ShouldTake())
                {
                    inventory[item.preferredSlot].BeAcquired();
                    return true;
                }
                return true;
            }
            else if (item is ItemStack)
            {
                ItemStack stack = (ItemStack)item;
                if (stack.CanAddItem())
                {
                    stack.BeAcquired();
                    return true;
                }
                return false;
            }
            else
            {
                inventory[item.preferredSlot].BeAcquired();
                return true;
            }
        }


        public bool AddToAmmo(ItemPickup pickup)
        {
            Gun weapon = (Gun)allItems[pickup.ItemID];
            int slot = weapon.AmmoTypeID;
            if(!ammo[slot].Full())
            {
                ammo[slot].Add(pickup.AmmoAmount, ammoText[slot]);
                return true;
            }
            return false;
        }


        public bool AddToArmor(ItemPickup pickup, bool always)
        {
            int slot = pickup.ItemID;
            Armor armor = (Armor)armors[slot];
            bool output = always || armor.ShouldTake(armors[activeArmor]);
            if (output)
            {
                armors[activeArmor].BeRemoved();
                activeArmor = slot;
                armor.BeAcquired();
            }
            return output;
        }

        private void SetSword(int itemId)
        {
            Item item = allItems[itemId];
            Sword newSword = (Sword)item;
            inventory[item.preferredSlot] = newSword;
            if (Sword.Held > 0)
            {
                startingSword.SetActive(false);
            }
            newSword.BeAcquired();
            newSword.OnPlayerSelect(this);
        }

        #endregion



    }

}