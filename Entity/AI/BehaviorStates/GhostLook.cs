using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/AI/Spectre Look", fileName = "SpectreLook", order = 21)]
    public class GhostLook : BehaviorObject {
        [SerializeField] BehaviorObject onSeePlayer;

        public override bool StateUpdate(EntityMob entity)
        {
            if (entity.LookForPlayer() || (entity.targetEntity != null))
            {
                entity.CurrentBehavior = onSeePlayer;
            }
            return true;
        }


        public override bool IsValidState(EntityMob ownerIn)
        {
            return true;
        }


        public override void StateEnter(EntityMob entityMob) {}


        public override void StateExit(EntityMob ownerIn) {}
    }

}