using UnityEngine;
using UnityEngine.InputSystem;


namespace CevarnsOfEvil
{

    public class MovePlayer : MonoBehaviour
    {
        public float baseSpeed = 3f;
        public float jumpForce = 400f;
        public float mouseSensitivity = 10f;
        public GameObject camPivot;
        public GameObject playerCam;
        public GameObject bodyCam;

        public Vector3 movement;
        public GameObject playerBody;

        private Animator animator;

        public Player player;
        public PlayerAct actor;
        public Level dungeon;

        private float looky;
        private Vector2[] moveIn = new Vector2[4];
        private Vector2[] lookIn = new Vector2[4];
        private int moveType;
        private bool falling;

        private float enviroCooldown;
        private StepData stepData;
        private bool onGround;
        [HideInInspector] public bool flying;

        private Rigidbody rigid;
        private Vector3 hVelocity;
        private Vector3 velocity;
        private float vSpeed;
        private Collider[] footContats;

        public bool OnGround { get; private set; }
        public float Looky { get => looky; }

        // Input System
        private PlayerInput input;
        private InputAction moveAction;
        private InputAction lookAction;
        private InputAction jumpAction;
        private InputAction sprintAction;
        private InputAction sprintToggle;
        private bool shouldJump;
        private bool shouldSprint;



        void Awake()
        {
            footContats = new Collider[3];
            InitInput();
        }


        // Start is called before the first frame update
        void Start()
        {
            animator = playerBody.GetComponent<Animator>();
            rigid = gameObject.GetComponent<Rigidbody>();
            actor = GetComponent<PlayerAct>();
            Cursor.lockState = CursorLockMode.Locked;
            looky = playerCam.transform.eulerAngles.x;
            enviroCooldown = Time.time;
            moveType = 2;
            falling = false;
        }


        public void SetLevel(Level level)
        {
            dungeon = level;
        }


        // Update is called once per frame
        void Update()
        {
            if (shouldSprint
                && (actor.Stamina > 0) && (movement != Vector3.zero))
            {
                moveType = 3;
            }
            else
            {
                moveType = 2;
            }
            SetAnimationVars();
#if UNITY_EDITOR
            if ((dungeon != null) && !flying)
            {
                stepData = dungeon.map.GetStepData(transform.position, dungeon,
                    player.Health, ref enviroCooldown);
            }
#else
                stepData = dungeon.map.GetStepData(transform.position, dungeon,
                    player.Health, ref enviroCooldown);
#endif
            if (moveType == 3)
            {
                actor.Stamina = Mathf.Clamp(actor.Stamina - Time.deltaTime, 0, PlayerAct.baseStamina);
            }
            else
            {
                actor.Stamina = Mathf.Clamp(actor.Stamina + (Time.deltaTime * 10), 0, PlayerAct.baseStamina);
            }
        }


        void LateUpdate()
        {
            AdjustHeading();
            //if (flying) Fly();
            //else Move();
            Move();
        }

#region Input
        private void InitInput()
        {
            input = GetComponent<PlayerInput>();
            moveAction = input.actions["Move"];
            lookAction = input.actions["Look"];
            jumpAction = input.actions["Jump"];
            sprintAction = input.actions["Sprint"];
            sprintToggle = input.actions["Toggle Sprint"];
            jumpAction.started += triggerJump;
            sprintAction.started += startSprint;
            sprintAction.canceled += stopSprint;
            sprintToggle.started += toggleSprint;
        }


        private void GetLookInput()
        {
#if UNITY_STANDALONE_LINUX
            lookIn[0] = lookIn[1]; lookIn[1] = lookIn[2];
            lookIn[2] = lookAction.ReadValue<Vector2>() * Options.lookSensitivity;
            lookIn[3] = ((lookIn[0] + lookIn[1] + lookIn[2]) / 3f);
#else 
            lookIn[3] = lookAction.ReadValue<Vector2>() * Options.lookSensitivity;
#endif
        }


        private void GetMoveInput()
        {
            moveIn[0] = moveIn[1]; moveIn[1] = moveIn[2];
            moveIn[2] = moveAction.ReadValue<Vector2>() * Options.moveSensitivity;
            moveIn[3] = ((moveIn[0] + moveIn[1] + moveIn[2]) / 3f);
        }


