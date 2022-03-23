using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil
{

    public class TestArena : Level
    {
        public GameObject mobPrefab;
        public GameObject stuffPrefab;

        private TestRoom spawnRoom;
        private TestRoom mobRoom;


        protected override void Start()
        {
            GameData.Init(seed.ToString(), DifficultySettings.norm);
            manager = GetComponent<GameManager>();
            Setup();
            Plan();
            BuildLevel(); 
            GetComponent<NavMeshSurface>().BuildNavMesh();
            PlacePlayer();
            spawnRoom.PopulateSpawnRoom(this);
            mobRoom.PoulateMobRoom(this);
            manager.Start();
        }


        private void Setup()
        {
            parts = protoRoom.GetComponent<RoomComponents>();
            random = new Xorshift(seed);
            if (seed == 0) random = new Xorshift();
            else random = new Xorshift(seed);
            defaultRoomTheme = new RoomTheme(theme, random);
            style = new StyleTheme(theme, random);
            size = SizeTable.huge;
            map = new MapMatrix(size);
            rooms = new RoomList();
            roomCount = 0;
        }


        private void Plan()
        {
            nodes = new HubRoom[2];
            spawnRoom = new TestRoom(40, 72, 48, 80, 0, 8, this, null, null, false).MakeStartRoom(this);
            mobRoom = new TestRoom(72, 136, 32, 108, 0, 16, this, null, null, false).MakeMobRoom(this);
            nodes[0] = new HubRoom(spawnRoom);
            nodes[1] = new HubRoom(mobRoom);
        }


        protected virtual void PlacePlayer()
        {
            Room startRoom = nodes[0].theRoom;
            Instantiate(startPad, new Vector3(startRoom.realX,
                startRoom.floorY, startRoom.realZ), startPad.transform.rotation);
            player.transform.position = new Vector3(startRoom.realX,
                startRoom.floorY + 0.2f, startRoom.realZ);
            player.GetComponent<MovePlayer>().SetLevel(this);
            player.SetActive(true);

            Room endRoom = nodes[1].theRoom;
            GameObject exit = Instantiate(endPad, new Vector3(endRoom.realX +128,
                endRoom.floorY + 128, endRoom.realZ), endPad.transform.rotation);
        }


    }

}