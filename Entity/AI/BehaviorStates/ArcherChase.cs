using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/AI/Archer Chase", fileName = "ArcherChase", order = 11)]
    public class ArcherChase : BehaviorObject
    {
        [SerializeField] ArcherManeuver maneuverState;
        [SerializeField] ArcherAttack attackState;


        public override void StateEnter(EntityMob ownerIn)
        {
            ownerIn.Anim.SetInteger("AnimID", AnimID);
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return ownerIn.targetEntity != null;
        }


        public override bool StateUpdate(EntityMob ownerIn)
        {

            // TODO: Go to maneuver when close to and can see target!
            IArcher archer = ownerIn as IArcher;
            if (ownerIn.CanSeeTarget())
            {
                if (archer.ReadyToShoot && (ownerIn.NextAttack < Time.time))
                {
                    ownerIn.CurrentBehavior = attackState;
                }
            }
            else
            {

            }

            return IsValidState(ownerIn);
        }
    }

}