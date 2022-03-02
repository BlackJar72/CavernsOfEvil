using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public static class AIHelper
    {
        public static Quaternion FindTurnDirection()
        {
            switch (Random.Range(0, 3))
            {
                case 0:
                    return Quaternion.identity;
                case 1:
                    return Quaternion.Euler(0, -30, 0);
                case 2:
                    return Quaternion.Euler(0, 30, 0);
                case 3:
                default:
                    return Quaternion.Euler(0, Random.Range(-45f, 45f), 0);
            }
        }


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
    }

}