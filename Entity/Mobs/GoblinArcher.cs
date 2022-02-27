using UnityEngine;


namespace CevarnsOfEvil
{

    public class GoblinArcher : EntityGoblinWarrior01, IArcher
    {
        [SerializeField] StickToTransform grabStringScript;
        [SerializeField] GameObject arrow;
        [SerializeField] Transform arrowSpawn;
        [SerializeField] Transform arrowPivot;
        [SerializeField] protected float inaccuracy;

        [SerializeField] GameObject bow;
        [SerializeField] Collider bowCollider;

        protected float? targetAngle;
        protected AimParams aim;
        protected Vector3 relVelocity;

        protected bool readyToShoot;

        public bool ReadyToShoot { get => readyToShoot; }


        public override void Start()
        {
            base.Start();
            readyToShoot = true;
        }


        public override void Update()
        {
            base.Update();
            relVelocity = transform.InverseTransformVector(navMeshAgent.velocity).normalized;
            anim.SetFloat("ZSpeed", (relVelocity.z * animSpeed));
            anim.SetFloat("XSpeed", (relVelocity.x * animSpeed));
        }


        public override void SetFactorSpeed(float speedFactor)
        {
            navMeshAgent.speed = baseMoveSpeed * speedFactor;
        }


        public override void GetAimParams(out AimParams aim)
        {
            aim.from = arrowSpawn.position;
            aim.toward = (targetObject.GetComponent<Collider>().bounds.center - arrowSpawn.position).normalized;

            float magnitude, rotation, x, y;
            Quaternion scatter;
            magnitude = Random.Range(-inaccuracy, inaccuracy);
            rotation = Random.Range(0, 360);
            x = Mathf.Sin(rotation) * magnitude;
            y = Mathf.Cos(rotation) * magnitude;
            scatter = Quaternion.AngleAxis(x, arrowPivot.right)
                    * Quaternion.AngleAxis(y, arrowPivot.up);
            aim.toward = scatter * aim.toward;
        }


        public void ShootGrabString()
        {
            grabStringScript.enabled = true;
        }

        public void ShootReleaseString()
        {
            grabStringScript.enabled = false;
            if (!isDead) SpawnArrow();
        }


        protected void SpawnArrow()
        {
            GetAimParams(out aim);
            GameObject proj = Instantiate(arrow, arrowSpawn.position, arrowSpawn.rotation);
            proj.GetComponent<Arrow>().LaunchSimple(new
                Vector3(transform.forward.x, aim.toward.y, transform.forward.z), this);
            entitySounds.PlaySound(voice, ESoundType.ATTACK);
        }


        public void ReadyAttackAngles()
        {
            if (targetAngle != null)
            {
                Vector3 toTarget = targetObject.transform.position - arrowPivot.position;
                targetAngle = Mathf.Tan(toTarget.y / Mathf.Sqrt((toTarget.x * toTarget.x) + (toTarget.z * toTarget.z)));

                arrowPivot.transform.localEulerAngles = new Vector3((float)targetAngle, 0, 0);
                anim.SetFloat("Angle", (float)targetAngle / -70f);

                GetAimParams(out aim);
            }
        }


        public void AttackOver()
        {
            readyToShoot = true;
            nextAttack = Time.time + attackTime + (Random.value * attackTime);
            anim.SetInteger("AnimID", 0);
        }


        public virtual void RangedAttack()
        {
            nextAttack = Time.time + attackTime + (Random.value * attackTime);
            readyToShoot = false;
        }

        public void ArrowAttack()
        {
            RangedAttack();
        }


        public override void Die(Damages damages)
        {
            //GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            targetAngle = null;
            readyToShoot = false;
            grabStringScript.enabled = false;
            bow.transform.parent = null;
            bowCollider.enabled = true;
            bow.GetComponent<EntityDeath>().enabled = true;
            Rigidbody bowrb = bow.GetComponent<Rigidbody>();
            bowrb.isKinematic = false;
            bowrb.velocity = (Vector3.down * 9.8f) 
                + new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f); ;
            bowrb.angularVelocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f);
            base.Die(damages);
        }
    }

}