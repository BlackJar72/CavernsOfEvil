using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    [RequireComponent(typeof(Rigidbody))]
    public class FlyingImp : EntityRangedMob, IWorldSensorUser
    {
        [SerializeField] WorldSensor sensor;
        private Rigidbody rb;

        private float looky;
        public Vector3 movement;
        private Vector3 hVelocity;
        private Vector3 velocity;
        private float vSpeed;

        private bool movementDecided;


        // Start is called before the first frame update
        public override void Start()
        {
            rb = GetComponent<Rigidbody>();
            base.Start();
        }


        // Update is called once per frame
        public override void Update()
        {
            base.Update();
            DecideMovement();
        }


        private void DecideMovement()
        {
            if(movementDecided) return;
        }


        public virtual void FixedUpdate()
        {
            movementDecided = false;           
        }


        public void OnWorldSensorTriggered(Collider other)
        {
            Debug.Log("Sensor collided with " + other.gameObject.name);
        }


        public void OnWorldSensorExit(Collider other)
        {
            //Debug.Log("Sensor left " + other.gameObject.name);
        }


    }


}