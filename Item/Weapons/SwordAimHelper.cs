using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;


namespace DLD
{
    public class SwordAimHelper : MonoBehaviour
    {
        private Camera cam;


        public void Start()
        {
            cam = gameObject.GetComponentInChildren<Camera>();
        }


        public void LateUpdate()
        {
            AimIK ik = gameObject.GetComponent<AimIK>();
            ik.solver.axis = ik.solver.transform.InverseTransformVector(ik.transform.rotation * cam.transform.forward);
        }
    }

}