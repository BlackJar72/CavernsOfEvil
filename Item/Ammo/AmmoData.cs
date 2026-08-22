using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace CevarnsOfEvil
{

    [System.Serializable]
    public class AmmoData
    {
        [SerializeField] AmmoType type;
        [SerializeField] int amount;

        public int Amount { get { return amount; } set { amount = Mathf.Clamp(value, 0, type.Max); } }
        public AmmoType Type { get { return type; } }


        public static int[] ToIntArray(AmmoData[] ammoData)
        {
            int[] result = new int[ammoData.Length];
            for(int i = 0; i < ammoData.Length; i++)
            {
                result[i] = ammoData[i].amount;
            }
            return result;
        }


        public static AmmoData[] FromIntArray(AmmoData[] ammoData, int[] amounts)
        {
            for(int i = 0; i < ammoData.Length; i++) ammoData[i].amount = amounts[i];
            return ammoData;
        }


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


        public void Fill(TMP_Text text) {
            amount = type.Max;
            text.text = amount.ToString();
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