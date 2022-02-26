using UnityEngine;


namespace DLD
{

    public partial class MapMatrix
    {
        SizeData size;
        public SizeData Size { get { return size; } }

        // Room identity
        readonly int[] room;
        readonly int[] rooms;
        readonly int[] type;
        // Basic Heights
        readonly int[] floorY;
        readonly int[] ceilY;
        readonly int[] nFloorY;
        readonly int[] nCeilY; // 0 = no door, > 0 is door height (at least 2)
        // Other data
        readonly int[] pools;   // 0 = no pool, > 0 is pool depth
        readonly int[] doors;
        readonly bool[] isWall;
        readonly bool[] isPillar;
        readonly bool[] astared;
        // Wokring Scratchpad
        readonly bool[] used;


        public MapMatrix(SizeData size)
        {
            this.size = size;
            rooms = new int[size.area];
            type = new int[size.area];
            floorY = new int[size.area];
            ceilY = new int[size.area];
            nFloorY = new int[size.area];
            nCeilY = new int[size.area];
            pools = new int[size.area];
            doors = new int[size.area];
            isWall = new bool[size.area];
            isPillar = new bool[size.area];
            astared = new bool[size.area];
            used = new bool[size.area];
            room = rooms;
        }



        // Getters
        public int GetRoom(int x, int z) => room[(z * size.width) + x];
        public int GetType(int x, int z) => type[(z * size.width) + x];
        public int GetFloorY(int x, int z) => floorY[(z * size.width) + x];
        public int GetCeilY(int x, int z) => ceilY[(z * size.width) + x];
        public int GetNFloorY(int x, int z) => nFloorY[(z * size.width) + x];
        public int GetNCeilY(int x, int z) => nCeilY[(z * size.width) + x];
        public int GetDoorway(int x, int z) => doors[(z * size.width) + x];
        public int GetPool(int x, int z) => pools[(z * size.width) + x];
        public bool GetWall(int x, int z) => isWall[(z * size.width) + x];
        public bool GetPillar(int x, int z) => isPillar[(z * size.width) + x];
        public bool GetAStared(int x, int z) => astared[(z * size.width) + x];
        public bool GetUsed(int x, int z) => used[(z * size.width) + x];
        public bool GetPassable(int x, int z) => !(isWall[(z * size.width) + x]
                    && !isPillar[(z * size.width) + x]) 
                    || (doors[(z * size.width) + x] > 2);
        public bool GetPassableAndSafe(int x, int z) => ((!isWall[(z * size.width) + x]
                    && !isPillar[(z * size.width) + x]) || (doors[(z * size.width) + x] > 2)) 
                    && (pools[(z * size.width) + x] < 1);
        public bool GetGoodMobSpawn(int x, int z) => !isWall[(z * size.width) + x]
                    && !isPillar[(z * size.width) + x] && (pools[(z * size.width) + x] < 1)
                    && (rooms[(z * size.width) + x] > 0) && !astared[(z * size.width) + x];


        // Setters
        public void SetRoom(int value, int x, int z) => room[(z * size.width) + x] = value;
        public void SetType(int value, int x, int z) => type[(z * size.width) + x] = value;
        public void SetFloorY(int value, int x, int z) => floorY[(z * size.width) + x] = value;
        public void SetCeilY(int value, int x, int z) => ceilY[(z * size.width) + x] = value;
        public void SetNFloorY(int value, int x, int z) => nFloorY[(z * size.width) + x] = value;
        public void SetNCeilY(int value, int x, int z) => nCeilY[(z * size.width) + x] = value;
        public void SetPool(int value, int x, int z) => pools[(z * size.width) + x] = value;
        public void SetDoorway(int value, int x, int z) => doors[(z * size.width) + x] = value;
        public void SetWall(bool value, int x, int z)
        {
            isPillar[(z * size.width) + x] = false;
            isWall[(z * size.width) + x] = value;
        }
        public void SetWall(int x, int z)
        {
            isPillar[(z * size.width) + x] = false;
            isWall[(z * size.width) + x] = true;
        }
        public void UnSetWall(int x, int z)
        {
            isPillar[(z * size.width) + x] = false;
            isWall[(z * size.width) + x] = false;
        }
        public void SetPillar(bool value, int x, int z)
        {
            isWall[(z * size.width) + x] = false;
            isPillar[(z * size.width) + x] = value;
        }
        public void SetPillar(int x, int z)
        {
            isWall[(z * size.width) + x] = false;
            isPillar[(z * size.width) + x] = true;
        }
        public void UnSetPillar(int x, int z)
        {
            isWall[(z * size.width) + x] = false;
            isPillar[(z * size.width) + x] = false;
        }
        public void SetAstared(bool value, int x, int z) => astared[(z * size.width) + x] = value;
        public void SetAstared(int x, int z) => astared[(z * size.width) + x] = true;
        public void SetUsed(bool value, int x, int z) => used[(z * size.width) + x] = value;
        public void SetUsed(int x, int z) => used[(z * size.width) + x] = true;



