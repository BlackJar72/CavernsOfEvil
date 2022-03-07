using UnityEngine;


namespace CevarnsOfEvil
{

    public class Zombie : EntityMob
    {
        protected float prefferedSpeed;
        protected float wanderUpdateTime;
        protected bool shouldTurn = false;

        // Accessors
        public float PrefferedSpeed { get => prefferedSpeed; set { prefferedSpeed = value; } }
        public float WanderUpdateTime { get => wanderUpdateTime; set { wanderUpdateTime = value; } }
        public bool ShouldTurn { get { return shouldTurn; } set { shouldTurn = value; } }


        // Start is called before the first frame update
        public override void Start()
        {
            animSpeed = prefferedSpeed = ((Random.value * 3f) / 4f) + 0.25f;
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody>();
            aggroRangeSq = aggroRange * aggroRange;
            CurrentBehavior = EmptyState.Instance.NextState(this);
            player = GameObject.Find("FemalePlayer");
            enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
            anim.SetFloat("Walk", (Random.value * 2f) - 1f);
            anim.SetFloat("SpeedFactor", 0);
            CurrentBehavior = EmptyState.Instance.NextState(this);
        }


        // Update is called once per frame
        public override void Update()
        {
            if (!currentBehavior.StateUpdate(this)) FindNewBehavior();
#if UNITY_EDITOR
            if (dungeon != null)
            {
                stepData = dungeon.map.GetStepData(transform.position, dungeon,
                    health, ref enviroCooldown);
            }
#else
            stepData = dungeon.map.GetStepData(transform.position, dungeon,
                health, ref enviroCooldown);
#endif
        }


        public virtual void FixedUpdate()
        {
            Move2D();
        }


        public override void GetAimParams(out AimParams aim)
        {
            aim.from = eyes.position;
            aim.toward = (targetObject.GetComponent<Collider>().bounds.center - eyes.position).normalized;

            float magnitude, rotation, x, y;
            Quaternion scatter;
            magnitude = Random.Range(-12, 12);
            rotation = Random.Range(0, 360);
            x = Mathf.Sin(rotation) * magnitude;
            y = Mathf.Cos(rotation) * magnitude;
            scatter = Quaternion.AngleAxis(x, transform.right)
                    * Quaternion.AngleAxis(y, transform.up);
            aim.toward = scatter * aim.toward;
        }


        public void SetAnimSpeedZombie()
        {
            anim.SetFloat("SpeedFactor", animSpeed);
        }


        public override void OnCollisionEnter(Collision collision)
        {
            ShouldTurn = true;
        }


        public override void TriggerHit(Collider other)
        {
            if (!isDead && (nextAttack < Time.time) && (other.gameObject == targetObject))
            {
                nextAttack = Time.time + attackTime;
                anim.SetInteger("Variant", Random.Range(0, 3));
                anim.SetTrigger("Attack");
                anim.SetFloat("SpeedFactor", animSpeed = 0);
                //entitySounds.PlayAttack(voice, 0);
                EntityHealth victim = other.gameObject.GetComponent<EntityHealth>();
                if (victim != null)
                {
                    victim.BeHitByAttack(meleeDamage, this);
                }
            }
        }


        public override void TriggerLeft(Collider other)
        {
            if (!isDead && (other.gameObject == targetObject))
            {
                anim.SetFloat("SpeedFactor", animSpeed = prefferedSpeed);
            }
        }


        public override void Die(Damages damages)
        {
            rigid.freezeRotation = true;
            base.Die(damages);
        }
    }

}