using UnityEngine;


namespace CevarnsOfEvil
{

    [CreateAssetMenu(menuName = "DLD/AI/Melee Chase", fileName = "MeleeChase", order = 1)]
    public class MeleeChase : BehaviorObject
    {
        public BehaviorObject clearAttackPath;

        public override bool StateUpdate(EntityMob ownerIn)
        {
            if (IsValidState(ownerIn))
            {
                if((clearAttackPath != null) && clearAttackPath.IsValidState(ownerIn)) 
                    ownerIn.CurrentBehavior = clearAttackPath;
                return true;
            }
            return false;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return (ownerIn.targetEntity != null)
                && ownerIn.targetEntity.enabled
                && (ownerIn.targetObject != null) 
                && ownerIn.targetObject.activeInHierarchy;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
        }
    }


}