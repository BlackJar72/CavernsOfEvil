using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class SolarGauntlet : Gun
    {
        public float rateOfFire = 12.5f;
        private float fireTime;

        public GameObject projectile;
        public AudioSource fireSound;
        public GameObject hitParticles;


        public void Start()
        {
            Init();
            fireTime = Time.time;
        }

        public override void OnMobUse(Entity mob)
        {
            throw new System.NotImplementedException();
        }

        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if ((Time.time > fireTime) && player.UseAmmo(ammoTypeID))
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                AimParams aim;
                player.GetAimParams(out aim);
                aim.from = aimTransform.position;
                FirePlasmaPlayer(aim, player.PlayerScript);
                //SpawnProjectile(aim, player.PlayerScript);
                anim.SetTrigger("Act");
                fireSound.Play();
                AlertListeningMobs(player.PlayerScript);
            }
        }


        public void SpawnProjectile(AimParams aim, Entity attacker, RaycastHit target, EntityHealth victim)
        {
            GameObject proj = Instantiate(projectile, aim.from, aimTransform.rotation);
            proj.GetComponent<HomingProjectileFX>().Launch(aim.toward, attacker, target, victim);
        }


        protected void FirePlasmaPlayer(AimParams aim, Entity attacker)
        {
            RaycastHit target;
            GameObject hit;
            if (Physics.Raycast(aim.from, aim.toward, out target, 256, GameConstants.PlayerAttackMask))
            {
                if (target.collider != null)
                {
                    hit = target.collider.gameObject;
                    EntityHealth victim = hit.GetComponent<EntityHealth>();
                    Instantiate(hitParticles, target.point, Quaternion.FromToRotation(Vector3.forward, target.normal));
                    SpawnProjectile(aim, attacker, target, victim);
                }
            }
        }
    }

}