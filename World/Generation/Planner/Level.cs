using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil
{

    public class Level : MonoBehaviour
    {
        public GameObject protoRoom;
        public RoomComponents parts;
        public ulong seed = 0;
        public GameObject player;

        public PickupLists pickupLists;

        public GameObject startPad;
        public GameObject endPad;

        public SizeData size;
        public Episode episode;
        public DungeonTheme theme;
        public RoomTheme defaultRoomTheme;
        public StyleTheme style;

        public Xorshift random;
        public MapMatrix map;
        public RoomList rooms;
        public int roomCount;

        public GameObject testObject;

        public HubRoom[] nodes;
        public List<EntityMob> mobs;

        protected GameManager manager;

        public GameManager Manager { get { return manager; } }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            manager = GetComponent<GameManager>();
            PlanLevel();
            map.GlobalDoorFixer();
            AStarLevel tester = new AStarLevel(this, nodes[0].theRoom, nodes[1].theRoom);
            if(!tester.Seek())
                SceneManager.LoadScene("DungeonScene");
            CreateLevel();
            ScoreData.NewLevel(mobs.Count);  
        }


        public void QuitGame()
        {
            Application.Quit();
        }


        public bool UseDegree(Degree degree)
            => random.NextInt(10) < (int)degree;


        public void PlanLevel()
        {
            parts = protoRoom.GetComponent<RoomComponents>();
            random = GameData.Xrandom;
            if (random == null)
            {
                random = new Xorshift();
            }
            theme = episode.SelectTheme(random);
            defaultRoomTheme = new RoomTheme(theme, random);
            style = new StyleTheme(theme, random);
            size = GameData.LevelSizeData;

            map = new MapMatrix(size);
            rooms = new RoomList();
            roomCount = 0;

            nodes = new HubRoom[random.NextInt(size.minNodes, size.maxNodes)];

            MakeHubs();
            ConnectHubs();
            GrowthCycle();

            FixRoomContents();
        }


        public void CreateLevel()
        {
            BuildLevel();
            PlacePlayer();
            PlaceMobs();
            PlacePickups();

            BuildTests();
        }



        private void MakeHubs()
        {
            int i = 0;
            Vector2Int[] hubLocations = HubPlacer.FindLocations(random, size, nodes.Length);
            while (i < nodes.Length)
            {
                nodes[i] = new HubRoom(hubLocations[i].x, 0,
                        hubLocations[i].y, random, this);
                if (nodes[i].theRoom != null) ++i;
            }
        }


        private void ConnectHubs()
        {
            if (random.NextBool())
            {
                ConnectNodesDensely();
            }
            else
            {
                ConnectNodesSparcely();
            }
        }


        /**
         * This will attempt to connect all nodes based on the logic that 
         * if B can be reached from A, and C can be reached from B, then 
         * C can be reached from A (by going through B if no other route 
         * exists).
         * 
         * Specifically, it will connect the first node to one random other 
         * node, and then connect a random node already connected to the 
         * first with one that has not been connected, until all nodes have 
         * attempted a connects.  Note that this does not guarantee connections 
         * as the attempt to place a route between any two nodes may fail.
         * 
         * @throws Throwable
         */
        private void ConnectNodesSparcely()
        {
            HubRoom first, other;
            List<HubRoom> connected = new List<HubRoom>(nodes.Length),
                            disconnected = new List<HubRoom>(nodes.Length);
            connected.Add(nodes[0]);
            for (int i = 1; i < nodes.Length; i++)
            {
                disconnected.Add(nodes[i]);
            }
            while (disconnected.Count > 0)
            {
                if (rooms.Count >= size.maxRooms)
                {
                    return;
                }
                first = connected[random.NextInt(connected.Count)];
                other = disconnected[random.NextInt(disconnected.Count)];
                Route route = new Route(first, other);
                route.drawConnections(this);
                connected.Add(other);
                disconnected.Remove(other);
            }
        }


        /**
         * This will attempt to make one connects between every two pairs of 
         * nodes by first connecting the first node to all others directly, 
         * then each successive node to every node with a higher index.  As 
         * nodes with a lower index will already have attempted a connects 
         * this is not repeated.  Note that this does not guarantee connections 
         * as the attempt to place a route between any two nodes may fail.
         * 
         * @throws Throwable
         */
        private void ConnectNodesDensely()
        {
            HubRoom first, other;
            for (int i = 0; i < nodes.Length; i++)
            {
                first = nodes[i];
                for (int j = i + 1; j < nodes.Length; j++)
                {
                    other = nodes[j];
                    if (rooms.Count >= size.maxRooms)
                    {
                        return;
                    }
                    if (other != first)
                    {
                        Route route = new Route(first, other);
                        route.drawConnections(this);
                    }
                }
            }
        }


        private void GrowthCycle()
        {
            List<Room> planter = new List<Room>();
            for (int i = 1; i < rooms.TotalCount; i++)
            {
                planter.Add(rooms[i]);
            }
            List<Room> grower; ;
            bool doMore = true;
            do
            {
                doMore = false;
                grower = planter;
                Shuffler.Shuffle(grower, random);
                planter = new List<Room>();
                foreach(Room room in grower)
                {
                    if (rooms.Count >= size.maxRooms)
                    {
                        return;
                    }
                    if (room.PlantChildren(this))
                    {
                        doMore = true;
                    }
                }
            } while (doMore);
        }


        private void FixRoomContents()
        {
            // TODO: Floodfill checks! 

            foreach (Room room in rooms)
            {
                //PoolFixer.FixInRoom(this, room);
                DoorChecker.ProcessDoors1(this, room);
            }
            foreach (Room room in rooms)
            {
                DoorChecker.ProcessDoors2(this, room); 
            }
            foreach (Room room in rooms)
            {
                DoorChecker.ProcessDoors3(this, room);
            }
            DoorChecker.CheckConnectivity(this);
        }


        protected void BuildLevel()
        {
            //Cave.InitForLevel();
            for(int i = 1; i < rooms.TotalCount; i++)
            {
                if(rooms[i] is Cave)
                {
                    (rooms[i] as Cave).MakeFloorNCielingData(this);
                }
                rooms[i].BuildRoom(parts);
            }
        }


        private void PlaceMobs()
        {
            // FIXME: Replace with A* Project Pro
            //GetComponent<NavMeshSurface>().BuildNavMesh();
            /*for (int i = 2; i < rooms.TotalCount; i++)
            {
                MobPlacer.Process(rooms[i], this);
            }*/
            MobPlacer.ProcessConsistent(rooms, this);
        }


        private void PlacePickups()
        {
            int damageToKillAll = 0;
            foreach (EntityMob mob in mobs)
            {
                damageToKillAll += mob.Health.DamageToKill;
            }

            // Now we know the number of mobs, lets guestimate the amount of ammo needed
            float diffFactor = GameData.LevelDifficulty.levelDifficulty;
            int guessHits = Mathf.CeilToInt(damageToKillAll /
                (10 + (5 * diffFactor)));
            guessHits = Mathf.CeilToInt(guessHits * (1 + (GameData.LevelDifficulty.areaPerEncounter / 1000f)));
            // Now, from the amount of time taken to kill each mob and an estimate of average damage lets guess the
            // number of health potions needed.
            float guessInjury = guessHits * (20 - (10 * diffFactor)) * (GameData.LevelDifficulty.areaPerEncounter / 10000f);

            PickupPlacer.Process(this, guessHits, Mathf.CeilToInt(guessInjury));
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
            GameObject exit = Instantiate(endPad, new Vector3(endRoom.realX,
                endRoom.floorY, endRoom.realZ), endPad.transform.rotation);
        }


        public void SpawnGameMob(GameObject mob, int x, int z, bool randomRotation = true)
        {
            Vector3 location = new Vector3((float)x + 0.5f, map.GetFloorY(x, z), (float)z + 0.5f);
            Quaternion q = mob.transform.rotation;
            if(randomRotation) q *= Quaternion.Euler(0, random.NextFloat() * 360, 0);
            GameObject monster = Instantiate(mob, location, q);
            EntityMob entity = monster.GetComponent<EntityMob>();
            if ((entity == null) || InStartArea(entity)
                || monster.GetComponent<EntityMob>().CanSeeCollider(player)) Destroy(monster);
            else
            {
                entity.SetGameManager(manager);
                entity.SetGameLevel(this);
                mobs.Add(entity);
            }
        }


        public void SpawnGameMob(GameObject mob, int x, int z, Quaternion mobRotation)
        {
            Vector3 location = new Vector3((float)x + 0.5f, map.GetFloorY(x, z), (float)z + 0.5f);
            Quaternion q = mob.transform.rotation * mobRotation;
            Instantiate(mob, location, q);
            GameObject monster = Instantiate(mob, location, q);
            EntityMob entity = monster.GetComponent<EntityMob>();
            if ((entity == null) || InStartArea(entity) 
                || monster.GetComponent<EntityMob>().CanSeeCollider(player)) Destroy(monster);
            else
            {
                entity.SetGameManager(manager);
                entity.SetGameLevel(this);
                mobs.Add(entity);
            }
        }


        public void RemoveMob(EntityMob mob)
        {
            mobs.Remove(mob);
            Destroy(mob.gameObject);
        }


        private bool InStartArea(EntityMob mob)
        {
            return ((map.GetRoom((int)mob.transform.position.x, (int)mob.transform.position.z) < 2) 
                || (player.transform.position - mob.transform.position).magnitude < 16);
        }


        public void SpawnPickup(ItemEntry pickup, int x, int z)
        {
            Vector3 location = new Vector3(pickup.Pickup.TransformData.position.x + x + 0.5f, 
                                           pickup.Pickup.TransformData.position.y + map.GetFloorY(x, z),
                                           pickup.Pickup.TransformData.position.z + z + 0.5f);
            GameObject item = Instantiate(pickup.WorldPrefab,
                location, pickup.Pickup.TransformData.rotation);
        }


        public void PlaceTestObject(int x, int z)
        {
            Vector3 location = new Vector3(x + 0.5f,
                                           map.GetFloorY(x, z) + 0.5f,
                                           z + 0.5f);
            GameObject item = Instantiate(testObject,
                location, testObject.transform.rotation);
        }


        public void DeactivateMobs()
        {
            foreach(EntityMob mob in mobs) 
            {
                mob.ForgetPlayer();
                //mob.GetComponent<Animator>().enabled = false;
                NavMeshAgent agent = mob.GetComponent<NavMeshAgent>();
                if(agent != null) agent.enabled = false;
                mob.enabled = false;
            }
        }


        public int MobsKilled()
        {
            int output = 0;
            foreach(EntityMob mob in mobs)
            {
                if((mob == null) || mob.IsDead) output++;
            }
            return output;
        }


        /// <summary>
        /// A place to add testing/debug code for level gen.
        /// </summary>
        private void BuildTests()
        {/*
            for(int i = 0; i < map.Size.width; i++)
                for (int j = 0; j < map.Size.width; j++)
                {
                    if(map.GetAStared(i, j))
                    {
                        Vector3 location = new Vector3(i + 0.5f,
                                                       map.GetFloorY(i, j) + 0.5f,
                                                       j + 0.5f);
                        GameObject item = Instantiate(testObject,
                            location, testObject.transform.rotation);
                    }
                }
        */}



    }
}