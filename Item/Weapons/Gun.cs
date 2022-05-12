using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public abstract class Gun : Item
    {

        [SerializeField] protected int ammoTypeID;
        public Light muzzleLight;


        public int AmmoTypeID { get { return ammoTypeID; } }


        protected void FireBulletPlayer(Vector3 from, Vector3 toward, Entity attacker)
        {
            RaycastHit target;
            GameObject hit;
            if (Physics.Raycast(from, toward, out target, 256, GameConstants.PlayerAttackMask))
            {
                if (target.collider != null)
                {
                    hit = target.collider.gameObject;
                    EntityHealth victim = hit.GetComponent<EntityHealth>();
                    if (hit.layer == GameConstants.levelLayer)
                    {
                        GameObject hitParticles = hit.GetComponent<Mesher>().Substance.HitParticles;
                        if(hitParticles)
                        Instantiate(hitParticles, target.point,
                            Quaternion.FromToRotation(Vector3.forward, target.normal));
                    }
                    else if (victim != null)
                    {
                        victim.BeHitByBullet(target, attacker);
                    }
                }
            }
        }


        protected void FireBulletMob(Vector3 from, Vector3 toward, Entity attacker)
        {
            RaycastHit target;
            GameObject hit;
            if (Physics.Raycast(from, toward, out target, 256, GameConstants.MobAttackMask))
            {
                if (target.collider != null)
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
                        victim.BeHitByBullet(target, attacker);
                    }
                }
            }
        }


        public override void OnPlayerDeselect(PlayerAct player)
        {
            muzzleLight.gameObject.SetActive(false);
            base.OnPlayerDeselect(player);
        }


        protected IEnumerator FlashLight()
        {
            muzzleLight.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.25f);
            muzzleLight.gameObject.SetActive(false);
        }

    }

}
