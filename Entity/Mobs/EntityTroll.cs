using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class EntityTroll : EntityGoblinWarrior01 {

        public override bool TakeDamage(ref Damages damage)
        {
            entitySounds.PlayHurt(voice, 0);
            #if UNITY_EDITOR
            if(!(health is TrollHealth)) {
                Debug.Log("EntityTroll needs TrollHealth!!!");
                Application.Quit();
            }
            #endif
            if((damage.type == DamageType.fire) || (damage.type == DamageType.firePlus)) {
                ((TrollHealth)health).BeDamagedByFire(damage);
            } else {
                damage.shock = 0;
            }
            bool output = base.TakeDamage(ref damage);

            if (!fleeing && (targetEntity != null)
            && ((targetEntity == damage.attacker)
            && wandering
            && (Random.Range(0, health.Shock) < ((damage.shock + damage.wound) * 2))))
            {
                fleeing = true;
            }
            return output;
        }

    }

}