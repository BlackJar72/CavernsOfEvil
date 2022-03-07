using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/AI/Zombie Charge", fileName = "ZombieCharge", order = 21)]
    public class ZombieCharge : BehaviorObject
    {
        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && (ownerIn.targetObject != null)
                && ownerIn.targetObject.activeInHierarchy;
        }

        public override IBehaviorState NextState(EntityMob ownerIn)
            => EmptyState.Instance.NextState(ownerIn);

        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool output = IsValidState(ownerIn);
            if (output)
            {
                Zombie zombie = ownerIn as Zombie;
                StepDataAI nextStep = ownerIn.Manager.GetAIDataForGround(ownerIn.transform.position,
                    ownerIn.transform.position + ownerIn.DesiredDirection, zombie);
                if (ownerIn.DistanceSqrToTarget() < 16f)
                {
                    zombie.Destination = zombie.targetObject.transform.position;
                    zombie.TurnToDestination();
                }
                else if (zombie.ShouldTurn || !nextStep.Desireable ||
                    (zombie.WanderUpdateTime < Time.time))
                {
                    zombie.Destination = zombie.targetObject.transform.position;
                    zombie.TurnToDestination();
                    zombie.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                    zombie.DesiredDirection = FindTurnDirection() * zombie.DesiredDirection;
                    zombie.ShouldTurn = false;
                }
                zombie.FaceHeading();
            }
            return true;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
            Zombie zombie = (Zombie)ownerIn;
            if (ownerIn.DesiredDirection == Vector3.zero)
            {
                ownerIn.DesiredDirection = ownerIn.transform.forward;
            }
            zombie.WanderUpdateTime = Time.time + 0.5f + (Random.value * 0.5f);
            ownerIn.AnimSpeed = zombie.PrefferedSpeed;
            zombie.Anim.SetFloat("SpeedFactor", zombie.AnimSpeed);
        }



        //public override void StateFixedUpdate(EntityMob ownerIn) { }
        // public override void StateLateUpdate(EntityMob ownerIn) { }


        private Quaternion FindTurnDirection()
        {
            switch (Random.Range(0, 4))
            {
                case 0:
                    return Quaternion.identity;
                case 1:
                    return Quaternion.Euler(0, -30, 0);
                case 2:
                    return Quaternion.Euler(0, 30, 0);
                case 3:
                default:
                    return Quaternion.Euler(0, Random.Range(-45f, 45f), 0);
            }
        }

    }

}