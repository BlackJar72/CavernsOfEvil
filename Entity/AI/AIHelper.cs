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

    }
}