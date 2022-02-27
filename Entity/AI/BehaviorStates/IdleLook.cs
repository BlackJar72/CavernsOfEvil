using UnityEngine;


namespace CevarnsOfEvil {

    [CreateAssetMenu(menuName ="DLD/AI/Idle Look", fileName = "IdleLook", order = 0)]
    public class IdleLook : BehaviorObject
    {
        [SerializeField] BehaviorObject onSeePlayer;

        public override bool StateUpdate(EntityMob ownerIn)
        {
            if (ownerIn.LookForPlayer() || (ownerIn.targetEntity != null))
            {
                ownerIn.CurrentBehavior = onSeePlayer;
            } 
            else if((ownerIn.NextIdleTalk < Time.time) && (Random.value < (0.5 * Time.deltaTime)) 
                && (ownerIn.DistanceSqrToPlayer() < 4096))
            {
                ownerIn.Sounds.PlayIdle(ownerIn.Voice);
                ownerIn.NextIdleTalk += 2 + (Random.value * 3);
            }
            return true;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return true;
        }


        public override void StateEnter(EntityMob ownerIn)
        {
        
        }


        public override void StateExit(EntityMob ownerIn)
        {
            ownerIn.Sounds.PlayAggro(ownerIn.Voice, 0);
        }
    }

}