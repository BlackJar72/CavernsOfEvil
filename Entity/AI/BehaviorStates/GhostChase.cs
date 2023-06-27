using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/AI/Spectre Chase", fileName = "SpectreChase", order = 22)]
    public class GhostChase : BehaviorObject
    {
        
        public override bool StateUpdate(EntityMob entityMob)
        {
            return true;
        }


        public override bool IsValidState(EntityMob ownerIn) 
        {
            return true;
        }


        public override void StateEnter(EntityMob entityMob) {}
    }

}