using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil {

    public class SpawnerProjectile : BasicProjectile {

        [SerializeField] GameObject spawnedPrefab;
        [SerializeField] GameObject impactEffect;
        [SerializeField] bool placeOnFloor;
        [SerializeField] bool isAttack;

        private Level dungeon;

        public Level Dungeon { get {return dungeon; } set { dungeon = value; } }


        /// <summary>
        /// Handle collision by applying damage to damagable targets and
        /// instantiating on special effects (such particle, explosion, or
        /// summons), before destroying the projectile.
        /// </summary>
        /// <param name="collision"></param>
        public override void OnCollisionEnter(Collision collision)
        {
            Entity victim = collision.gameObject.GetComponent<Entity>();
            if (isAttack && (victim != null))
            {
                EntityHealth health = collision.gameObject.GetComponent<Entity>().Health;
                health.BeHitByAttack(damageBase, damageType, attacker);
            }
            Instantiate(impactEffect, transform.position, transform.rotation).SetActive(true);
            Vector3 spawnPos = transform.position;
            bool placeabble = true;
            if(placeOnFloor) {
                Vector2Int mapCoords = new Vector2Int((int)spawnPos.x, (int)spawnPos.z);
                placeabble = dungeon.map.GetInBounds(mapCoords.x, mapCoords.y)
                          && (dungeon.map.GetRoom(mapCoords.x, mapCoords.y) > 0)
                          && ((spawnPos.z - (float)dungeon.map.GetFloorY(mapCoords.x, mapCoords.y)) < 2f);
                spawnPos.y = (float)dungeon.map.GetFloorY(mapCoords.x, mapCoords.y);
            }
            if(placeabble) {
                Instantiate(spawnedPrefab.gameObject, spawnPos, Quaternion.Euler(0,
                            Random.Range(0f, 360f), 0));
            }
            Destroy(gameObject);
        }

    }

}
