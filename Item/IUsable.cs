using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public interface IUsable
    {
        public void OnPlayerUse(PlayerAct player, Animator anim);
        public void OnMobUse(Entity mob);
    }
}