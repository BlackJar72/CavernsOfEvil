using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    public class EntityDeath : MonoBehaviour
    {
        [SerializeField] Collider[] colliders;

        void Update()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb && (rb.linearVelocity.magnitude < 0.01) && (rb.angularVelocity.magnitude < 0.01f))
            {
                rb.isKinematic = true;
                rb.Sleep();
                foreach (Collider collider in colliders) { collider.enabled = false; }
                NavMeshAgent nma = GetComponent<NavMeshAgent>();
                if(nma != null) nma.enabled = false;
                enabled = false;
            }
        }



        public void ForceImmediate() 
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.Sleep();
            foreach (Collider collider in colliders) { collider.enabled = false; }
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            if(nma != null) nma.enabled = false;
            enabled = false;
        }


        public void Reset()
        {
            foreach (Collider collider in colliders) { collider.enabled = true; }
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb && (rb.linearVelocity.magnitude < 0.01))
            {
                rb.isKinematic = false;
                rb.WakeUp();
                enabled = false;
            }

        }
    }
}
