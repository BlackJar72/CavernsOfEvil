using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

namespace CevarnsOfEvil
{
    public interface IWorldSensorUser
    {
        public void OnWorldSensorTriggered(Collider other);
        public void OnWorldSensorExit(Collider other);

        public Vector3 Destination { get; }
    }


}
