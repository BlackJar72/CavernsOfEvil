using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace DLD
{

    public class AmmoData
    {
        AmmoType type;
        int amount;

        public int Amount { get { return amount; } set { amount = Mathf.Clamp(value, 0, type.Max); } }
        public AmmoType Type { get { return type; } }


        public AmmoData(AmmoType type)
        {
            this.type = type;
            amount = type.StartAmount;
        }


        public bool Use(TMP_Text text)
        {
            if (amount == 0)
            {
                return false;
            }
            else
            {
                amount--;
                text.text = amount.ToString();
                return true;
            }
        }


        public bool Full()
        {
            return amount >= type.Max;
        }


        public void Add(int quantity, TMP_Text text)
        {
            amount = Mathf.Clamp(amount + quantity, 0, type.Max);
            text.text = amount.ToString();
        }


        public void Init(AmmoType type)
        {
            this.type = type;
            amount = type.StartAmount;
        }
    }


}