        private void triggerJump(InputAction.CallbackContext context)
        {
            shouldJump = true;
        }


        private void startSprint(InputAction.CallbackContext context)
        {
            shouldSprint = true;
        }


        private void stopSprint(InputAction.CallbackContext context)
        {
            shouldSprint = false;
        }


        private void toggleSprint(InputAction.CallbackContext context)
        {
            shouldSprint = !shouldSprint;
        }

#endregion


        private void AdjustHeading()
        {
            GetLookInput();

            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + lookIn[3].x,
                transform.eulerAngles.z
            );

            looky = Mathf.Clamp(looky - lookIn[3].y, -70f, 70f);
            camPivot.transform.rotation = Quaternion.Euler(looky / 2, transform.eulerAngles.y, 0).normalized;
            playerCam.transform.localRotation = Quaternion.Euler(looky / 2, camPivot.transform.eulerAngles.z, 0).normalized;
        }


        private void Move()
        {
            GetMoveInput();
            movement.Set(moveIn[3].x, 0, moveIn[3].y);

            Vector3 newVelocity = new Vector3(0, velocity.y, 0);

            if (movement.magnitude > 0)
            {
                float speedFactor;
                if (stepData.floorEffect == FloorEffect.slow) speedFactor = 1;
                else speedFactor = moveType;
                if (movement.magnitude > 1)
                {
                    newVelocity += transform.rotation * (movement.normalized * baseSpeed * speedFactor);
                }
                else
                {
                    newVelocity += transform.rotation * (movement * baseSpeed * speedFactor);
                }
            }

            if(stepData.floorEffect == FloorEffect.ice)
            {
                float slipFactor = Time.deltaTime * 1.5f;
                hVelocity = (newVelocity * slipFactor) + (hVelocity * (1 - slipFactor));
            }
            else hVelocity = newVelocity;

            onGround = IsOnGround();
            falling = falling && !onGround;

            if (onGround)
            {
                if (shouldJump && (actor.Stamina > 10f))
                {
                    vSpeed = 8;
                    actor.Stamina = Mathf.Clamp(actor.Stamina - 10f, 0, PlayerAct.baseStamina);
                    animator.SetTrigger("Jump");
                }
                else
                {
                    vSpeed = Mathf.Max(vSpeed, 0);
                }
            }
            else
            {
                vSpeed -= 15 * Time.deltaTime;
                if (!falling && (velocity.y < -9))
                {
                    falling = true;
                    animator.SetTrigger("Falling");
                }
            }
            velocity.Set(hVelocity.x, vSpeed, hVelocity.z);
            GetComponent<CharacterController>().Move(velocity * Time.deltaTime);
            shouldJump = false;
        }


        /*
        private void Fly()
        {
            GetMoveInput();
            movement.Set(moveIn[3].x, 0, moveIn[3].y);

            if (Input.GetAxis("Jump") > 0f) movement.y += 1;
            if (Input.GetKey(KeyCode.LeftShift)) movement.y -= 1;

                if (movement.magnitude > 0)
            {
                float speedFactor;
                if (stepData.floorEffect == FloorEffect.slow) speedFactor = 1;
                else speedFactor = moveType;
                if (movement.magnitude > 1)
                {
                    transform.position += transform.rotation * (movement.normalized * 16 * Time.deltaTime);
                }
                else
                {
                    transform.position += transform.rotation * (movement * 16 * Time.deltaTime);
                }
            }
        }


        public void ToogleFlying()
        {
            flying = !flying;
        }
        */

        private void SetAnimationVars()
        {
            animator.SetFloat("MoveZ", movement.z);
            animator.SetFloat("MoveX", movement.x);
            animator.SetInteger("MoveType", moveType);
            animator.SetBool("OnGround", IsOnGround());
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Finish"))
            {
                EndLevel end = other.GetComponent<EndLevel>();
                if (end != null) end.End();
            }
        }


        public bool IsOnGround()
        {
            // For now, assume if not rising or falling that 
            // we are on the ground and jumping is OK.  This 
            // may be combined with map data and a gradient 
            // for handling ramps and stairs later.
            //return (rigid.velocity.y * rigid.velocity.y < 0.001f);
            return (Physics.OverlapSphereNonAlloc(gameObject.transform.position, 
                0.1f, footContats, GameConstants.LevelMask) > 0);
        }


    }


}