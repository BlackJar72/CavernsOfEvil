using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [RequireComponent (typeof(Rigidbody))]
    public class PhysicalMob : EntityMob
    {
        protected Rigidbody rb;

        protected float timeForCourseChange;


        // Start is called before the first frame update
        public override void Start()
        {
            rb = GetComponent<Rigidbody>();
            base.Start();
        }


        // // Update is called once per frame
        // public override void Update()
        // {
        //     base.update();
        // }




        protected virtual void FixedUpadate()
        {
            Move();
        }


        public override void GetAimParams(out AimParams aim)
        {
            throw new System.NotImplementedException();
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



        #endregion


    }

}