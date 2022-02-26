using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nito.Collections;

namespace DLD
{

    public partial class Room
    {
        public static readonly Room roomNull = null;

		public int id;

		public readonly int beginX, midX, endX, beginZ, midZ, endZ, floorY, ceilY, y;
		public float realX, realZ;
		public bool hasWholePattern;
		public RoomTheme theme;
		public bool isNode, isSubroom, hasEntrance, hasExit, isBigRoom;
		public Symmetry sym;
		public SymmetryVar symdat;
		public int orientation;
		public bool xFlip, zFlip;
		public List<RoomSeed> childSeeds = new List<RoomSeed>();
		public List<Doorway> doors;
		public List<DoorQueue> connections;
		public List<Doorway> topDoors;
		public Doorway midpoint; // not really a door but used as one at times
		public List<Tile> spawnableSpots;
		public RoomAreas areas;

		public int GeometricArea => (endX - beginX) * (endZ - beginZ);


		public Room(int beginX, int endX, int beginZ, int endZ, int floorY, int ceilY,
                    Level dungeon, Room parent, Room previous, bool isBigRoom = false)
		{
			this.dungeon = dungeon;
			id = dungeon.rooms.AddRoomFast(this);
			isNode = (previous == null);
			isSubroom = (parent != null);
			this.isBigRoom = isBigRoom;

			if (isNode /*|| !dungeon.theme.IsCaves*/)
			{
				theme = new RoomTheme(dungeon.theme, dungeon.random);
			}
			else
			{
				theme = new RoomTheme(dungeon.theme, previous, dungeon, dungeon.random);
			}
			map = dungeon.map;
			sym = SymTable.GetSymmetry(dungeon);
			symdat = SymTable.Data(sym);
			orientation = dungeon.random.NextInt(4);
			xFlip = dungeon.random.NextBool();
			zFlip = dungeon.random.NextBool();

			childSeeds = new List<RoomSeed>();
			doors = new List<Doorway>();
			topDoors = new List<Doorway>();
			connections = new List<DoorQueue>();
			spawnableSpots = new List<Tile>();

			this.beginX = beginX;
			this.endX = endX;
			this.beginZ = beginZ;
			this.endZ = endZ;
			this.floorY = floorY;
			this.ceilY = ceilY;

			midX = beginX + ((endX - beginX) / 2);
			midZ = beginZ + ((endZ - beginZ) / 2);

			realX = (((float)(endX - beginX)) / 2.0f) + (float)beginX + 1.0f;
			realZ = (((float)(endZ - beginZ)) / 2.0f) + (float)beginZ + 1.0f;

			for (int i = beginX; i <= endX; i++)
				for (int j = beginZ; j <= endZ; j++)
				{
					if (dungeon.map.GetRoom(i, j) == 0)
					{
						dungeon.map.SetRoom(id, i, j);
						dungeon.map.SetCeilY(ceilY, i, j);
						dungeon.map.SetFloorY(floorY, i, j);
					}
					else
					{
						dungeon.map.SetCeilY(Mathf.Max(ceilY, dungeon.map.GetCeilY(i, j)), i, j);
						dungeon.map.SetFloorY(Mathf.Min(floorY, dungeon.map.GetFloorY(i, j)), i, j);
					}
				}

			for (int i = beginX; i <= endX; i++)
			{
				dungeon.map.SetWall(i, beginZ);
				dungeon.map.SetWall(i, endZ);
			}
			for (int i = beginZ; i <= endZ; i++)
			{
				dungeon.map.SetWall(beginX, i);
				dungeon.map.SetWall(endX, i);
			}			
		}


		public virtual Room Plan(Level dungeon, Room parent)
		{
            //if(id == 1) return this;
			Doorways(dungeon);

			if (!dungeon.UseDegree(dungeon.style.complexity) && !isNode)
			{
				hasWholePattern = true;
				if (dungeon.UseDegree(dungeon.style.pools))
				{
					Walkway(dungeon);
				}
				else if (dungeon.UseDegree(dungeon.style.complexity)
					  || dungeon.UseDegree(dungeon.style.symmetry))
				{
					Cutin(dungeon);
				}
			}
			else AddFeatures(dungeon);
            return this;
		}


