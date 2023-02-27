using UnityEditor;
using UnityEngine;


namespace CevarnsOfEvil {


    public class EnvironmentalAOE : MonoBehaviour {
        [SerializeField] int damage;
        [SerializeField] DamageType type;


        void OnTriggerEnter(Collider other) {
            EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
            if(victim) {
                victim.BeHitByEnviroDamage(damage, type);
            }
        }


        void OnTriggerStay(Collider other) {
            EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
            if(victim) {
                victim.BeHitByEnviroDamage(damage, type);
            } else if(other.gameObject.GetComponent<EnvironmentalAOE>()) {
                Destroy(gameObject);
            }
        }


    }

}