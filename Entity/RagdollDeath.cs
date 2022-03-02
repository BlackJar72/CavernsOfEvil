using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    public class RagdollDeath : EntityDeath
    {
        [SerializeField] RagdollController ragdoll;
        private float stopTime;

        private void OnEnable()
        {
            ragdoll.Dead();
            stopTime = Time.time + 5f;
        }


        protected override void Update()
        {
            if (ragdoll.CheckForStoppingTimed(stopTime))
            {
                foreach (Collider collider in colliders) { collider.enabled = false; }
                NavMeshAgent nma = GetComponent<NavMeshAgent>();
                if (nma != null) nma.enabled = false;
                enabled = false;
            }


        }


        public override void Reset()
        {
            foreach (Collider collider in colliders) { collider.enabled = true; }
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.WakeUp();
            enabled = false;
            ragdoll.Alive();
        }
    }

}