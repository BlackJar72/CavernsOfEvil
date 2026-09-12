using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    /// <summary>
    /// This is different from an explosion in that is does not add explosive force.  
    /// </summary>
    public class AOEDamager : SpawnableByProjectile
    {
        [SerializeField] int damage = 100;
        [SerializeField] float range = 7.5f;
        [SerializeField] DamageType damageType;
        public ParticleSystem visuals;


        public void Explode(GameObject projectile, Collision hit, Entity attacker)
        {
            Instantiate(visuals, transform.position, transform.rotation);
            Collider[] colliders = Physics.OverlapSphere(transform.position, range, GameConstants.DamageMask);
            Collider projCollider = projectile.GetComponent<Collider>();
            foreach (Collider collider in colliders)
            {
                EntityHealth health = collider.GetComponent<EntityHealth>();
                if ((health != null) && (InSameRoom(attacker, hit, collider) || (CheckSight(collider, hit))))
                {
                    // This should be fine, as I don't think anything else used this besides wand of fire fire balls
                    // (if another uses is created, the script can be duplicated and this removed).
                    if(health is PlayerHealth) {
                        health.BeHitByAttack((damage * 3) / 4, damageType, attacker);
                        ((Player)health.Owner).ActivateFireOverlay();
                    }
                    else health.BeHitByAttack(damage, damageType, attacker);
                }
            }
            Destroy(gameObject);
        }


        public bool CheckSight(Collider collider, Collision hit)
        {
            return !Physics.Linecast(hit.collider.bounds.center, collider.bounds.center, GameConstants.LevelMask);
        }


        public override void OnSpawn(GameObject projectile, Collision hit, Entity attacker)
        {
            Explode(projectile, hit, attacker);
        }


        private bool InSameRoom(Entity entity, Collision hit, Collider collider)
        {
            //if(entity is Player) { // Why did I ever have this limitation; it was supposed to test same room for all targers!
                Player player = entity as Player;
                #if UNITY_EDITOR
                return (player.Mover.dungeon == null) || (player.Mover.dungeon.Manager
                .InSameRoom(transform.position, collider.bounds.center));

                #else
                return player.Mover.dungeon.Manager
                .InSameRoom(transform.position, collider.bounds.center);
                #endif
            //}
            //else return false;
        }
    }

}
