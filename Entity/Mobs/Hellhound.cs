using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;


namespace CevarnsOfEvil {

    public class Hellhound : EntityRangedNavMeshUser {
        [SerializeField] GameObject fireJet;
        [SerializeField] GameObject firePrefab;

        private AimIK aimik;
        private bool aimikon;


        public override void Start()
        {
            base.Start();
            SetFactorSpeed(0);
            aimik = GetComponent<AimIK>();
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if((damage.type == DamageType.fire) || (damage.attacker == this)) return false;
            if (Random.value < 0.2)
            {
                entitySounds.PlayHurt(voice, 0);
                anim.SetTrigger("Pain");
                fireJet.SetActive(false);
                nextAttack += 0.625f;
            }
            return base.TakeDamage(ref damage);
        }


        public override void Die(Damages damages)
        {
            entitySounds.PlayDeath(voice, 0);
            base.Die(damages);
        }


        public void FireOn() {
            fireJet.SetActive(true);
            aimikon = aimik.enabled;
            aimik.enabled = false;
            StartCoroutine(BurningDelayed());
        }


        public void FireOff() {
            fireJet.SetActive(false);
            aimik.enabled = aimikon;
        }


        public void PlaceFire() {
            Vector3 firepos = new Vector3(voice.transform.position.x, transform.position.y, voice.transform.position.z);
            Instantiate(firePrefab, firepos,
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0));
        }


        public void StartPlaceFire() {
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", 4);
            entitySounds.PlayAttack(voice, 1);
        }


        public override void MeleeAttack()
        {
            int attack = Random.Range(1, 5);
            SetFactorSpeed(0);
            anim.SetInteger("AnimID", attack);
            entitySounds.PlayAttack(voice, 0);
            if(attack < 4) base.MeleeAttack();
        }


        public override void RangedAttack()
        {
            if(Random.Range(0, 2) < 1) {
                SetFactorSpeed(0);
                anim.SetInteger("AnimID", 4);
                entitySounds.PlayAttack(voice, 0);
            } else {
                base.RangedAttack();
                SetFactorSpeed(0);
                anim.SetInteger("AnimID", 1);
                entitySounds.PlayAttack(voice, 0);
            }
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
            aimikon = aimik.enabled = false;
            base.RemoveTarget();
        }


        public override void FireProjectile()
        {
            if ((targetEntity != null) && (targetEntity.enabled))
            {
                NextFireTime = Time.time + FireDelay;
                AimParams aim;
                GetAimParams(out aim);
                GameObject proj = Instantiate(projectile, aim.from, ProjSpawn.rotation);
                proj.GetComponent<SimpleProjectile>().LaunchSimple(aim.toward, this);
            }
        }


        public virtual IEnumerator BurningDelayed()
        {
            yield return new WaitForSeconds(0.75f);
            FireOff();
            PlaceFire();
        }
    }

}