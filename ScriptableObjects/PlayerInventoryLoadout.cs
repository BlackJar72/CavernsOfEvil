using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/Player Loadout", fileName = "PlayerInvetory", order = 1)]
    public class PlayerInventoryLoadout : ScriptableObject
    {
        [SerializeField] Item[] inventory = new Item[9];
        [SerializeField] int activeSlot = 0;
        [SerializeField] Item[] otherItems;

        [SerializeField] AmmoType[] ammoTypes = new AmmoType[4];
        private AmmoData[] ammo = new AmmoData[4];
        [SerializeField] TMP_Text[] ammoText = new TMP_Text[5];

        [SerializeField] Armor[] armors = new Armor[5];
        private int activeArmor = 0;


        // Start is called before the first frame update
        public void Fuck(PlayerAct script)
        {
            for(int i = 0; i < inventory.Length; i++)
            {
                inventory[i] = script.inventory[i];
            }

            otherItems = new Item[script.otherItems.Length];
            for (int i = 0; i < otherItems.Length; i++)
            {
                otherItems[i] = script.otherItems[i];
            }

            for (int i = 0; i < ammoTypes.Length; i++)
            {
                ammoTypes[i] = script.ammoTypes[i];
            }

            for (int i = 0; i < ammoText.Length; i++)
            {
                ammoText[i] = script.ammoText[i];
            }

            for (int i = 0; i < armors.Length; i++)
            {
                armors[i] = script.armors[i];
            }
        }
    }

}