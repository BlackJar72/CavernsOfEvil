using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public interface IRoutingAgent
    {
        public abstract bool GoodLocation(Vector2 location);
        public abstract bool GoodLocation(Vector3 location);

        /// <summary>
        /// Height above the floor.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract float GroundHeight(Vector2 location);

        /// <summary>
        /// Height about the floor or liquid surface.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract float SurfaceHeight(Vector2 location);

        public abstract bool ValidStep(Vector3 from, ref Vector3 to);
        public abstract bool GoodStep(Vector3 from, ref Vector3 to);
        public abstract float StepCost(Vector3 from, Vector3 to);
        public abstract List<Vector2Int> FindRoute(Vector3 from, Vector3 to);
        public abstract AIStep GetMoveDirection();
        public abstract void SetDestination(Vector3 destination);

    }


    public struct AIStep
    {
        public Vector3 direction;
        public bool jump;

        public AIStep(Vector3 dir, bool jump)
        {
            direction = dir;
            this.jump = jump;
        }

        public AIStep(float x, float y, float z, bool jump)
        {
            direction = new Vector3(x, y, z);
            this.jump = jump;
        }

        public AIStep(Vector3 dir)
        {
            direction = dir;
            jump = false;
        }

        public AIStep(float x, float y, float z)
        {
            direction = new Vector3(x, y, z);
            jump = false;
        }
    }

}