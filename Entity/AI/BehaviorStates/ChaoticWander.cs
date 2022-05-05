using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{


    [CreateAssetMenu(menuName = "DLD/AI/Chaotic Wander", fileName = "ChaoticWander", order = 20)]
    public class ChaoticWander : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;
        [SerializeField] float vocalRate = 1.0f;

        public override bool IsValidState(EntityMob ownerIn) => true;

        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {
            if (ownerIn.LookForPlayer() || (ownerIn.targetEntity != null))
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            }
            else if ((ownerIn.NextIdleTalk < Time.time) && (Random.value < (Time.deltaTime * vocalRate)))
            {
                ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                ownerIn.NextIdleTalk += (2 / vocalRate) + (Random.value * 3);
            }
            ChaoticFlyer flyer = ownerIn as ChaoticFlyer;
            if(flyer.ShouldTurn || (flyer.WanderUpdateTime < Time.time))
            {
                flyer.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                flyer.DesiredDirection = AIHelper.GetTurnDirection3d(flyer.DesiredDirection);
                flyer.ShouldTurn = false;
            }
            flyer.FaceHeading();
            return true;
        }

        public override void StateEnter(EntityMob ownerIn) 
        { 
            (ownerIn as ChaoticFlyer).DesiredDirection = ownerIn.transform.forward;
        }

    }

}