        // Special Data Readers
        public bool OnFloor(Vector3 pos)
        {
            if((pos.x < 0) || (pos.x >= size.width) || (pos.z < 0) || (pos.z >= size.width))
            {
                return false;
            }
            return (pos.y - (float)floorY[((int)pos.z * size.width) + (int)pos.z]) < 0.1f;
        }
        public bool InPool(Vector3 pos)
        {
            if ((pos.x < 0) || (pos.x >= size.width) || (pos.z < 0) || (pos.z >= size.width))
            {
                return false;
            }
            return (pools[((int)pos.z * size.width) + (int)pos.z] > 0) &&
                    ((pools[((int)pos.z * size.width) + (int)pos.z] 
                        + floorY[((int)pos.z * size.width) 
                        + (int)pos.z]) > pos.y);
        }
        public Coords GetCoords(int index) => new Coords(index % size.width, index / size.width);


        public bool IsValidLocation(Vector3 pos)
        {
            if ((pos.x < 0) || (pos.x >= size.width) || (pos.z < 0) || (pos.z >= size.width))
            {
                return false;
            }
            int x = (int)pos.x, z = (int)pos.z;
            return ((GetRoom(x, z) > 0) && GetPassable(x, z));
        }


        public bool IsValidLocation(Vector3 pos, float height)
        {
            if ((pos.x < 0) || (pos.x >= size.width) || (pos.z < 0) || (pos.z >= size.width))
            {
                return false;
            }
            int x = (int)pos.x, z = (int)pos.z;
            return ((GetRoom(x, z) > 0) && GetPassable(x, z) 
                && ((GetCeilY(x, z) - GetFloorY(x, z)) > height));
        }


        public bool GetAstarAvailable(int x, int z, int from, int to) 
        {  
            return !used[(z * size.width) + x] 
                && ((room[(z * size.width) + x] == from) 
                    || ((room[(z * size.width) + x] == to)));
        }


        public bool GetLevelAstarAvailable(int x, int z)
        {
            return !used[(z * size.width) + x]
                && ((room[(z * size.width) + x] != 0)
                && (!isWall[(z * size.width) + x] 
                    || (doors[(z * size.width) + x] > 1)));
        }


        public StepData GetStepData(Vector3 location, Level dungeon, 
                EntityHealth health, ref float hurtTime)
        {
            StepData data = new StepData();
            Vector2Int tile = new Vector2Int((int)location.x, (int)location.z);
            int floorheight = GetFloorY(tile.x, tile.y);
            int pooldepth = GetPool(tile.x, tile.y);
            data.roomID = GetRoom(tile.x, tile.y);
            data.onGround = location.y < (floorheight + 0.1f);
            data.inLiquid = (pooldepth > 0)
                && (location.y < (floorheight + pooldepth));
            Room room = dungeon.rooms[data.roomID];

            // Is there a more efficient way to do this, without so many conditionals?
            if (data.inLiquid)
            {
                data.floorEffect = room.theme.liquidSubstance.Effect;
                if((Time.time > hurtTime) && (room.theme.liquidSubstance.Damage > 0))
                {
                    hurtTime = Time.time + 1.0f;
                    health.BeHitByAttack(room.theme.liquidSubstance.Damage, room.theme.liquidSubstance.TypeOfDamage, null);
                }
            }
            else if (data.onGround && (room != null))
            {
                data.floorEffect = room.theme.floorSubstance.Effect;
            }
            else data.floorEffect = FloorEffect.none;

            return data;
        }


    }


}