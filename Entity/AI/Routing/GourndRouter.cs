using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class GourndRouter : MonoBehaviour, IRoutingAgent
    {
        protected EntityMob owner;
        protected GameManager manager;
        protected Level level;
        protected StepDataAI stepDataAI;
        protected Vector3 destination;
        protected Vector3 direction;
        protected List<Vector2Int> route;
        protected int routeStep;


        protected StepDataAI GetStepData(Vector3 from, Vector3 to)
        {
            return stepDataAI = manager.GetAIDataForGround(from, to, owner);
        }


        public virtual bool GoodLocation(Vector2 location)
        {
            return manager.LocationGoodAI(location);
        }


        public virtual bool GoodLocation(Vector3 location)
        {
            return manager.LocationGoodAI(location);
        }


        /// <summary>
        /// Height above the floor.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual float GroundHeight(Vector2 location)
        {
            return level.map.GetFloorY((int)location.x, (int)location.y);
        }

        /// <summary>
        /// Height about the floor or liquid surface.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual float SurfaceHeight(Vector2 location)
        {
            int x = (int)location.x, z = (int)location.y;
            return level.map.GetFloorY(x, z) + level.map.GetPool(x, z);
        }


        public virtual bool ValidStep(Vector3 from, ref Vector3 to)
        {
            return GetStepData(from, to).Valid;
        }


        public virtual bool GoodStep(Vector3 from, ref Vector3 to)
        {
            return GetStepData(from, to).Desireable;
        }


        public virtual float StepCost(Vector3 from, Vector3 to)
        {
            float baseCost = 1f; // FIXME: This should be based on surface traversed
            float xdist = from.x - to.x;
            float zdist = from.z - to.z;
            return (Mathf.Sqrt((xdist * xdist) + (zdist * zdist)) * baseCost)
                + Mathf.Abs(to.y - from.y);
        }


        public virtual List<Vector2Int> FindRoute(Vector3 from, Vector3 to)
        {
            //FIXME/TODO: Does nothing; this need to calculate the route (primary 
            //            function), then return it.
            return route;
        }


        public virtual AIStep GetMoveDirection()
        {
            //TODO/FIXME: Stand-in, for now, really does nothing
            return new AIStep(direction);
        }


        public virtual void SetDestination(Vector3 destination)
        {
            this.destination = destination;
        }
    }

}