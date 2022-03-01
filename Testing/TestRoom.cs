using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class TestRoom : Room
    {
        public TestRoom(int beginX, int endX, int beginZ, int endZ, int floorY, int ceilY, 
            Level dungeon, Room parent, Room previous, bool isBigRoom = false) 
            : base(beginX, endX, beginZ, endZ, floorY, ceilY, dungeon, parent, previous, isBigRoom)
        {}

        GameObject mobObject;
        EntityMob mob;


        public TestRoom MakeStartRoom(TestArena dungeon)
        {
            Vector2Int doordir = new Vector2Int(0, 1);
            AddDoor(dungeon, endX, midZ - 1, doordir);
            AddDoor(dungeon, endX, midZ, doordir);
            AddDoor(dungeon, endX, midZ + 1, doordir);
            return this;
        }


        public TestRoom MakeMobRoom(TestArena dungeon)
        {
            Vector2Int doordir = new Vector2Int(0, -1);
            AddDoor(dungeon, beginX, midZ - 1, doordir);
            AddDoor(dungeon, beginX, midZ, doordir);
            AddDoor(dungeon, beginX, midZ + 1, doordir);
            for(int i = -10; i < 11; i++)
            {
                dungeon.map.SetFloorY(floorY + 3, midX - 1, midZ + i);
                dungeon.map.SetFloorY(floorY + 3, midX, midZ + i);
                dungeon.map.SetFloorY(floorY + 3, midX + 1, midZ + i);
            }
            for (int i = -4; i < 5; i++)
            {
                dungeon.map.SetFloorY(floorY + 5, midX, midZ + i);
                dungeon.map.SetFloorY(floorY + 2, midX + i, midZ -11);
                dungeon.map.SetFloorY(floorY + 2, midX + i, midZ + 11);
            }
            for (int i = -6; i < 7; i++)
            {
                dungeon.map.SetFloorY(floorY + 1, ((midX + beginX) / 2) - 1, midZ + i);
                dungeon.map.SetFloorY(floorY + 1, ((midX + beginX) / 2), midZ + i);
                dungeon.map.SetFloorY(floorY + 1, ((midX + beginX) / 2) + 1, midZ + i);
            }
            return this;
        }


        public void PopulateSpawnRoom(TestArena dungeon)
        {
            GameObject.Instantiate(dungeon.stuffPrefab,
                new Vector3(realX, floorY + 0.5f, realZ), Quaternion.Euler(0, 90, 0));
        }


        public void PoulateMobRoom(TestArena dungeon)
        {
            mobObject = GameObject.Instantiate(dungeon.mobPrefab,
                new Vector3((midX + endX) / 2, floorY, realZ), Quaternion.Euler(0, 270, 0));
            mob = mobObject.GetComponent<EntityMob>();
            if (mob != null)
            {
                mob.SetGameManager(dungeon.Manager);
                mob.SetGameLevel(dungeon);
            }

        }

    }

}