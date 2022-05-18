using UnityEngine;
using UnityEngine.AI;


namespace CevarnsOfEvil
{

    public class EntityDeath : MonoBehaviour
    {
        [SerializeField] protected Collider[] colliders;
        

        protected virtual void Update()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            Entity mob = gameObject.GetComponent<EntityMob>();
            if(mob != null) 
            {
                float floorHeight = mob.Manager.Dungeon.map.GetFloorY((int)(transform.position.x + 0.5f), 
                        (int)(transform.position.z + 0.5f));
                if(transform.position.y < floorHeight) 
                {
                    Vector3 landing = transform.position;
                    landing.y = floorHeight;
                    transform.position = landing;
                    ForceImmediate();
                    return;
                }
            }
            if ((rb.velocity.magnitude < 0.01) && (rb.angularVelocity.magnitude < 0.01f))
            {
                rb.isKinematic = true;
                rb.Sleep();
                foreach (Collider collider in colliders) { collider.enabled = false; }
                NavMeshAgent nma = GetComponent<NavMeshAgent>();
                if(nma != null) nma.enabled = false;
                enabled = false;
            }
        }



        public virtual void ForceImmediate() 
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.Sleep();
            foreach (Collider collider in colliders) { collider.enabled = false; }
            NavMeshAgent nma = GetComponent<NavMeshAgent>();
            if(nma != null) nma.enabled = false;
            enabled = false;
        }


        public virtual void Reset()
        {
            foreach (Collider collider in colliders) { collider.enabled = true; }
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb.velocity.magnitude < 0.01)
            {
                rb.isKinematic = false;
                rb.WakeUp();
                enabled = false;
            }

        }
    }
}
