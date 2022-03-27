using UnityEngine;
using RootMotion.FinalIK;


namespace CevarnsOfEvil
{
    public class BoomMob : EntityRangedNavMeshUser
    {
        private AimIK aimik;

        public override void Start()
        {
            base.Start();
            SetFactorSpeed(0);
            aimik = GetComponent<AimIK>();
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if (Random.value < 0.2)
            {
                entitySounds.PlayHurt(voice, 0);
                anim.SetTrigger("Pain");
                nextAttack += 0.625f;
            }
            return base.TakeDamage(ref damage);
        }


        public override void Die(Damages damages)
        {
            entitySounds.PlayDeath(voice, 0);
            base.Die(damages);
            GetComponent<EntityDeath>().ForceImmediate();
        }


        public override void MeleeAttack()
        {
            base.MeleeAttack();
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", 2);
            entitySounds.PlayAttack(voice, 1);
        }


        public override void RangedAttack()
        {
            base.RangedAttack();
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", 1);
            entitySounds.PlayAttack(voice, 0);
        }


        public override void SetTarget(Entity enemy)
        {
            base.SetTarget(enemy);
            aimik.solver.target = enemy.transform;
            aimik.enabled = true;
        }


        protected override void SetTarget(GameObject enemy)
        {
            base.SetTarget(enemy);
            aimik.solver.target = enemy.transform;
            aimik.enabled = true;
        }


        public override void RemoveTarget()
        {
            aimik.solver.target = null;
            aimik.enabled = false;
            base.RemoveTarget();
        }
    }

}