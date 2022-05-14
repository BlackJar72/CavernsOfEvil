using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
This is class specifically for applying physics to an AI character
(a monster) using the kinematic CharacterController.
*/

namespace CevarnsOfEvil
{
    [RequireComponent(typeof(CharacterController))]
    public class CCPhysics : MonoBehaviour
    {
        [SerializeField] protected Vector3 gravity;
        [SerializeField] protected bool falling; // Should use gravity
        [SerializeField] protected GameObject body;

        protected CharacterController cc;
        protected Vector3 velocity;
        protected Vector3 physcialVelocity;
        protected Vector3 aimove;
        protected MapMatrix leveldata;
        public bool hitWall;


        public bool Falling { get {return falling; } set { falling = value; } }
        public Vector3 AIMove { get {return aimove; } set {aimove = value; }}
        public Vector3 Velocity { get {return velocity; } set {velocity = value; } }
        public Vector3 PhysicalVelocity { get {return physcialVelocity; } set {physcialVelocity = value; } }
        public CharacterController physicsbody { get {return cc; } }
        public MapMatrix LevelData { get {return leveldata; } set {leveldata = value; } }


        // Start is called before the first frame update
        void Start()
        {
            cc = GetComponent<CharacterController>();
            velocity = physcialVelocity = aimove = Vector3.zero;
            if(body == null) body = gameObject;
        }



        protected virtual void FixedUpdate()
        {
            if(falling && !IsOnGround()) 
            {
                physcialVelocity += gravity * Time.deltaTime;
            }
            else
            {
                physcialVelocity = Vector3.zero;
            }
            velocity = PhysicalVelocity + aimove;
            hitWall = (cc.Move(velocity * Time.deltaTime) == CollisionFlags.Sides);
        }


        protected virtual bool IsOnGround() 
        {
            float floorHeght = leveldata.GetFloorY((int)(body.transform.position.x + 0.5f), 
                                                   (int)(body.transform.position.z + 0.5f));
            if(body.transform.position.y < floorHeght) 
                {
                    Vector3 newPos = transform.position;
                    newPos.y = floorHeght;
                    body.transform.position = newPos;
                    return true;
                }
            return body.transform.position.y == floorHeght;
        }


        public virtual bool ShouldCorpseStop() 
        {
            return (velocity.magnitude < 0.01)
                && (body.transform.position.y < leveldata.GetFloorY((int)(body.transform.position.x + 0.5f), 
                                                   (int)(body.transform.position.z + 0.5f)));
        }


        public virtual void Die() 
        {
            aimove = Vector3.zero;
            falling = true;            
        }

}

}
