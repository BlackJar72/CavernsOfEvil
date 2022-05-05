using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil {

    [CreateAssetMenu(menuName = "DLD/AI/Chaotic Flight Charge", fileName = "ChaoticCharge", order = 21)]
    public class ChaoticFlightCharge : BehaviorObject
    {
        [SerializeField] Attack attackState;
        [SerializeField] float vocalRate = 1.0f;


        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);



        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool output = IsValidState(ownerIn);
            if (output)
            {
                ChaoticRanged flyer = ownerIn as ChaoticRanged;
                if (flyer.CanSeeTarget())
                {
                    if(flyer.NextAttack < Time.time) 
                    {
                        flyer.CurrentBehavior = attackState;
                    }
                    SetHeadingTowardTarget(flyer);
                }
                else if(flyer.ShouldTurn || (flyer.WanderUpdateTime < Time.time))
                {
                    flyer.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                    if(Random.value > 0.75f) 
                    {
                        SetHeadingTowardTarget(flyer);
                    }
                    else
                    {
                        flyer.DesiredDirection = AIHelper.GetTurnDirection3d(flyer.DesiredDirection);
                        flyer.ShouldTurn = false;
                    }
                }
                flyer.FaceHeading();
            }
            if ((ownerIn.NextIdleTalk < Time.time) && (Random.value < (Time.deltaTime * vocalRate)))
            {
                ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                ownerIn.NextIdleTalk += (2 / vocalRate) + (Random.value * 3);
            }
            return true;
        }


        private void SetHeadingTowardTarget(ChaoticFlyer flyer) 
        {
            if (flyer.DistanceSqrToTarget() < 16f)
            {
                flyer.Destination = flyer.targetObject.transform.position;
                flyer.TurnToDestination();
            }
            else if (flyer.ShouldTurn || 
                (flyer.WanderUpdateTime < Time.time))
            {
                flyer.Destination = flyer.targetObject.transform.position;
                flyer.TurnToDestination();
                flyer.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                flyer.DesiredDirection = AIHelper.GetSkewedDirection3d(flyer.DesiredDirection);
                flyer.ShouldTurn = false;
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
            ownerIn.Anim.SetInteger("AnimID", AnimID);
            SetHeadingTowardTarget(ownerIn as ChaoticFlyer);
        }
    }

}
