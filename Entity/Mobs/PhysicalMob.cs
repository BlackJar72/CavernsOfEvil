using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [RequireComponent (typeof(Rigidbody))]
    public class PhysicalMob : EntityMob
    {
        protected Rigidbody rb;
        
        // Movement Fields
        protected float looky;
        public Vector3 movement;
        protected Vector3 hVelocity;
        protected Vector3 velocity;
        protected float vSpeed;
        protected float turnCooldown = float.NegativeInfinity;
        protected float collisionCooldown = float.NegativeInfinity;

        [SerializeField] protected GameObject projectile;
        [SerializeField] protected Transform projectileSpawn;
        [SerializeField] protected float inaccuracy;
        [SerializeField] float rateOfFire;

        protected float fireDelay;
        protected float nextFireTime;


        public float FireDelay { get { return fireDelay; } }
        public float NextFireTime { get { return nextFireTime; } set { nextFireTime = value; } }
        public Transform ProjSpawn { get { return projectileSpawn; } }

        protected float timeForCourseChange;


        // Start is called before the first frame update
        public override void Start()
        {
            velocity = Vector3.zero;
            hVelocity = transform.forward;
            rb = GetComponent<Rigidbody>();
            base.Start();
        }


        Vector3 lastVel = Vector3.zero;
        // Update is called once per frame
        public override void Update() {
            base.Update();
        }


        public override void Die(Damages damages)
        {
            rb.useGravity = true;
            entitySounds.PlayDeath(voice, 0);
            health.gameObject.SetActive(false);
            base.Die(damages);
        }


        protected virtual void FixedUpdate()
        {
            Move();
            int floory = GameManager.Instance.Dungeon.map.GetFloorY((int)rb.position.x, (int)rb.position.z);
            if(rb.position.y < floory)
            {
                rb.position = new Vector3(rb.position.x, floory, rb.position.z);
            }
        }


        protected void OnCollisionEnter(Collision collision)
        {
            Vector3 collisionNormal = collision.contacts[0].normal;
            collisionNormal.y = 0;
            if((collision.gameObject.CompareTag("Wall") || (collisionNormal.sqrMagnitude > 0)) && 
               ((Vector3.Angle(transform.forward, collisionNormal) < 15)) && (Time.time > collisionCooldown)) {
                collisionCooldown = Time.time + 0.1f;
                RandomLookyFromDirection(collisionNormal);
                Vector3 toTarget = desiredDirection;
                vSpeed = toTarget.y = 0;
                velocity = toTarget.normalized * baseMoveSpeed;
                toTarget.y = 0;
                hVelocity = toTarget.normalized * baseMoveSpeed;
                if(Random.Range(0, 2) == 0) NewTurnCooldown();
            }
        }


        public void NewTurnCooldown() => turnCooldown = Time.time + Random.Range(0.0f, 1.0f) + Random.Range(0.0f, 0.5f) + 0.5f;



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


        public virtual void GetNewDirection()
        {
            if(turnCooldown < Time.time) {
                if(HasLineOfSighToTarget() || InSameRoom(targetObject)) {                
                    Vector3 toTarget = (targetEntity.transform.position - transform.position);
                    velocity = toTarget.normalized * baseMoveSpeed;
                    toTarget.y = 0;
                    hVelocity = toTarget.normalized;
                } else {
                    RandomLookyFromDirection(hVelocity);
                    Vector3 toTarget = desiredDirection;
                    toTarget.y = 0;
                    velocity = toTarget.normalized * baseMoveSpeed;
                    hVelocity = toTarget.normalized * baseMoveSpeed;
                }
                NewTurnCooldown();
            } 
        }


        public virtual void Move()
        {
            LookAndMoveCurrentDir();
        }


        public virtual void LookAndMoveCurrentDir()
        {
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(hVelocity), Time.fixedDeltaTime * 10));
            rb.velocity = velocity;
        }


        public virtual Vector3 ToPlayerDir()
        {
            Vector3 toTarget = (player.transform.position - transform.position);
            velocity = toTarget.normalized * baseMoveSpeed;
            toTarget.y = 0;
            hVelocity = toTarget.normalized;
            return velocity;
        }


        public virtual Vector3 ToTargetDir()
        {
            Vector3 toTarget = (targetEntity.transform.position - transform.position);
            velocity = toTarget.normalized * baseMoveSpeed;
            toTarget.y = 0;
            hVelocity = toTarget.normalized;
            return velocity;
        }


        public virtual Vector3 ToCurrentTarget()
        {
            Vector3 toTarget = (player.transform.position - transform.position);
            toTarget.y = 0;
            toTarget.Normalize();
            return toTarget;
        }


        public Vector3 RandomHorizontalDiagonal()
        {
            Vector3 straight = destination - transform.position;
            straight.y = 0;
            Quaternion.LookRotation(straight, Vector3.up).ToAngleAxis(out float theta, out Vector3 up);
            switch (Random.Range(0, 3))
            {
                case 0:
                    theta = Random.Range(0.0f, 360.0f);
                    break;
                case 1: 
                    theta += Random.Range(0.0f, 45.0f) - Random.Range(0.0f, 45.0f);
                    break;
                case 2:
                default: break;
            }
            return new Vector3(Mathf.Sin(theta * Mathf.Deg2Rad), 0, Mathf.Cos(theta * Mathf.Deg2Rad));
        }



        public void RandomLookyFromDirection(Vector3 dir)
        {
            dir.y = 0;
            Quaternion.LookRotation(dir, Vector3.up).ToAngleAxis(out float theta, out Vector3 up);
            switch (Random.Range(0, 3))
            {
                case 0:
                    theta = Random.Range(0.0f, 360.0f);
                    break;
                case 1: 
                    theta += Random.Range(0.0f, 45.0f) - Random.Range(0.0f, 45.0f);
                    break;
                case 2:
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
