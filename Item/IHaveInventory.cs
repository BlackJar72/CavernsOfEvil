using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public interface IHaveInventory
    {
        public bool AddToInventory(ItemPickup pickup);
    }

}