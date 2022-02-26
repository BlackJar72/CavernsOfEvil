using UnityEngine;


namespace DLD
{

    public class Shotgun : Gun
    {
        [SerializeField] float rateOfFire = 1.0f;
        [SerializeField] int amountOfShot = 8;
        private float fireTime;

        [SerializeField] ParticleSystem muzzleFlash;
        [SerializeField] AudioSource boom;

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
                player.GetAimParams(out AimParams aim);
                Shoot(aim, player.GetTransform(), player.PlayerScript);
                anim.SetTrigger("Act");
                StartCoroutine(FlashLight());
                muzzleFlash.Play();
                boom.Play();
                AlertListeningMobs(player.PlayerScript);
            }
        }


        public override void OnMobUse(Entity mob)
        {
            if (Time.time > fireTime)
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                mob.GetAimParams(out AimParams aim);
                Shoot(aim, mob.GetTransform(), mob);
                StartCoroutine(FlashLight());
                muzzleFlash.Play();
                boom.Play();
            }
        }


        private void Shoot(AimParams aim, Transform shooterTransform, Entity attacker)
        {
            float magnitude, rotation, x, y;
            Quaternion scatter;
            for (int i = 0; i < amountOfShot; i++)
            {
                magnitude = Random.Range(-5f, 5f);
                rotation = Random.Range(0, 360);
                x = Mathf.Sin(rotation) * magnitude;
                y = Mathf.Cos(rotation) * magnitude;
                scatter = Quaternion.AngleAxis(x, shooterTransform.right)
                        * Quaternion.AngleAxis(y, shooterTransform.up);
                FireBulletPlayer(aim.from, scatter * aim.toward, attacker);
            }
        }
    }

}