using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class Sword : Item
    {
        [SerializeField] float attackTime = 0.4f;
        [SerializeField] int damage = 30;
        [SerializeField] DamageType damageType;
        [SerializeField] AudioSource swoosh;

        private float fireTime;
        private MeshRenderer shower;

        private static int held;

        public static int Held { get { return held; } }


        public int Damage { get { return damage; } }


        public void Start()
        {
            Init();
            fireTime = Time.time;
            shower = gameObject.GetComponentInChildren<MeshRenderer>();
        }


        public override void OnMobUse(Entity mob)
        {
            throw new System.NotImplementedException();
        }
        

        public override void OnPlayerUse(PlayerAct player, Animator anim)
        {
            if (Time.time > fireTime)
            {
                AimParams aim;
                fireTime = Time.time + attackTime;
                player.GetAimParams(out aim);
                if(player.PlayerScript.Mover.IsOnGround())
                {
                    CheckHit(aim.from, aim.toward, player.PlayerScript);
                }
                else
                {
                    CheckPowerHit(aim.from, aim.toward, player.PlayerScript);
                }
                player.Stamina = Mathf.Clamp(player.Stamina - 10, 0, PlayerAct.baseStamina);
                anim.SetInteger("Action", 1);
                anim.SetTrigger("Act");
                swoosh.Play();
                AlertListeningMobs(player.PlayerScript);
            }
        }


        public override void OnPlayerSelect(PlayerAct player)
        {
            hotbarScript.Select();
            gameObject.SetActive(true);
            fireTime = Time.time + attackTime;
            StartCoroutine(ShowItem());
        }


        private IEnumerator ShowItem()
        {
            yield return new WaitForSeconds(0.40f);
            shower.enabled = true;
            yield return null;
        }


        public override void OnPlayerDeselect(PlayerAct player)
        {
            shower.enabled = false;
            hotbarScript.Deselect();
            gameObject.SetActive(false);
        }


        protected void CheckHit(Vector3 from, Vector3 toward, Entity attacker)
        {
            RaycastHit target;
            GameObject hit;
            if (Physics.SphereCast(from, 0.25f, toward, out target, 1.75f, GameConstants.MobMask))
            {
                if ((target.collider != null))
                {
                    hit = target.collider.gameObject;
                    EntityHealth victim = hit.GetComponent<EntityHealth>();
                    if (hit.CompareTag("Floor") || hit.CompareTag("Wall"))
                    {
                        GameObject hitParticles = hit.GetComponent<Mesher>().Substance.HitParticles;
                        Instantiate(hitParticles, target.point,
                            Quaternion.FromToRotation(Vector3.forward, target.normal));
                    }
                    else if (victim != null)
                    {
                        if((victim.Owner is EntityMob) && !((EntityMob)victim.Owner).HasTarget) {
                            victim.BeHitByRaycastAttack(target, (damage * 2) + 10, damageType, attacker);
                        }
                        else victim.BeHitByRaycastAttack(target, damage, damageType, attacker);
                    }
                }
            }
        }


        protected void CheckPowerHit(Vector3 from, Vector3 toward, Player attacker)
        {
            RaycastHit target;
            GameObject hit;
            if (Physics.SphereCast(from, 0.25f, toward, out target, 2.0f, GameConstants.DamageMask))
            {
                if ((target.collider != null))
                {
                    hit = target.collider.gameObject;
                    EntityHealth victim = hit.GetComponent<EntityHealth>();
                    if (hit.CompareTag("Floor") || hit.CompareTag("Wall"))
                    {
                        GameObject hitParticles = hit.GetComponent<Mesher>().Substance.HitParticles;
                        Instantiate(hitParticles, target.point,
                            Quaternion.FromToRotation(Vector3.forward, target.normal));
                    }
                    else if (victim != null)
                    {
                        if((victim.Owner is EntityMob) && !((EntityMob)victim.Owner).HasTarget) {
                            victim.BeHitByRaycastAttack(target, (damage * 2) + 30, damageType, attacker);
                        } else {
                            victim.BeHitByRaycastAttack(target, (damage * 2) + 10, damageType, attacker);
                        }
                        fireTime += (attackTime / 2f);
                        attacker.Actor.Stamina -= 5;
                    }
                }
            }
        }


        public override void BeAcquired()
        {
            hotbarScript.Change(icon);
            hotbarScript.Activate();
            equiped = true;
        }


        public static void SetSword(int swordID)
        {
            held = swordID;
        }


        public static void SwordInit()
        {
            held = 0;
        }
    }
}