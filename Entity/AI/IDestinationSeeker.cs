using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public interface IDestinationSeeker 
    {

        public float NavmeshTimer { get; set; }

        // Accessor properties
        public bool CanReachDestination { get; }


        #region NavMesh Integration
        // NavMesh integration
        public bool CanReachDestinationBetter();
        public void SetNavmeshDestination(Vector3 destination);
        public void SetDestination(Vector3 destination);

        #region Randomizers
        public void SetRandomDestination(int range);
        public void SetRandomDestinationCurrent(int range);
        public void SetRandomDestinationTarget(int range);
        #endregion

        public void ClearNavmeshDestination();
        public void SetNavmeshDestination();
        public void EnableNavmesh();
        public void DisableNavmesh();
        public void UpdateNavmesh();
        public void SetDestinationAndUpdate(Vector3 destination);
        public void ForceNavmeshUpdate();
        public bool LineToTargetClear();
        #endregion
        
    }

}
