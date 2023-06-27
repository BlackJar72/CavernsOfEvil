using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public interface IInventoryItem : IUsable
    {
        public void BeAcquired();

    }
}