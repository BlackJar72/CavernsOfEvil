using UnityEngine;
using UnityEngine.AI;
using kfutils;


namespace CevarnsOfEvil
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavSeeker : MonoBehaviour
    {
        public const float MAX_DIST = 1.0f;
        public const float MAX_DIST_SQR = MAX_DIST * MAX_DIST; 

        [SerializeField] EntitySeekerFollower parent;
        private NavMeshAgent agent;
        public bool stopped = true;


        public NavMeshAgent Agent => agent;


        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            transform.parent = transform.parent.parent;
        }


        // Update is called once per frame
        // This now looks at distance only in the horizonal plane. 
        // Vertical (y coordinate) may be used to set ShouldJump on 
        // entity following the seeker.
        void Update()
        {
            Vector3 separation = transform.position - parent.transform.position;
            agent.isStopped = stopped || (separation.HSqrMagnitude() > MAX_DIST_SQR);
        }
        
    }


}