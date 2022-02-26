using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class StickToTransform : MonoBehaviour
    {
        [SerializeField] Transform sticky;

        Vector3 original;


        private void Awake()
        {
            original = transform.localPosition;
        }


        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = sticky.position;
        }


        private void OnDisable()
        {
            transform.localPosition = original;
        }
    }

}
