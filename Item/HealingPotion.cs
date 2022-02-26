using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public class HealingPotion : StackableItem
    {
        [SerializeField] int amountHealed;

        public override void OnMobUse(Entity mob)
        {
            mob.TakeHealingPoition(amountHealed);
        }

        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            player.TakeHealingPotion(amountHealed);
        }
    }

}