using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/AI/Archer Attack", fileName = "ArcherAttack", order = 10)]
    public class ArcherAttack : BehaviorObject
    {
        [SerializeField] ArcherManeuver maneuverState;
        [SerializeField] ArcherChase chaseState;


        public override bool IsValidState(EntityMob ownerIn)
        {
            IArcher archer = ownerIn as IArcher;
            // Not valid is no line of sight, but don't abandon mid-shot
            return !archer.ReadyToShoot; // TODO: Maneuver if target is too close!
        }


        public override bool StateUpdate(EntityMob ownerIn)
        {
            bool valid = IsValidState(ownerIn);
            if (!valid)
            {
                ownerIn.CurrentBehavior = chaseState; // TODO: Go to maneuver if can see target.
            }
            else
            {
                Vector3 toTarget = ownerIn.targetObject.transform.position - ownerIn.transform.position;
                ownerIn.transform.rotation = Quaternion.Lerp(ownerIn.transform.rotation,
                    Quaternion.LookRotation(toTarget), Time.deltaTime * 15f);
            }
            return valid;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
            ownerIn.Anim.SetInteger("AnimID", AnimID);
            ownerIn.SetFactorSpeed(AnimMoveSpeed);
            IArcher archer = ownerIn as IArcher;
            archer.ArrowAttack();
        }

    }

}