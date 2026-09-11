using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class DungeonManager : MonoBehaviour
    {
        public static DungeonManager instance;

        private MapMatrix map;

        public DungeonManager Instance => instance;
        public Level Dungeon => Level.Instance;


        private void Awake()
        {
          instance = this;  
        }


        public void Start()
        {
            instance = this;
            map = Level.Instance.map;
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
            Room room = Level.Instance.rooms[data.roomID];

            // Is the a more efficient way to do this, without so many conditionals?
            if (data.inLiquid) data.floorEffect = room.theme.liquidSubstance.Effect;
            else if (data.onGround) data.floorEffect = room.theme.floorSubstance.Effect;
            else data.floorEffect = FloorEffect.none;

            return data;
        }


        #region Routing and AI
        #region Location Equality Tests
        public bool SameVoxel(Vector3 a, Vector3 b) => (((int)a.x == (int)b.x) 
                                                    && ((int)a.y == (int)b.y) 
                                                    && ((int)a.z == (int)b.z));
        public bool SameTile(Vector3 a, Vector3 b) => (((int)a.x == (int)b.x) && ((int)a.z == (int)b.z));
        public bool SameTile(Vector2 a, Vector2 b) => (((int)a.x == (int)b.x) && ((int)a.y == (int)b.y));
        public bool SameVoxel(Vector3Int a, Vector3Int b) => ((a.x == b.x) && (a.y == b.y) && (a.z == b.z));
        public bool SameTile(Vector3Int a, Vector3Int b) => ((a.x == b.x) && (a.z == b.z));
        public bool SameTile(Vector2Int a, Vector2Int b) => ((a.x == b.x) && (a.y == b.y));
        #endregion

        #region AI Location Checking 
        public bool LocationSafe(Vector3 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            return !((location.y < (map.GetFloorY(tile.x, tile.y)
                    + map.GetPool(tile.x, tile.y) + 0.01f))
                    && (Level.Instance.rooms[map.GetRoom(tile.x, tile.y)].theme.liquidSubstance.Damage <=0));
        }


        public bool LocationSafe(Vector3 location, Vector2Int tile)
        {
            return !((location.y < (map.GetFloorY(tile.x, tile.y)
                    + map.GetPool(tile.x, tile.y)) && (map.GetPool(tile.x, tile.y) > 0)));
        }


        public bool LocationGoodAI(Vector3 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            return !(map.GetBlocked(tile.x, tile.y) || map.GetPillar(tile.x, tile.y)
                && ((map.GetFloorY(tile.x, tile.y) + map.GetPool(tile.x, tile.y)) < location.y));
        }


        public bool LocationGoodAI(Vector2 location)
        {
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.y);
            return !(map.GetBlocked(tile.x, tile.y) || map.GetPillar(tile.x, tile.y)
                && (map.GetPool(tile.x, tile.y) <= 0.0f));
        }


        public StepDataAI GetAIDataForGround(Vector3 start, Vector3 end, EntityMob mob)
        {
            StepDataAI output = new StepDataAI();
            Vector2Int endTile = new Vector2Int((int)end.x, (int)end.z);
            if (SameVoxel(start, end))
            {
                output.passable = true;
                output.reachable = true;
                output.reversable = true;
                output.safe = LocationSafe(start, endTile);
            }
            else
            {
                Vector2Int startTile = new Vector2Int((int)start.x, (int)start.z);
                float heightDiff = Mathf.Abs(end.y - start.y);
                output.passable = map.GetPassable(endTile.x, endTile.y);
                output.reachable = output.reversable = (heightDiff < 0.5f);
                output.safe = LocationSafe(end, endTile);
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

        #region Reachability Testing
        /*
        // TODO: Special Pathing
        //
        // Special pathings based on A* (probably) or BFS to help with certain features.
        //
        // These can be done using a new boolean table, like that used for quality control 
        // pathing, but separate and different from it.  This can be used to keep track of 
        // already used (selected, tested, included, rejected) tiles.  When a tile is added 
        // to the priority queue it can also be added to a list of used tile (both as Vector2Int) 
        // and the bool for those coordinates set to true (used).  The bool[,] can be test 
        // while running the algorithm (much faster then searching a list), while the list 
        // can be iterated once at the end to set all bools back to false then cleared (a 
        // clean-up step). This prevents allocation of a huge 2D array with every run of the 
        // algorithm, while still allow fast checks and a fast simple clean-up.
        //
        // For specific use cases:
        //
        // * For AoE attacks (notable fireballs from the wand or fire), any tile can be added 
        // as long as it (1) is not a wall or pillar and (2) is in range.  If a path can be 
        // found from the AoE origin to the target it is hit, otherwise it is not.  This should 
        // not run very long, as AoE ranges are small, so it should either succeed or run out 
        // of valid tiles before many iteration.  This would be run after the current test for 
        // in room and line of sight.  Because actual number of tiles traversed is not important 
        // (as it must stay in range anyway) no special handling of diagonals is needed and 
        // A* Manhattan can be used while only considering axis-aligned steps.
        //
        // * For sound propigation range should be half (or less) that of the full straight line 
        // range (mostly for balance reasons, so we aren't waking whole unexplored rooms).  Total 
        // distance travelled might also be needed, and both for better modelling (realism) and
        // for balance (nerfing).  This might mean A* Euclidean and handling of diagnals.  Diagnals
        // can be considered along with adjacent tiles.  
        //     1. That is, check the for tiles rachable by axis aligned movement.
        //     2. If the tile is added, add those to its left and right if they also qualify.
        //         * That is left and right relative to the direction for the current tile.
        //     3. Proceed through all four non-diagnoally adjacent tiles.
        // Hopefully, this is not too expensive as these are longer distances than those use for 
        // AoE's and would need to be done more often.  However, sticking with the system planned
        // for AoE might still be better, as further nerfing enemy hearing range (dianals being
        // effectively treat as a distance of 2 instead of the square root of 2) might actually
        // be a good thing, along side the simplicity and likely greater efficiency.
        //
        // * Also, for retreating archers, it might be good to find a place that is the farthest 
        // reachable in 10 steps, using only those a mob could take (excluded height change, pool, 
        // etc., besides the usual walls, used, and out of range).  This might then be achievable 
        // by using A* but maximizing rather than minimizing the distance (perhaps by making 
        // distance negative to longer is a lower number).  If it works and was not to demanding 
        // on processing power it would be an improvement over the current system the has them 
        // path to a location 10 m in the opposite direction as the player (and which can be off 
        // the navmesh resulting a the archer just standing there).
        //
        // Also, if the search succeeds, all attempted steps could be marked as connected so that
        // finding them again (tests for other targets) could treat fingind them as success.
        // Likewise, if the search fails (having exhausted all reachable tiles), the tested tiles
        // could be marked as disconnect so that finding on (by having it as the start) can be
        // immediately determined a failure.  This could speed things up a lot of multiple targets
        // need to be searched for out source.  Perhaps an enum could be used as a data type for
        // map table (2D array).  This would probably need to encode four states: Unused, Considering,
        // Success, Failure. A 2D array of this would take the place of the bool[,] mentioned above.
        //
        // Not sure when or if I'll get around to this, but it would likely be a huge improvement.
        //
        // Then, better sound propigation / hearing modelling might need to be balanced by a buff 
        // to the player character.  Already the game seems harder than it did, possibly because 
        // preventing mobs from getting stuck in corners leads to more enemies reaching the player 
        // faster! 
        // */


        public bool TileInBounds(Vector2Int tile) => map.GetInBounds(tile);
        public bool TileInBounds(Vector3Int tile) => map.GetInBounds(tile);


        public bool TileInRange(Vector2Int origin, Vector2Int candidate, int range)
        {
            int xDiff = candidate.x - origin.x;
            int yDiff = candidate.y - origin.y;
            return ((xDiff * xDiff) + (yDiff * yDiff)) <= (range * range);
        }


        public bool TileEnergyPassable(Vector2Int candidate) => map.GetPassable(candidate.x, candidate.y); 


        
        public class TilePather
        {
            // This needs to be a reference type (class) so the same StepTile object can be
            // in multiple places at once, specifically, the MapMatrix, PriorityQueue, and
            // List of in use steps.
            public class StepTile : System.IComparable<StepTile>
            {
                public int traversed, heuristic;
                public Vector2Int location;
                public int Cost => traversed + heuristic;
                public int CompareTo(StepTile other) => Cost.CompareTo(other.Cost);
                public static bool operator >(StepTile a, StepTile b) => a.CompareTo(b) > 0;
                public static bool operator <(StepTile a, StepTile b) => a.CompareTo(b) < 0;
                public static bool operator >=(StepTile a, StepTile b) => a.CompareTo(b) >= 0;
                public static bool operator <=(StepTile a, StepTile b) => a.CompareTo(b) <= 0;
                public static bool operator ==(StepTile a, StepTile b) => a.CompareTo(b) == 0;
                public static bool operator !=(StepTile a, StepTile b) => a.CompareTo(b) != 0;
                public override bool Equals(object obj) => base.Equals(obj);
                public override int GetHashCode() => base.GetHashCode();
            }

            private Vector2 origin;
            private Vector2 destination;
            private PriorityQueue<StepTile> priority;
            private List<StepTile> considred;

        }




        # endregion




        #endregion

    }

}
