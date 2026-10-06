using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    /// <summary>
    /// This class is for small, fast moving projectiles that are actually handled mostly as 
    /// hit scans, with the projectile game object acting mostly as a special effect. 
    /// 
    /// It will home toward the area detected by a raycast (the hit scan), and will act is if 
    /// hitting when it is very close to, at, or has past the area of the hit.  
    /// 
    /// This uses a dot product between the direction  (normalixed) it is shot in and and the 
    /// vector to the hit spot in place of distance.  By the law of cosines, the dot product is 
    /// equal to the distance times the cosine, thus it will become negative if the projectile 
    /// image is behind / has passed the target (hit spot from raycast). 
    /// 
    /// This only works with fast moving projectiles, so that the target can't move to much (thus 
    /// hiding the homing so the path looks straight) and not allowing angle between the current 
    /// and initial trajectory to become shallow (reducing the cosine),
    /// </summary>
    public class HomingProjectileFX : MonoBehaviour
    {
        [SerializeField] protected int damageBase;
        [SerializeField] protected float speed;
        [SerializeField] protected DamageType damageType;
        [SerializeField] protected GameObject impactEffect;

        [HideInInspector] public Entity attacker;
        [HideInInspector] public Vector3 initialDir;
        [HideInInspector] public GameObject target;
        [HideInInspector] public EntityHealth victim;


        // Update is called once per frame
        void Update()
        {
            Vector3 movement = (target.transform.position - transform.position).normalized * speed * Time.deltaTime;
            transform.position = transform.position + movement;
            if(Vector3.Dot(movement, initialDir) < (Time.deltaTime / speed)) Hit();
        }


        public void Launch(Vector3 dir, Entity attacker, RaycastHit rayhit, EntityHealth victim)
        {
            target = new GameObject("Hit Spot");
            target.transform.position = rayhit.point;
            target.transform.parent = rayhit.collider.gameObject.transform;
            target.transform.rotation.SetLookRotation(rayhit.normal);
            transform.rotation = Quaternion.LookRotation(dir);
            initialDir = dir.normalized;
            this.attacker = attacker;
            this.victim = victim;
        }


        public void Hit()
        {
            if(victim != null) victim.BeHitByAttack(damageBase, damageType, attacker);
            Instantiate(impactEffect, target.transform.position, transform.rotation, target.transform.parent).SetActive(true);
            Destroy(target);
            Destroy(gameObject);
        }



    }


}