using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public abstract class AIHelper
    {
        public static bool CanShootTarget(EntityMob owner)
        {
            return ((owner.targetObject != null)
                && (owner.targetObject.GetComponent<Collider>() != null)
                && CanShootCollider(owner));
        }


        public static bool CanShootCollider(EntityMob owner)
        {
            GameObject other = owner.targetObject;
            Vector3 otherLoc = other.GetComponent<Collider>().bounds.center;
            Vector3 toOther = otherLoc - owner.Eyes.position;
            RaycastHit garbage;
            return ((toOther.sqrMagnitude < owner.AggroRangeSq)
                && (Vector3.Dot(owner.Eyes.forward, toOther) > 0)
                && !Physics.SphereCast(owner.Eyes.position, 0.2f, toOther, out garbage,
                toOther.magnitude, GameConstants.LevelMask));
        }

        
        public static Quaternion FindSkewDirection()
        {
            switch(Random.Range(0, 5))
            {
                case 0:
                case 1:
                    return Quaternion.identity;
                case 2:
                    return Quaternion.Euler(0, -30, 0);
                case 3:
                    return Quaternion.Euler(0,  30, 0);
                case 4:
                default:
                    return Quaternion.Euler(0, Random.Range(-45f, 45f), 0);
            }
        }


        public static void SkewDirection3d(ref Vector3 velocity) 
        {
            float dy = velocity.y;
            velocity.y = 0;
            velocity = FindSkewDirection() * velocity;
            velocity.y = dy;
        }


        public static Vector3 GetSkewedDirection3d(Vector3 velocity) 
        {
            float dy = velocity.y;
            velocity.y = 0;
            velocity = FindSkewDirection() * velocity;
            velocity.y = dy;
            return velocity;
        }


        private static Quaternion FindTurnDirection()
        {
            switch(Random.Range(0, 4))
            {
                case 0:
                    return Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                case 1:
                    return Quaternion.Euler(0, -30, 0);
                case 2:
                    return Quaternion.Euler(0,  30, 0);
                case 3:
                default:
                    return Quaternion.Euler(0, Random.Range(-45f, 45f), 0);
            }
        }


        public static void Turnirection3d(ref Vector3 velocity) 
        {
            float dy = velocity.y;
            velocity.y = 0;
            velocity = FindTurnDirection() * velocity;
            velocity.y = dy;
        }


        public static Vector3 GetTurnDirection3d(Vector3 velocity) 
        {
            float dy = velocity.y;
            velocity.y = 0;
            velocity = FindSkewDirection() * velocity;
            velocity.y = dy;
            return velocity;
        }


    }
}