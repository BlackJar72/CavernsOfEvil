using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{


    [CreateAssetMenu(menuName = "DLD/AI/Zombie Wander", fileName = "ZombieWander", order = 20)]
    public class ZombieWander : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;

        public override bool IsValidState(EntityMob ownerIn) => true;

        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {
            if (ownerIn.LookForPlayer() || (ownerIn.targetEntity != null))
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            }
            EntityZombie zombie = ownerIn as EntityZombie;
            StepDataAI nextStep = ownerIn.Manager.GetAIDataForGround(ownerIn.transform.position, 
                ownerIn.transform.position + ownerIn.DesiredDirection, zombie);
            if(zombie.ShouldTurn || !nextStep.Desireable || (zombie.WanderUpdateTime < Time.time))
            {
                zombie.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                zombie.DesiredDirection = AIHelper.FindTurnDirection() * zombie.DesiredDirection;
                zombie.ShouldTurn = false;
            }
            zombie.FaceHeading();
            return true;
        }

        public override void StateEnter(EntityMob ownerIn) 
        { 
            EntityZombie zombie = (EntityZombie)ownerIn;
            if(ownerIn.DesiredDirection == Vector3.zero)
            {
                ownerIn.DesiredDirection = ownerIn.transform.forward;
                ownerIn.AnimSpeed = zombie.PrefferedSpeed;
                zombie.WanderUpdateTime = Time.time + 0.5f + (Random.value * 0.5f);
                zombie.Anim.SetFloat("SpeedFactor", zombie.AnimSpeed);
            }
        }



        //public override void StateFixedUpdate(EntityMob ownerIn) { }
        // public override void StateLateUpdate(EntityMob ownerIn) { }

    }

}