using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class RagdollController : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] Collider parentCollider;
        [SerializeField] Rigidbody parentRB;
        [SerializeField] Collider[] children;
        [SerializeField] Rigidbody[] rb;

        private bool defaultParentRB;


        private void Start()
        {
            defaultParentRB = parentRB.isKinematic;
            Alive();
        }


        public void Alive()
        {
            animator.enabled = true;
            parentCollider.enabled = true;
            parentRB.isKinematic = defaultParentRB;
            foreach (Collider collider in children)
            {
                collider.enabled = false;
            }
            foreach (Rigidbody collider in rb)
            {
                collider.isKinematic = true;
            }
        }


        public void Dead()
        {
            animator.enabled = false;
            parentCollider.enabled = false;
            parentRB.isKinematic = true;
            foreach (Collider collider in children)
            {
                collider.enabled = true;
            }
            foreach (Rigidbody collider in rb)
            {
                collider.isKinematic = false;
            }
        }


        public void Deactivate()
        {
            foreach (Collider collider in children)
            {
                collider.enabled = false;
            }
            foreach (Rigidbody collider in rb)
            {
                collider.isKinematic = true;
            }
        }


        public bool Stopped()
        {
            foreach (Rigidbody rigid in rb)
            {
                if ((rigid.velocity.sqrMagnitude > 0.01) 
                    || (rigid.angularVelocity.sqrMagnitude > 0.01)) return false;
            }
            return true;
        }


        public bool CheckForStopping()
        {
            bool output = Stopped();
            if (output) Deactivate();
            return output;
        }


        public bool CheckForStoppingTimed(float deadline)
        {
            bool output = (Time.time > deadline) || Stopped();
            if (output) Deactivate();
            return output;
        }

    }

}