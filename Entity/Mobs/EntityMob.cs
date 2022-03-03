using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public abstract partial class EntityMob : Entity
    {
        // Serialized Fields
        [SerializeField] protected EntitySounds entitySounds;
        [SerializeField] protected AudioSource voice;
        [SerializeField] protected Transform eyes;
        [SerializeField] protected float aggroRange;
        [SerializeField] protected float attackTime = 1.0f;
        [SerializeField] protected int meleeDamage = 0;
        [SerializeField] protected float meleeRange = 1.25f;
        [SerializeField] protected float baseMoveSpeed;

        // Protected Internals
        protected Animator anim;
        protected float aggroRangeSq;
        protected float nextAttack;
        protected float nextIdleTalk;
        protected float meleeStopDistance;
        protected Level dungeon;
        protected StepData stepData;
        protected float enviroCooldown;
        protected float animSpeed;

        // Keeping track of the current enemy
        [HideInInspector] public GameObject targetObject;
        [HideInInspector] public Entity targetEntity;
        [HideInInspector] public bool alerted;

        // Accessor Properties
        public Animator Anim { get { return anim; } }
        public float AttackTime { get { return attackTime; } }
        public float AggroRangeSq { get { return aggroRangeSq; } }
        public float NextAttack { get { return nextAttack; } set { nextAttack = value; } }
        public EntitySounds Sounds { get { return entitySounds; } }
        public AudioSource Voice { get { return voice; } }
        public float StasisAI { get { return stasisAI; } }
        public float NextIdleTalk { get { return nextIdleTalk; } set { nextIdleTalk = value; } }
        public float MeleeStopDistance { get { return meleeStopDistance; } }
        public float BaseMoveSpeed { get { return baseMoveSpeed; } }
        public Transform Eyes => eyes;


        // Special Internals -- may be replaced by other system
        protected GameObject player;

        // Delegate definitions
        protected delegate void SetAnimSpeed();

        // Delegates
        protected SetAnimSpeed setAnimByVelocity;
        protected SetAnimSpeed setAnimToZero;
        protected SetAnimSpeed setAnimSpeed;


        /************************************************************************************/
        /*                               METHOD CODE                                        */
        /************************************************************************************/


        public virtual void Start()
        {
            /*if(!dungeon.map.IsValidLocation(transform.position, GetCollider().bounds.extents.y * 2))
            {
                Destroy(gameObject);
            }*/
            anim = GetComponent<Animator>();
            aggroRangeSq = aggroRange * aggroRange;
            CurrentBehavior = EmptyState.Instance.NextState(this);
            player = GameObject.Find("FemalePlayer");
            enviroCooldown = nextIdleTalk = stasisAI = nextAttack = Time.time;
            setAnimByVelocity = new SetAnimSpeed(SetAnimSpeedVelocity);
            setAnimToZero = new SetAnimSpeed(SetAnimSpeedZero);
            setAnimSpeed = setAnimToZero;
        }


        public void SetGameManager(GameManager manager)
        {
            gameManager = manager;
        }


        public void SetGameLevel(Level level)
        {
            dungeon = level;
        }


        public virtual void Update()
        {
            if(!currentBehavior.StateUpdate(this)) FindNewBehavior();
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
            float tFactor = Time.deltaTime * 10;
            setAnimSpeed();
            Move();
        }


        protected void SetAnimSpeedVelocity()
        {
            float tFactor = Time.deltaTime * 10;
            animSpeed = (AIVelocity.magnitude * tFactor) + (animSpeed * (1 - tFactor));
        }


        protected void SetAnimSpeedZero()
        {
            animSpeed = 0;
        }


        public virtual void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.CompareTag("Player"))
            {
                alerted = true;
            }
        }


        public override void Die(Damages damages)
        {
            anim.SetTrigger("Die");
            anim.SetBool("Dead", true);
            base.Die(damages);
        }


        public bool InSameRoom(GameObject other)
        {
            return (dungeon == null) 
                || dungeon.Manager.InSameRoom(transform.position, other.transform.position);
        }


        public bool InSameRoom(Vector3 other)
        {
            return (dungeon == null)
                || dungeon.Manager.InSameRoom(transform.position, other);
        }


        public float DistanceSqrToPlayer()
        {
            return (transform.position - player.transform.position).sqrMagnitude;
        }


        public void TargetPlayer()
        {
            SetTarget(player);
        }


        public virtual void SetTarget(Entity enemy)
        {
            targetEntity = enemy;
            targetObject = enemy.gameObject;
        }


        protected virtual void SetTarget(GameObject enemy)
        {
            Entity enemyEntity = enemy.GetComponent<Entity>();
            if (enemyEntity != null)
            {
                targetObject = enemy;
                targetEntity = enemyEntity;
            }
        }


        public virtual void RemoveTarget()
        {
            targetEntity = null;
            targetObject = null;
        }


        public bool InMeleeRange()
        {
            return (targetObject.GetComponent<Collider>().bounds.center - eyes.position).sqrMagnitude 
                < (meleeRange * meleeRange);
        }


        public virtual void Attack()
        {
            if(InMeleeRange()) MeleeAttack();
        }


        public virtual void MeleeAttack()
        {
            nextAttack = Time.time + attackTime;

            EntityHealth victim = targetObject.GetComponent<EntityHealth>();
            if (victim != null)
            {
                victim.BeHitByAttack(meleeDamage, this);
            }
        }


        public override bool TakeDamage(ref Damages damage)
        {
            bool output = base.TakeDamage(ref damage);
            if (output && (damage.attacker != null)
                && ((targetEntity == null) || (Random.Range(0, health.Shock) < damage.shock))
                && (damage.attacker != this))
            {
                SetTarget(damage.attacker);
                if(damage.attacker is Player) AlertNearby(this, 8);
            }
            return output;
        }


        public void AlertNearby(EntityMob src, float range)
        {
            Collider[] colliders = Physics.OverlapSphere(voice.transform.position, 8, GameConstants.MobMask);
            foreach (Collider collider in colliders)
            {
                EntityHealth health = collider.gameObject.GetComponent<EntityHealth>();
                if ((health != null) && (health.Owner != null) && (health.Owner is EntityMob)
                    && !(health.Owner as EntityMob).alerted)
                {
                    (health.Owner as EntityMob).HearAllies(voice.transform.position);
                }
            }
        }

    }



}