using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AmmoType", fileName = "AmmoType", order = 202)]
    public class AmmoType : ScriptableObject
    {
        [SerializeField] int id;
        [SerializeField] int max;
        [SerializeField] int startAmount;

        private int amount;

        public int Id { get { return id; } }
        public int Max { get { return max; } }
        public int StartAmount { get { return startAmount; } }
        public int Amount { get { return amount; } set { amount = Mathf.Clamp(value, 0, max); } }


        public bool Use()
        {
            if(amount == 0)
            {
                return false;
            }
            else
            {
                amount--;
                return true;
            }
        }


        public void Add(int quantity)
        {
            amount = Mathf.Clamp(amount + quantity, 0, max);
        }


        public void Init()
        {
            amount = startAmount;
        }
    }
}