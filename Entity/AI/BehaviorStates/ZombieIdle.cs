using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/AI/Zombie Idle", fileName = "ZombieIdle", order = 22)]
    public class ZombieIdle : BehaviorObject
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
            return true;
        }

        public override void StateEnter(EntityMob ownerIn)
        {
            Zombie zombie = (Zombie)ownerIn;
            ownerIn.DesiredDirection = Vector3.zero;
            zombie.Anim.SetFloat("SpeedFactor", ownerIn.AnimSpeed = 0);
        }
    }

}