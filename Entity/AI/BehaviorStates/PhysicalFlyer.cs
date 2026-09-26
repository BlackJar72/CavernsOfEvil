using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/AI/Physically Flying Chase", fileName = "FlyingChase", order = 31)]
    public class PhysicalFlyer : BehaviorObject 
    {
        [SerializeField] Attack attackState;

        public override bool StateUpdate(EntityMob entityMob)
        {
            PhysicalMob owner = entityMob as PhysicalMob;
            if (IsValidState(owner))
            {
                owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, 
                    Quaternion.LookRotation(owner.targetObject.transform.position - owner.transform.position, 
                        owner.transform.up), Time.deltaTime);
                if ((owner != null) && owner.CanSeeTarget() && (owner.NextAttack < Time.time))
                {
                    owner.CurrentBehavior = attackState;
                } 
                else
                {
                    owner.GetNewDirection();
                }
                return true;
            }
            else
            {
                owner.RemoveTarget();
                return false;
            }
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && !ownerIn.targetEntity.IsDead
                && (ownerIn.targetObject != null)
                && ownerIn.targetObject.activeInHierarchy;
        }


        public override void StateEnter(EntityMob entityMob)
        {
            PhysicalMob owner = entityMob as PhysicalMob;
            owner.Anim.SetInteger("AnimID", AnimID);
        }
    
        
    }


}