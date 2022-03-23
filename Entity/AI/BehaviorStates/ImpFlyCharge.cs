using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil

{
    [CreateAssetMenu(menuName = "DLD/AI/Imp Combat Fly", fileName = "ImpCombat", order = 21)]
    public class ImpFlyCharge : BehaviorObject
    {
        [SerializeField] BehaviorObject attackState;
        [SerializeField] BehaviorObject wanderState;


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
                WingedImp imp = ownerIn as WingedImp;
                GameManager manager = imp.Dungeon.Manager;
                bool targetSeen = imp.CanSeeTarget();
                StepDataAI nextStep = manager.GetAIDataForFlying(ownerIn.transform.position,
                    ownerIn.transform.position + imp.DesiredDirection, imp);
                if (ownerIn.DistanceSqrToTarget() < 16f)
                {
                    imp.Destination = imp.targetObject.transform.position;
                    imp.TurnToDestination();
                }
                else if (imp.ShouldTurn || !nextStep.Desireable ||
                    (imp.WanderUpdateTime < Time.time))
                {
                    imp.Destination = imp.targetObject.transform.position;
                    imp.TurnToDestinationFlying();
                    imp.WanderUpdateTime = Time.time + 2f + (Random.value * 2f);
                    imp.DesiredDirection = FindTurnDirection() * imp.DesiredDirection;
                    imp.ShouldTurn = false;
                }
                imp.FaceHeading();
                if (targetSeen && (imp.NextAttack < Time.time))
                {
                    imp.CurrentBehavior = attackState;
                }
                if(!targetSeen || (imp.ShouldTurn && (Random.value < 0.1f)))
                {
                    imp.CurrentBehavior = wanderState;
                }
                imp.Move();
            }
            return true;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
            WingedImp imp = ownerIn as WingedImp;
            imp.DesiredDirection = (ownerIn.targetObject.transform.position - ownerIn.transform.position).normalized;
            imp.WanderUpdateTime = Time.time + 0.5f + (Random.value * 0.5f);
            //imp.DisableNavmesh();
        }



        // public override void StateFixedUpdate(EntityMob ownerIn) { }
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