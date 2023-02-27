using UnityEngine;
using System.Collections;


namespace CevarnsOfEvil {

    public class EntitySpectre : EntityMob {

        Vector3 heading;

        public override void Start()
        {
            player = GameObject.Find("FemalePlayer");
            if(CanSeeCollider(player)) Destroy(gameObject);
            else {
                anim = GetComponent<Animator>();
                aggroRangeSq = aggroRange * aggroRange;
                CurrentBehavior = EmptyState.Instance.NextState(this);
                enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
                setAnimByVelocity = new SetAnimSpeed(SetAnimSpeedVelocity);
                setAnimToZero = new SetAnimSpeed(SetAnimSpeedZero);
                wandering = fleeing = false;
            }
        }


        public override void Update() {
            if(!currentBehavior.StateUpdate(this)) FindNewBehavior();
            if(targetEntity) Move();
        }


        // Yes, ghosts can move through walls
        private void Move() {
            Vector3 toTarget = targetEntity.transform.position - transform.position;
            if(heading.sqrMagnitude == 0) heading = toTarget.normalized * BaseMoveSpeed;
            else {
                heading = heading.normalized + (toTarget.normalized * 5f * Time.deltaTime);
                heading = heading.normalized * baseMoveSpeed;
            }
            if(toTarget.sqrMagnitude > 1) transform.position += (heading * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(heading * Time.deltaTime, Vector3.up);
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                anim.SetInteger("Variant", Random.Range(0, 3));
                anim.SetTrigger("Attack");
                entitySounds.PlayAttack(voice, 0);

                EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
                if (victim != null)
                {
                    victim.BeHitByAttack(meleeDamage, this);
                }
            }
        }


        public override bool TakeDamage(ref Damages damage)
        {
            if(damage.type == DamageType.physical) return false;
            entitySounds.PlayHurt(voice, 0);
            bool output = base.TakeDamage(ref damage);
            return output;
        }


        public override void Die(Damages damage) {
            base.Die(damage);
            StartCoroutine(Vanish());
        }


        IEnumerator Vanish()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }


        public override void GetAimParams(out AimParams aim) {
            throw new System.NotImplementedException();
        }
    }

}
