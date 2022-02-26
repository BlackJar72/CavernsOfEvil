using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public interface IHaveInventory
    {
        public bool AddToInventory(ItemPickup pickup);
    }

}