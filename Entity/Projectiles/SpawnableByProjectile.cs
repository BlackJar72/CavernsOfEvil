using UnityEngine;


namespace CevarnsOfEvil
{

    public abstract class SpawnableByProjectile : MonoBehaviour
    {
        public abstract void OnSpawn(GameObject projectile, Collision hit, Entity attacker);
    }

}