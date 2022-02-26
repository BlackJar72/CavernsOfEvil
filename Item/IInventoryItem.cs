using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public interface IInventoryItem : IUsable
    {
        public void BeAcquired();

    }
}