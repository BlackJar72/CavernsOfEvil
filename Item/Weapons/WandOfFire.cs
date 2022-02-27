using UnityEngine;
using RootMotion.FinalIK;


namespace CevarnsOfEvil
{

    public class WandOfFire : Wand
    {
        [SerializeField] float rateOfFire = 1.0f;
        private float fireTime;

        [SerializeField] GameObject projectile;
        [SerializeField] AudioSource launchSound;
        [SerializeField] Transform lauchSpawn;

        private static int charges;
        public override int Charges { get { return charges; } set { charges = value; } }


        // Start is called before the first frame update
        void Start()
        {
            Init();
            fireTime = Time.time;
        }


        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if ((Charges > 0) && (Time.time > fireTime))
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                Charges--;
                AimParams aim;
                player.GetAimParams(out aim);
                aim.from = lauchSpawn.position;
                FireProjectile(aim.from, aim.toward, player.PlayerScript);
                anim.SetTrigger("Act");
                launchSound.Play();
                AlertListeningMobs(player.PlayerScript);
                chargeScaler.SetBar(Charges, maxCharges);
                if (Charges < 1)
                {
                    BeRemoved();
                    anim.SetInteger("Item", -1);
                    anim.SetTrigger("Swap");
                }
            }
        }


        public override void OnMobUse(Entity mob)
        {
            if (Time.time > fireTime)
            {
                fireTime = Time.time + (1.0f / rateOfFire);
                AimParams aim;
                mob.GetAimParams(out aim);
                aim.from = lauchSpawn.position;
                FireProjectile(aim.from, aim.toward, mob);
                launchSound.Play();
            }
        }


        protected void FireProjectile(Vector3 from, Vector3 toward, Entity attacker)
        {
            GameObject proj = Instantiate(projectile, from, aimTransform.rotation);
            proj.GetComponent<Projectile>().LaunchSimple(toward, attacker);
        }


        public override void OnPlayerSelect(PlayerAct player)
        {
            if (aimTransform != null)
            {
                AimIK aimIk = player.gameObject.GetComponent<AimIK>();
                aimIk.solver.transform = aimTransform;
            }
            gameObject.SetActive(true);
            hotbarScript.Select();
        }


        public static void WandInit()
        {
            charges = 0;
        }
    }
}