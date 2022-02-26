using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class Pistol : Gun
    {
        public float rateOfFire = 2.0f;
        private float fireTime;

        public ParticleSystem muzzleFlash;
        public AudioSource bang;


        public void Start()
        {
            Init();
            fireTime = Time.time;
        }


        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if ((Time.time > fireTime) && player.UseAmmo(ammoTypeID))
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                AimParams aim;
                player.GetAimParams(out aim);
                FireBulletPlayer(aim.from, aim.toward, player.PlayerScript);
                anim.SetTrigger("Act"); 
                StartCoroutine(FlashLight());
                muzzleFlash.Play();
                bang.Play();
                AlertListeningMobs(player.PlayerScript);
            }
        }


        public override void OnMobUse(Entity mob)
        {
            if (Time.time > fireTime)
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                AimParams aim;
                mob.GetAimParams(out aim);
                FireBulletPlayer(aim.from, aim.toward, mob);
                StartCoroutine(FlashLight());
                muzzleFlash.Play();
                bang.Play();
            }
        }
    }

}