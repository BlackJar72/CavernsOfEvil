using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class EntityZombie : EntityMob
    {
        protected float prefferedSpeed;

        // Delegates


        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            prefferedSpeed = ((Random.value * 2f) / 3f) + (1f / 3f);
            animSpeed = prefferedSpeed;
            setAnimSpeed = new SetAnimSpeed(SetAnimSpeedZombie);
            anim.SetFloat("Walk", (Random.value * 2f) - 1f);
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
            setAnimSpeed();
            Move();
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


        protected void SetAnimSpeedZombie()
        {
            anim.SetFloat("SpeedFactor", animSpeed);
        }


        public override void Move()
        {
            if (stepData.floorEffect == FloorEffect.ice)
            {
                float slipFactor = Time.deltaTime * 1.5f;
                movement = (AIVelocity * slipFactor) + (movement * (1 - slipFactor));
            }
            else movement = AIVelocity;

            shouldJump = onGround = IsOnGround();

            if (onGround)
            {
                if (shouldJump)
                {
                    vSpeed = 5;
                    anim.SetTrigger("Jump");
                }
                else
                {
                    vSpeed = Mathf.Max(vSpeed, 0);
                }
            }
            else
            {
                vSpeed -= 15 * Time.deltaTime;
            }

            velocity = movement + physicalVelocity;
            velocity.y += vSpeed;
            GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
            shouldJump = false;
            physicalVelocity *= (1 - Time.deltaTime);
            AIVelocity = Vector3.zero;
        }


    }

}