		public Room FixHub()
        {
			int xrange = Mathf.Min(Mathf.CeilToInt((float)(endX - beginX) / 4), 5);
			int zrange = Mathf.Min(Mathf.CeilToInt((float)(endZ - beginZ) / 4), 5);
			for(int i = midX - xrange; i <= (midX + xrange); i++)
				for (int j = midZ - zrange; j <= (midZ + zrange); j++)
                {
					map.SetRoom(id, i, j);
					map.SetFloorY(floorY, i, j);
					map.UnSetWall(i, j);
					map.SetPool(0, i, j);
					map.SetDoorway(0, i, j);
                }
			return this;
		}



		/**
		 * Fills a room with a "liquid" and then adds a walkway through it; 
		 * this is used for rooms with a whole room pattern. 
		 * 
		 * @param dungeon
		 */
		private void Walkway(Level dungeon)
		{
			int depth = 1;
			ShapeData shape = ShapeDataTables.wholeShape(sym, dungeon.random);
			for (int i = beginX + 1; i < endX; i++)
				for (int j = beginZ + 1; j < endZ; j++)
				{
					dungeon.map.SetFloorY(floorY - depth, i, j);
					dungeon.map.SetPool(depth, i, j);
				}
			shape.family[orientation].drawWalkway(dungeon, this, realX, realZ,
					(byte)(endX - beginX + 1), (byte)(endZ - beginZ + 1), xFlip, zFlip);
		}


		/**
		 * Fills the room with walls, then opens up a passage; used for rooms 
		 * with a whole room pattern.
		 * 
		 * @param dungeon
		 */
		private void Cutin(Level dungeon)
		{
			ShapeData shape = ShapeDataTables.wholeShape(sym, dungeon.random);
			for (int i = beginX; i <= endX; i++)
				for (int j = beginZ; j <= endZ; j++)
				{
					dungeon.map.SetWall(i, j);
				}
			shape.family[orientation].drawCutin(dungeon, this, realX, realZ,
					(byte)(endX - beginX - 1), (byte)(endZ - beginZ - 1), xFlip, zFlip);
		}


		/**
		 * Adds features other than chests and spawns to the room to rooms that
		 * lack a whole room pattern.  This is called by plan room.
		 * 
		 * It will try to at a number of features based on dungeon complexity. 
		 * On each attempt it will check each possible feature once; if a feature
		 * is selected to add it the attempt ends and the next begins.  To avoid 
		 * a universal bias based on checking order, all features types are added 
		 * to a list and shuffled once, giving each room its own set of biases 
		 * that can be thought of as the room character or a room equivalent to 
		 * the dungeons wide theme-based probabilities.
		 * 
		 * @param dungeon
		 */
		public void AddFeatures(Level dungeon)
		{
			if ((int)dungeon.style.complexity <= 0) return;
			List<FeatureAdder> features = new List<FeatureAdder>();
			features.Add(new PlatformAdder(dungeon.style.vertical));
            if(dungeon.random.NextInt((int)dungeon.style.vertical + 1)
                    > dungeon.random.NextInt((int)dungeon.style.pools + 1))
			{
                features.Add(new PitAdder(dungeon.style.vertical));
            }
            else 
            {
                features.Add(new PoolAdder(dungeon.style.pools));
            }
			{
				features.Add(new CutoutAdder(dungeon.style.complexity));
				features.Add(new PillarAdder(dungeon.style.pillars));
			}
			features.Add(new IslandRoomAdder(dungeon.style.islands));
			Shuffler.Shuffle(features, dungeon.random);
			int tries = ((endX - beginX + endZ - beginZ) / (4 + symdat.level));
			for (int i = 0; i <= tries; i++)
			{
				foreach(FeatureAdder feat in features)
				{
					if (feat.addFeature(dungeon, this)) break;
				}
			}
		}


