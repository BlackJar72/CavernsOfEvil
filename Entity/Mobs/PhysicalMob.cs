using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [RequireComponent (typeof(Rigidbody))]
    public class PhysicalMob : EntityMob
    {
        private Rigidbody rb;

        // Movement Fields
        private float looky;
        public Vector3 movement;
        private Vector3 hVelocity;
        private Vector3 velocity;
        private float vSpeed;

        // Projectile Fileds
        [SerializeField] protected GameObject projectile;
        [SerializeField] protected Transform projectileSpawn;
        [SerializeField] protected float inaccuracy;
        [SerializeField] float rateOfFire;
        protected float fireDelay;
        protected float nextFireTime;

        protected float timeForCourseChange;


        // Start is called before the first frame update
        public override void Start()
        {
            rb = GetComponent<Rigidbody>();
            base.Start();
        }


        // Update is called once per frame
        public override void Update() {/*Probably do nothing (unless some things?)*/}




        protected virtual void FixedUpadate()
        {
            Move();
        }


        public override void GetAimParams(out AimParams aim)
        {
            float magnitude, rotation, x, y;
            Quaternion scatter;
            aim.from = projectileSpawn.transform.position;
            aim.toward = (targetEntity.GetComponent<Collider>().bounds.center - projectileSpawn.position).normalized;
            magnitude = Random.Range(-inaccuracy, inaccuracy);
            rotation = Random.Range(0, 360);
            x = Mathf.Sin(rotation) * magnitude;
            y = Mathf.Cos(rotation) * magnitude;
            scatter = Quaternion.AngleAxis(x, projectileSpawn.right)
                    * Quaternion.AngleAxis(y, projectileSpawn.up);
            aim.toward = scatter * aim.toward;
        }


        #region Movement


        public virtual void Move()
        {
            
        }


        public Vector3 RandomHorizontalDiagonal()
        {
            Vector3 straight = destination - transform.position;
            straight.y = 0;
            Quaternion.LookRotation(straight, Vector3.up).ToAngleAxis(out float theta, out Vector3 up);
            switch (Random.Range(0, 4))
            {
                case 0:
                    theta += 30;
                    break;
                case 1: 
                    theta -= 30;
                    break;
                case 2:
                    theta += Random.Range(0.0f, 45.0f) - Random.Range(0.0f, 45.0f);
                    break;
                case 3:
                default: break;
            }
            return new Vector3(Mathf.Sin(theta * Mathf.Deg2Rad), 0, Mathf.Cos(theta * Mathf.Deg2Rad));
        }



        public void RandomLookyFromDirection(Vector3 dir)
        {
            dir.y = 0;
            Quaternion.LookRotation(dir, Vector3.up).ToAngleAxis(out float theta, out Vector3 up);
            switch (Random.Range(0, 4))
            {
                case 0:
                    theta += 30;
                    break;
                case 1: 
                    theta -= 30;
                    break;
                case 2:
                    theta += Random.Range(0.0f, 45.0f) - Random.Range(0.0f, 45.0f);
                    break;
                case 3:
                default: break;
            }
            if(theta < 0.0f) theta = 360.0f - theta;
            if(theta > 360.0f) theta -= 360.0f;
            looky = theta;
            SetDirection(new Vector3(Mathf.Sin(theta * Mathf.Deg2Rad), 0, Mathf.Cos(theta * Mathf.Deg2Rad)));
        }


        #endregion


    }

}