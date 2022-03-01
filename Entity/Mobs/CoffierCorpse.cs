using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class CoffierCorpse : EntityZombie
    {
        [SerializeField] Collider[] colliders;


        public override bool TakeDamage(ref Damages damage)
        {
            damage.wound = 0;
            return base.TakeDamage(ref damage);
        }


        public override void Die(Damages damages)
        {
            StartCoroutine(DeathTimer());
            base.Die(damages);
        }


        IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(Random.Range(6f, 10f));
            Resurrect();
        }


        public void Resurrect()
        {
            enabled = true;
            if(health.Health < 1)
            {
                enabled = false;
                return;
            }
            foreach(Collider collider in colliders)
            {
                collider.enabled = true;
            }
            health.Health -= 1;
            health.Shock = health.Health * 5;
            anim.SetBool("Dead", false);
            anim.SetTrigger("Revive");
            GetComponent<Rigidbody>().WakeUp(); 
            isDead = false;
            GetComponent<EntityDeath>().enabled = false;
            health.enabled = true;

        }

    }

}