using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class GameManager : MonoBehaviour
    {
        Level level;

        private MapMatrix map;

        private Player Player;
        private List<Entity> entities;
        private List<Player> playerList;

        // Entities to heal
        private List<Entity> injured; 
        // groups that update AI together
        private List<EntityMob>[] aiBatches = new List<EntityMob>[10];


        public void Start()
        {
            level = GetComponent<Level>();
            map = level.map;
        }


        public void SetLevel(Level level)
        {
            this.level = level;
            map = level.map;
        }


        public StepData GetStepData(Vector3 location)
        {
            StepData data = new StepData();
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            int floorheight = map.GetFloorY(tile.x, tile.y);
            int pooldepth = map.GetPool(tile.x, tile.y);
            data.roomID = map.GetRoom(tile.x, tile.y);
            data.onGround = location.y  < (floorheight + 0.01f);
            data.inLiquid = (pooldepth > 0) 
                && (location.y < (floorheight + pooldepth - 0.05f));
            Room room = level.rooms[data.roomID];

            // Is the a more efficient way to do this, without so many conditionals?
            if (data.inLiquid) data.floorEffect = room.theme.liquidSubstance.Effect;
            else if (data.onGround) data.floorEffect = room.theme.floorSubstance.Effect;
            else data.floorEffect = FloorEffect.none;

            return data;
        }

        #region Routing and AI
        #region Location Equality Tests
        public bool SameVoxel(Vector3 a, Vector3 b)
        {
            return (((int)a.x == (int)b.x) && ((int)a.y == (int)b.y) && ((int)a.z == (int)b.z));
        }


        public bool SameTile(Vector3 a, Vector3 b)
        {
            return (((int)a.x == (int)b.x) && ((int)a.z == (int)b.z));
        }


        public bool SameTile(Vector2 a, Vector2 b)
        {
            return (((int)a.x == (int)b.x) && ((int)a.y == (int)b.y));
        }


        public bool SameVoxel(Vector3Int a, Vector3Int b)
        {
            return ((a.x == b.x) && (a.y == b.y) && (a.z == b.z));
        }


        public bool SameTile(Vector3Int a, Vector3Int b)
        {
            return ((a.x == b.x) && (a.z == b.z));
        }


        public bool SameTile(Vector2Int a, Vector2Int b)
        {
            return ((a.x == b.x) && (a.y == b.y));
        }
        #endregion


        public bool LocationSafe(Vector3 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            return !((location.y < (map.GetFloorY(tile.x, tile.y)
                    + map.GetPool(tile.x, tile.y) + 0.01f))
                    && (level.rooms[map.GetRoom(tile.x, tile.y)].theme.liquidSubstance.Damage <=0));
        }


        public bool LocationSafe(Vector3 location, Vector2Int tile)
        {
            return !((map.GetPool(tile.x, tile.y) > 0) && (location.y < (map.GetFloorY(tile.x, tile.y)
                    + map.GetPool(tile.x, tile.y)))
                    && (level.rooms[map.GetRoom(tile.x, tile.y)].theme.liquidSubstance.Damage <= 0));
        }


        public bool LocationSafeGround(Vector3 location, Vector2Int tile)
        {
            return !((map.GetPool(tile.x, tile.y) > 0) 
                    && (level.rooms[map.GetRoom(tile.x, tile.y)].theme.liquidSubstance.Damage <= 0));
        }


        public bool LocationGoodAI(Vector3 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            return !(map.GetWall(tile.x, tile.y) || map.GetPillar(tile.x, tile.y)
                && ((map.GetFloorY(tile.x, tile.y) + map.GetPool(tile.x, tile.y)) < location.y));
        }


        public bool LocationGoodAI(Vector2 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.y);
            return !(map.GetWall(tile.x, tile.y) || map.GetPillar(tile.x, tile.y)
                && (map.GetPool(tile.x, tile.y) <= 0.0f));
        }


        public StepDataAI GetAIDataForGround(Vector3 start, Vector3 end, EntityMob mob)
        {
            StepDataAI output = new StepDataAI();
            Vector2Int endTile = new Vector2Int((int)end.x, (int)end.z);
            Vector2Int startTile = new Vector2Int((int)start.x, (int)start.z);
            if (SameVoxel(start, end))
            {
                output.passable = true;
                output.reachable = true;
                output.reversable = true;
                output.safe = LocationSafe(start, endTile);
                output.height = map.GetFloorY(endTile.x, endTile.y);
                output.deltay = 0;
            }
            else
            {
                end.y = map.GetFloorY(endTile.x, endTile.y);
                output.height = map.GetFloorY(endTile.x, endTile.y);
                output.deltay = output.height - map.GetFloorY(startTile.x, startTile.y);
                float verticleSpace = map.GetCeilY(endTile.x, endTile.y) - end.y;
                output.passable = map.GetPassable(endTile.x, endTile.y) 
                                  && (verticleSpace > mob.GetCollider().bounds.size.y);
                output.reachable = output.deltay < 0.25f;//output.deltay <= 1.25f;
                output.reversable = output.deltay > -0.25f;//output.deltay > -1f;
                output.safe = !(map.GetPool(endTile.x, endTile.y) > 0) || (map.GetPool(startTile.x, startTile.y) > 0);
            }
            return output;
        }


        public StepDataAI GetAIDataForFlying(Vector3 start, Vector3 end, EntityMob mob)
        {
            StepDataAI output = new StepDataAI();
            Vector2Int endTile = new Vector2Int((int)end.x, (int)end.z);
            output.reachable = true;
            output.reversable = true;
            if (SameVoxel(start, end))
            {
                output.passable = true;
                output.safe = LocationSafe(start, endTile);
            }
            else
            {
                Vector2Int startTile = new Vector2Int((int)start.x, (int)start.z);
                end.y = Mathf.Max(map.GetFloorY(endTile.x, endTile.y), end.y);
                float heightDiff = end.y - map.GetFloorY(startTile.x, startTile.y);
                float verticleSpace = map.GetCeilY(endTile.x, endTile.y) - end.y;
                output.passable = map.GetPassable(endTile.x, endTile.y)
                                  && (verticleSpace > mob.GetCollider().bounds.size.y);
                output.safe = LocationSafe(end, endTile);
            }
            return output;
        }


        public bool InSameRoom(Vector3 a, Vector3 b)
        {
            return map.GetRoom((int)a.x, (int)a.z) == map.GetRoom((int)b.x, (int)b.z);
        }



        #endregion



    }

}
