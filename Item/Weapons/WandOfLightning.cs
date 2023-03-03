using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.ThunderAndLightning;


namespace CevarnsOfEvil
{

    public class WandOfLightning : Wand
    {
        [SerializeField] float rateOfFire = 1.0f;
        private float fireTime;

        [SerializeField] LightningBoltPrefabScript lightning;
        [SerializeField] AudioSource thunder;

        private static int charges;
        public override int Charges { get { return charges; } set { charges = value; } }


        public void Start()
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
                LightnightBlast(aim.from, aim.toward, player.PlayerScript);
                anim.SetTrigger("Act");
                thunder.Play();
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
            // Mobs Don't Use This
        }


        protected void LightnightBlast(Vector3 from, Vector3 toward, Entity attacker)
        {
            GameObject hit;
            Vector3 last;
            Ray ray = new Ray(from, toward);
            RaycastHit[] targets = Physics.RaycastAll(ray);
            System.Array.Sort(targets, HitSorter.Instance);
            if ((targets != null) && (targets.Length > 0))
            {
                last = aimTransform.position;
                foreach (RaycastHit target in targets)
                {
                    if (target.collider != null)
                    {
                        lightning.Trigger(last, target.point);
                        hit = target.collider.gameObject;
                        EntityHealth victim = hit.GetComponent<EntityHealth>();
                        if (hit.CompareTag("Floor") || hit.CompareTag("Wall"))
                        {
                            GameObject hitParticles = hit.GetComponent<Mesher>().Substance.HitParticles;
                            Instantiate(hitParticles, target.point,
                                Quaternion.FromToRotation(Vector3.forward, target.normal));
                            break;
                        }
                        else if (victim != null)
                        {                            
                            victim.BeHitByRaycastAttack(target, Random.Range(120, 180), 
                                DamageType.electric, attacker);
                        }
                        last = target.point;
                    }
                }
            }
            else
            {
                lightning.Trigger(aimTransform.position, from + (toward * 64));
            }
        }


        public override void OnPlayerSelect(PlayerAct player)
        {
            gameObject.SetActive(true);
            hotbarScript.Select();
        }


        public override void Init()
        {
            base.Init();
        }


        public static void WandInit()
        {
            charges = 0;
        }
    }


    public class HitSorter : IComparer<RaycastHit>
    {
        public static readonly HitSorter Instance = new HitSorter();

        public int Compare(RaycastHit x, RaycastHit y)
        {
            return (int)Mathf.Sign(x.distance - y.distance);
        }
    }


}
