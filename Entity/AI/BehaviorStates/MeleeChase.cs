using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Chase", fileName = "MeleeChase", order = 1)]
    public class MeleeChase : BehaviorObject
    {
        public BehaviorObject clearAttackPath;

        public override bool StateUpdate(EntityMob ownerIn)
        {
            if (IsValidState(ownerIn))
            {
                ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.gameObject.transform.position);
                if((clearAttackPath != null) && clearAttackPath.IsValidState(ownerIn)) 
                    ownerIn.CurrentBehavior = clearAttackPath;
                return true;
            }
            ownerIn.RoutingAgent.isStopped = true;
            return false;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && (ownerIn.targetObject != null) 
                && ownerIn.targetObject.activeInHierarchy;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            //ownerIn.RoutingAgent.isStopped = false;
            ownerIn.EnableNavmesh();
        }
    }


}