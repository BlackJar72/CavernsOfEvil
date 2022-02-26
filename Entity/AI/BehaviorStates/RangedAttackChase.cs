using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/AI/Ranged Chase", fileName = "RangedChase", order = 2)]
    public class RangedAttackChase : BehaviorObject
    {
        [SerializeField] Attack attackState;

        public override bool StateUpdate(EntityMob ownerIn)
        {
            EntityRangedMob owner = ownerIn as EntityRangedMob;
            if (IsValidState(owner))
            {
                owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, 
                    Quaternion.LookRotation(owner.targetObject.transform.position - owner.transform.position, 
                        owner.transform.up), Time.deltaTime);
                ownerIn.SetDestinationAndUpdate(ownerIn.targetObject.gameObject.transform.position);
                if (owner.CanSeeTarget() && (owner.NextAttack < Time.time))
                {
                    owner.CurrentBehavior = attackState;
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


        public override void StateEnter(EntityMob ownerIn)
        {
            EntityRangedMob owner = (ownerIn as EntityRangedMob);
            ownerIn.Anim.SetInteger("AnimID", AnimID);
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            ownerIn.RoutingAgent.isStopped = false;
        }
    }

}