using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    /// <summary>
    /// This is different from an explosion in that is does not add explosive force.  
    /// </summary>
    public class WandFireballDamager : SpawnableByProjectile
    {
        [SerializeField] int damage = 100;
        [SerializeField] float range = 7.5f;
        [SerializeField] DamageType damageType;
        public ParticleSystem visuals;


        public void Explode(GameObject projectile, Collision hit, Player attacker)
        {
            Instantiate(visuals, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, GameConstants.DamageMask);
            foreach (Collider collider in colliders)
            {
                EntityHealth health = collider.GetComponent<EntityHealth>();
                if ((health != null) 
                    && (InSameRoom(attacker, hit, collider) || CheckSight(collider, hit)))
                {
                    health.BeHitByAttack(damage, damageType, attacker);
                    if(health is PlayerHealth) {
                        ((Player)health.Owner).ActivateFireOverlay();
                    }
                }
            }
            Destroy(gameObject);
        }


        public void ExplodeForMob(GameObject projectile, Collision hit, Entity attacker)
        {
            Instantiate(visuals, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, GameConstants.DamageMask);
            foreach (Collider collider in colliders)
            {
                EntityHealth health = collider.GetComponent<EntityHealth>();
                if ((health != null) && (CheckSight(collider, hit)))
                {
                    health.BeHitByAttack(damage, damageType, attacker);
                    if(health is PlayerHealth) {
                        ((Player)health.Owner).ActivateFireOverlay();
                    }
                }
            }
            Destroy(gameObject);
        }


        public bool CheckSight(Collider collider, Collision hit)
        {
            return !Physics.Linecast(hit.collider.bounds.center, 
                collider.bounds.center, GameConstants.LevelMask);
        }


        public override void OnSpawn(GameObject projectile, Collision hit, Entity attacker)
        {
            if(attacker is Player) Explode(projectile, hit, (Player)attacker);
            else ExplodeForMob(projectile, hit, attacker);
        }


        private bool InSameRoom(Player player, Collision hit, Collider collider)
        {
#if UNITY_EDITOR
            return (player.Mover.dungeon == null) || (player.Mover.dungeon.Manager
                .InSameRoom(transform.position, collider.bounds.center));

#else
            return player.Mover.dungeon.Manager
                .InSameRoom(transform.position, collider.bounds.center);
#endif
        }
    }

}