		/**
		 * Determine the number of doorways and add them.
		 * 
		 * @param dungeon
		 */
		protected void Doorways(Level dungeon)
		{
			int num = dungeon.random.NextInt(2 + ((endX - beginX + endZ - beginZ) / ((SymTable.Data(sym).level * 8) + 8))
					+ ((int)dungeon.style.subrooms / (2 + SymTable.Data(sym).level))) + 1;
			for (int i = 0; i < num; i++) Doorway(dungeon);
		}


		/**
		 * Adds a doorway.
		 * 
		 * @param dungeon
		 * @param x
		 * @param z
		 * @param xOriented
		 */
		protected void AddDoor(Level dungeon, int x, int z, Vector2Int direction)
		{
			doors.Add(new Doorway(x, z, direction));
			dungeon.map.SetDoorway(GameConstants.BaseDoorHeight, x, z);
		}


		/**
		 * Creates a doorway, or more than one based on room symmetry.
		 * 
		 * @param dungeon
		 */
		protected void Doorway(Level dungeon)
		{
			int xExtend = 0;
			int zExtend = 0;
			int xSeedDir = 0;
			int zSeedDir = 0;
			int wall = dungeon.random.NextInt(4);
			int x, z, oppX, oppZ;
			switch (wall)
			{
				case 0:
					x = beginX;
					oppX = endX;
					z = dungeon.random.NextInt(endZ - beginZ - 3) + 2;
					oppZ = endZ - z;
					z += beginZ;
					xSeedDir = -1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 1:
					z = beginZ;
					oppZ = endZ;
					x = dungeon.random.NextInt(endX - beginX - 3) + 2;
					oppX = endX - x;
					x += beginX;
					xSeedDir = 0;
					zSeedDir = -1;
					xExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 2:
					x = endX;
					oppX = beginX;
					z = dungeon.random.NextInt(endZ - beginZ - 3) + 2;
					oppZ = endZ - z;
					z += beginZ;
					xSeedDir = 1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 3:
					z = endZ;
					oppZ = beginZ;
					x = dungeon.random.NextInt(endX - beginX - 3) + 2;
					oppX = endX - x;
					x += beginX;
					xSeedDir = 0;
					zSeedDir = 1;
					xExtend = dungeon.random.NextInt(3) - 1;
					break;
				default: // Removes the "...may not be initialized" warning.
					x = beginX;
					oppX = endX;
					z = dungeon.random.NextInt(endZ - beginZ - 3) + 2;
					oppZ = endZ - z;
					z += beginZ;
					xSeedDir = -1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
			}
			AddDoor(dungeon, x, z, new Vector2Int(xSeedDir, zSeedDir));
			AddDoor(dungeon, x + xExtend, z + zExtend, new Vector2Int(xSeedDir, zSeedDir));
			childSeeds.Add(new RoomSeed(x + xSeedDir, floorY, z + zSeedDir));
			// Apply Symmetries
			switch (sym)
			{
				case Symmetry.NONE: break;
				case Symmetry.TR1:
					{
						oppX = (int)(realX + ((z - realZ) / (float)(endZ - beginZ)) * (endX - beginX));
						if (oppX < 1) oppX = 1;
						if (oppX > (dungeon.size.width - 2)) oppX = (dungeon.size.width - 2);
						oppZ = (int)(realZ + ((x - realX) / (float)(endX - beginX)) * (endZ - beginZ));
						if (oppZ < 1) oppZ = 1;
						if (oppZ > (dungeon.size.width - 2)) oppZ = (dungeon.size.width - 2);
						AddDoor(dungeon, oppX, oppZ, new Vector2Int(zSeedDir, xSeedDir));
						AddDoor(dungeon, oppX + xExtend, oppZ + zExtend, new Vector2Int(zSeedDir, xSeedDir));
						childSeeds.Add(new RoomSeed(oppX + zSeedDir, floorY, oppZ + xSeedDir));
					}
					break;
				case Symmetry.TR2:
					{
						oppX = (int)(realX + ((z - realZ) / (float)(endZ - beginZ)) * (endX - beginX));
						if (oppX < 1) oppX = 1;
						if (oppX > (dungeon.size.width - 2)) oppX = (dungeon.size.width - 2);
						oppZ = (int)(realZ + ((x - realX) / (float)(endX - beginX)) * (endZ - beginZ));
						oppZ = endZ - (oppZ - beginZ);
						if (oppZ < 1) oppZ = 1;
						if (oppZ > (dungeon.size.width - 2)) oppZ = (dungeon.size.width - 2);
						AddDoor(dungeon, oppX, oppZ, new Vector2Int(zSeedDir, -xSeedDir));
						AddDoor(dungeon, oppX + xExtend, oppZ + zExtend, new Vector2Int(zSeedDir, -xSeedDir));
						childSeeds.Add(new RoomSeed(oppX + zSeedDir, floorY, oppZ - xSeedDir));
					}
					break;
				case Symmetry.X:
					{
						AddDoor(dungeon, oppX, z, new Vector2Int(-xSeedDir, zSeedDir));
						AddDoor(dungeon, oppX - xExtend, z + zExtend, new Vector2Int(-xSeedDir, zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, z + zSeedDir));
					}
					break;
				case Symmetry.Z:
					{
						AddDoor(dungeon, x, oppZ, new Vector2Int(xSeedDir, -zSeedDir));
						AddDoor(dungeon, x + xExtend, oppZ - zExtend, new Vector2Int(xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(x + xSeedDir, floorY, oppZ - zSeedDir));
					}
					break;
				case Symmetry.XZ:
					{
						AddDoor(dungeon, oppX, z, new Vector2Int(-xSeedDir, zSeedDir));
						AddDoor(dungeon, oppX - xExtend, z + zExtend, new Vector2Int(-xSeedDir, zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, z + zSeedDir));
						AddDoor(dungeon, x, oppZ, new Vector2Int(xSeedDir, -zSeedDir));
						AddDoor(dungeon, x + xExtend, oppZ - zExtend, new Vector2Int(xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(x + xSeedDir, floorY, oppZ - zSeedDir));
						AddDoor(dungeon, oppX, oppZ, new Vector2Int(-xSeedDir, -zSeedDir));
						AddDoor(dungeon, oppX - xExtend, oppZ - zExtend, new Vector2Int(-xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, oppZ - zSeedDir));
					}
					break;
				case Symmetry.R:
					{
						AddDoor(dungeon, oppX, oppZ, new Vector2Int(-xSeedDir, -zSeedDir));
						AddDoor(dungeon, oppX - xExtend, oppZ - zExtend, new Vector2Int(-xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, oppZ - zSeedDir));
					}
					break;
				case Symmetry.SW:
					{
						AddDoor(dungeon, oppX, z, new Vector2Int(-xSeedDir, zSeedDir));
						AddDoor(dungeon, oppX - xExtend, z + zExtend, new Vector2Int(-xSeedDir, zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, z + zSeedDir));
						AddDoor(dungeon, x, oppZ, new Vector2Int(xSeedDir, -zSeedDir));
						AddDoor(dungeon, x + xExtend, oppZ - zExtend, new Vector2Int(xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(x + xSeedDir, floorY, oppZ - zSeedDir));
						AddDoor(dungeon, oppX, oppZ, new Vector2Int(-xSeedDir, -zSeedDir));
						AddDoor(dungeon, oppX - xExtend, oppZ - zExtend, new Vector2Int(-xSeedDir, -zSeedDir));
						childSeeds.Add(new RoomSeed(oppX - xSeedDir, floorY, oppZ - zSeedDir));
						break;
					}
				default: break;
			}
		}

		public bool PlantChildren(Level dungeon)
		{
			bool result = false;
			foreach (RoomSeed planted in childSeeds)
			{
				if (dungeon.rooms.Count >= dungeon.size.maxRooms) return false;
				int x = dungeon.random.NextInt(dungeon.size.width);
				int z = dungeon.random.NextInt(dungeon.size.width);
				int xdim = dungeon.random.NextInt(dungeon.size.maxRoomSize - 5) + 6;
				int zdim = dungeon.random.NextInt(dungeon.size.maxRoomSize - 5) + 6;
				int ymod = (xdim <= zdim) ? (int)Mathf.Sqrt(xdim) : (int)Mathf.Sqrt(zdim);
				int roomHeight = dungeon.random.NextInt(((int)dungeon.style.vertical / 2) + ymod) + 3;
				if (planted.growRoom(xdim, zdim, roomHeight, dungeon, null, this) != null)
					result = true;
			}
			return result;
		}


		/**
		 * Adds a room new room branching from this one that is part of a sequence of 
		 * rooms between two dungeon nods.
		 * 
		 * @param dungeon
		 * @param dir
		 * @param xdim
		 * @param zdim
		 * @param height
		 * @param source
		 * @return
		 */
		public Room Connector(Level dungeon, int dir, int xdim, 
						int zdim, int height, Route source)
		{
			int xExtend = 0;
			int zExtend = 0;
			int xSeedDir = 0;
			int zSeedDir = 0;
			int x, z, oppX, oppZ;
			switch (dir)
			{
				case 2:
					x = beginX;
					oppX = endX;
					z = (int)realZ;
					oppZ = z;
					xSeedDir = -1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 3:
					z = beginZ;
					oppZ = endZ;
					x = (int)realX;
					oppX = x;
					xSeedDir = 0;
					zSeedDir = -1;
					xExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 0:
					x = endX;
					oppX = beginX;
					z = (int)realZ;
					oppZ = z;
					xSeedDir = 1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
				case 1:
					z = endZ;
					oppZ = beginZ;
					x = (int)realX;
					oppX = x;
					xSeedDir = 0;
					zSeedDir = 1;
					xExtend = dungeon.random.NextInt(3) - 1;
					break;
				default: // Removes the "...may not be initialized" warning.
					x = beginX;
					oppX = endX;
					z = (int)realZ;
					oppZ = z;
					z += beginZ;
					xSeedDir = -1;
					zSeedDir = 0;
					zExtend = dungeon.random.NextInt(3) - 1;
					break;
			}
			AddDoor(dungeon, x, z, new Vector2Int(xSeedDir, zSeedDir));
			AddDoor(dungeon, x + xExtend, z + zExtend, new Vector2Int(xSeedDir, zSeedDir));
			if (((x + xSeedDir) >= dungeon.size.width) ||
					((x + xSeedDir) < 0) ||
					((z + zSeedDir) >= dungeon.size.width) ||
					((z + zSeedDir) < 0)) return null;
			if (dungeon.map.GetRoom(x + xSeedDir, z + zSeedDir) != 0)
			{
				return dungeon.rooms[dungeon.map.GetRoom(x + xSeedDir, z + zSeedDir)];
			}
			else if ((dir % 2) == 0) return new RoomSeed(x + xSeedDir, floorY, z + zSeedDir)
					 .growRoomZ(xdim, zdim, height, dungeon, null, this);
			else return new RoomSeed(x + xSeedDir, floorY, z + zSeedDir)
					.growRoomX(xdim, zdim, height, dungeon, null, this);
		}


		/**
		 * Determines the other rooms to which a door leads; this is 
		 * used in processing door corrections and room passibility.
		 * 
		 * @param door
		 */
		public void AddToConnections(Doorway door)
		{
			if (id < 1)
			{
				return;
			}
			if (connections.Count < 1)
			{
				DoorQueue pq = new DoorQueue(door.otherside);
				pq.Push(door, door);
				connections.Add(pq);
			}
			else
			{
				bool added = false;
				foreach(DoorQueue pq in connections)
				{
					if (pq.IsRoom(door.otherside))
					{
						pq.Push(door, door);
						added = true;
						break;
					}
				}
				if (!added)
				{
					DoorQueue pq = new DoorQueue(door.otherside);
					pq.Push(door, door);
					connections.Add(pq);
				}
			}
		}



	}

}
