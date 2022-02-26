using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class AttackAreaMonitor : MonoBehaviour
    {
        [SerializeField] Entity entity;


        public void OnEnterStay(Collider other)
        {
            entity.TriggerHit(other);
        }


        public void OnTriggerStay(Collider other)
        {
            entity.TriggerHit(other);
        }


        public void OnTriggerExit(Collider other)
        {
            entity.TriggerLeft(other);
        }

    }

}