using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public abstract class EntityRangedMob : EntityMob
    {
        [SerializeField] protected GameObject projectile;
        [SerializeField] protected Transform projectileSpawn;
        [SerializeField] protected float inaccuracy;
        [SerializeField] float rateOfFire;

        protected float fireDelay;
        protected float nextFireTime;


        public float FireDelay { get { return fireDelay; } }
        public float NextFireTime { get { return nextFireTime; } set { nextFireTime = value; } }
        public Transform ProjSpawn { get { return projectileSpawn; } }


        public override void Start()
        {
            base.Start();
            fireDelay = 1.0f / rateOfFire;
            nextFireTime = Time.time;
        }



        public override void GetAimParams(out AimParams aim)
        {
            float magnitude, rotation, x, y;
            Quaternion scatter;
            aim.from = projectileSpawn.transform.position;
            aim.toward = (targetEntity.GetComponent<Collider>().bounds.center - projectileSpawn.position).normalized;
            magnitude = Random.Range(-inaccuracy, inaccuracy);
            rotation = Random.Range(0, 360);
            x = Mathf.Sin(rotation) * magnitude;
            y = Mathf.Cos(rotation) * magnitude;
            scatter = Quaternion.AngleAxis(x, projectileSpawn.right)
                    * Quaternion.AngleAxis(y, projectileSpawn.up);
            aim.toward = scatter * aim.toward;
        }


        public override void Attack()
        {
            nextAttack = Time.time + attackTime;
            if(InMeleeRange()) MeleeAttack();
            else if (nextFireTime < Time.time )
            {
                stasisAI = nextAttack;
                nextFireTime = stasisAI + fireDelay + ((1.0f  + fireDelay) * Random.value);
                RangedAttack();
            }
        }


        public virtual void RangedAttack() 
        {
            StartCoroutine(FireDelayed());
        }


        public virtual void FireProjectile()
        {
            if ((targetEntity != null) && (targetEntity.enabled))
            {
                NextFireTime = Time.time + FireDelay;
                AimParams aim;
                GetAimParams(out aim);
                GameObject proj = Instantiate(projectile, aim.from, ProjSpawn.rotation);
                proj.GetComponent<Projectile>().LaunchSimple(aim.toward, this);
            }
        }


        public virtual IEnumerator FireDelayed()
        {
            yield return new WaitForSeconds(FireDelay / 5.0f);
            FireProjectile();
        }
    }

}