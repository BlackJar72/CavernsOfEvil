using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
	public class CompareLists<T> : IComparer<List<T>>
	{
		public int Compare(List<T> o1, List<T> o2)
		{
			return o1.Count - o2.Count;
		}
	}


	/**
	 * This class contains utility methods for testing the validity and viability
	 * of doors.  Among other things it will remove the infamous doors to nowhere,
	 * sort doors by destination room, pick a good door for each room to run the
	 * A* connectively text-fix on, eliminate doors the transition between liquid 
	 * and non-liquid tiles, and place liquids in doors that have liquid on both
	 * sides.
	 * 
	 * @author JaredBGreat (Jared Blackburn)
	 *
	 */
	public class DoorChecker
		{
			static CompareLists<Room> c = new CompareLists<Room>();


		/**
		 * This will check to see if a tile is a valid location in the
		 * Dungeon.  That is, it will check to see if its both inside
		 * the dungeons and has a room value other than zero.
		 * 
		 * @param tile
		 * @return boolean for if tile is inside a room in the dungeon
		 */
		public static bool ValidateTile(Level dungeon, int x, int z)
		{
			if ((x < 0 || x >= dungeon.size.width) ||
				(z < 0 || z >= dungeon.size.width)) return false;
			return (dungeon.map.GetRoom(x, z) > 0);
		}


		/**
		 * Will check if a tile representing a door location actually
		 * connects two rooms, and remove doors that do not.  Put differently
		 * this will remove a door to nowhere and inform the calling method
		 * if such an action was needed.
		 * 
		 * @param door
		 * @return boolean for if the door connects two room in the dungeon
		 */
		public static bool ValidateDoor(Level dungeon, Doorway door)
		{
			return (ValidateTile(dungeon, door.x - 1, door.z)
					&& ValidateTile(dungeon, door.x + 1, door.z)
					&& ValidateTile(dungeon, door.x, door.z - 1)
					&& ValidateTile(dungeon, door.x, door.z + 1)
					&& ValidateTile(dungeon, door.x - 1, door.z - 1)
					&& ValidateTile(dungeon, door.x + 1, door.z + 1)
					&& ValidateTile(dungeon, door.x + 1, door.z - 1)
					&& ValidateTile(dungeon, door.x - 1, door.z + 1));
		}


		/**
		 * This will get a list of doors to run A* on for the purpose of 
		 * ensuring room passibility.
		 * 
		 * @param room
		 * @return ArrayList of one Doorway per connected room
		 */
		public static List<Doorway> MakeConnectionList(Room room, Xorshift random)
		{
			List<Doorway> output = new List<Doorway>(room.connections.Count);
			foreach(DoorQueue exits in room.connections)
			{
				output.Add(exits.First);
			}
			output.Shuffle(random);
			return output;
		}


		/**
		 * This will return the id of the room on the other side of the door.
		 * 
		 * @param exit
		 * @param room
		 * @param dungeon
		 * @return the id of the connect room
		 */
		public static int GetOtherRoom(Doorway exit, Room room, Level dungeon)
		{
			int output = dungeon.map.GetRoom(exit.x + exit.direction.x,
				exit.z + exit.direction.y);
			if (output != room.id)
			{
				return dungeon.map.GetRoom(exit.x + exit.direction.x,
					exit.z + exit.direction.y);
			}
			return dungeon.map.GetRoom(exit.x - exit.direction.x,
				exit.z - exit.direction.y);
		}


		/**
		 * This will run A* on exits to ensure all can be reached. It will also
		 * ensure the same door is used in connected rooms by giving a negative
		 * value and passing it to the connected rooms DoorQueue.	 * 
		 * 
		 * @param exits
		 */
		public static void CheckConnections(List<Doorway> exits,
						Room room, Level dungeon)
		{
			if (exits.Count < 1) return;
			Doorway next, current;
			List<Doorway> connected = new List<Doorway>(exits.Count);
			connected.Add(exits[exits.Count - 1]);
			exits.RemoveAt(exits.Count - 1);
			for (int i = exits.Count - 1; i > -1; --i)
			{
				current = connected[dungeon.random.NextInt(connected.Count)];
				connected.Add((next = exits[i]));
					new AStar(room, dungeon, current, next).Seek();
			}
		}


		public static void RetestDoors(Level dungeon, Room room)
		{
			if (room.doors.Count < 1)
			{
				return;
			}
			foreach(Doorway door in room.doors)
			{
				int doorHeight = dungeon.map.GetDoorway(door.x, door.z);
				if (!dungeon.map.GetAStared(door.x, door.z))
				{
					if (dungeon.map.GetWall(door.x + door.direction.x,
							door.z + door.direction.y) ||
						dungeon.map.GetWall(door.x - door.direction.x,
							door.z - door.direction.y))
						dungeon.map.SetDoorway(0, door.x, door.z);
					if ((dungeon.map.GetPool(door.x + door.direction.x,
							door.z + door.direction.y) > 0) ||
						(dungeon.map.GetPool(door.x - door.direction.x,
							door.z - door.direction.y) > 0))
						dungeon.map.SetDoorway(0, door.x, door.z);
					if (dungeon.map.GetPillar(door.x + door.direction.x,
							door.z + door.direction.y) ||
						dungeon.map.GetPillar(door.x - door.direction.x,
							door.z - door.direction.y))
						dungeon.map.SetDoorway(0, door.x, door.z);
				}
				else
				{
					if (dungeon.map.GetWall(door.x + door.direction.x,
							door.z + door.direction.y)) {
						if (!dungeon.map.GetWall(door.x + 2 * door.direction.x,
								door.z + 2 * door.direction.y))
						{
							dungeon.map.SetDoorway(doorHeight, door.x + door.direction.x,
								door.z + door.direction.y);
						}
						else
						{
							dungeon.map.SetDoorway(0, door.x, door.z);
						}
					}
					else if(dungeon.map.GetWall(door.x - door.direction.x,
							door.z - door.direction.y))
					{
						if (!dungeon.map.GetWall(door.x - 2 * door.direction.x,
								door.z + 2 * door.direction.y))
						{
							dungeon.map.SetDoorway(doorHeight, door.x - door.direction.x,
								door.z - door.direction.y);
						}
						else if (!(dungeon.map.GetDoorway(door.x + door.direction.x,
							door.z + door.direction.y) == doorHeight))
						{
							dungeon.map.SetDoorway(0, door.x, door.z);
						}
					}
				}
			}
		}


		public static void ProcessDoors1(Level dungeon, Room room)
		{
			List<Doorway> invalid = new List<Doorway>();
			bool valid;
			foreach(Doorway door in room.doors)
			{
				valid = ValidateDoor(dungeon, door);
				if (!valid)
				{
					invalid.Add(door);
					dungeon.map.SetDoorway(0, door.x, door.z);
				}
				else
				{
					door.prioritize(dungeon, room.id); // Will only be run on valid doors
					room.AddToConnections(door);
				}
			}
			foreach (Doorway door in invalid) room.doors.Remove(door);
		}


		public static void CheckConnectivity(Level dungeon)
			{
				List<List<Room>> sections = new RoomBFS(dungeon).Check();
				// TODO: A better way of doing this!
			}


		public static void ProcessDoors2(Level dungeon, Room room)
		{
			room.topDoors = MakeConnectionList(room, dungeon.random);
			foreach(Doorway exit in room.topDoors)
			{
				exit.priority -= 16;
				dungeon.rooms[GetOtherRoom(exit, room, dungeon)]
					.AddToConnections(new Doorway(exit, room.id));
			}
		}


		public static void ProcessDoors3(Level dungeon, Room room)
		{
			CheckConnections(room.topDoors, room, dungeon);
			RetestDoors(dungeon, room);
		}


		public static void CaveConnector(Level dungeon, Room cave)
		{
			foreach(Doorway door in cave.doors)
			{
				new AStar(cave, dungeon, cave.midpoint, dungeon.rooms[door.otherside].midpoint).Seek();
			}
		}
	}

}