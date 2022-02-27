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
                SpawnProjectile(aim, player.PlayerScript);
                anim.SetTrigger("Act");
                fireSound.Play();
                AlertListeningMobs(player.PlayerScript);
            }
        }


        public void SpawnProjectile(AimParams aim, Entity attacker)
        {
            GameObject proj = Instantiate(projectile, aim.from, aimTransform.rotation);
            proj.GetComponent<SimpleProjectile>().LaunchSimple(aim.toward, attacker);
        }
    }

}