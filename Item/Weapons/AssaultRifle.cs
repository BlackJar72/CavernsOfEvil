using UnityEngine;


namespace DLD
{

    public class AssaultRifle : Gun
    {
        public float rateOfFire = 10.0f;
        private float fireTime;

        public GameObject fireEffects;


        public void Start()
        {
            Init();
            fireTime = Time.time;
        }


        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if (Time.time > fireTime)
            {
                if (player.UseAmmo(ammoTypeID))
                {
                    fireTime = Time.time + (1.0f / rateOfFire);
                    AimParams aim;
                    player.GetAimParams(out aim);
                    FireBulletPlayer(aim.from, aim.toward, player.PlayerScript);
                    anim.SetTrigger("Act");
                    anim.SetInteger("Action", 1);
                    muzzleLight.gameObject.SetActive(true);
                    fireEffects.SetActive(true);
                    AlertListeningMobs(player.PlayerScript);
                }
                else
                {
                    muzzleLight.gameObject.SetActive(false);
                    fireEffects.SetActive(false);
                }
            }
        }

        public override void OnPlayerEndUse()
        {
            muzzleLight.gameObject.SetActive(false);
            fireEffects.SetActive(false);
        }


        public override void OnPlayerSelect(PlayerAct player)
        {
            base.OnPlayerSelect(player);
            // Do this to fix the glitch were the gun would somtimes appear 
            // to be firing (though not actually doing so) when first selected.
            OnPlayerEndUse();
        }


        public override void OnMobUse(Entity mob)
        {
            if (Time.time > fireTime)
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                AimParams aim;
                mob.GetAimParams(out aim);
                FireBulletPlayer(aim.from, aim.toward, mob);
            }
        }
    }

}