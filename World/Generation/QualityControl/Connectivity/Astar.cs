using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;


namespace DLD
{

	/**This meant to represent a Tile (block column / xz coordinate) that acts
	 * as a both a node and an implied edge (from the previous node) in an
	 * implied nav-graph consisting of the rooms floor surface.
	 * 
	 * The purpose of this is for storing steps on the route from door to 
	 * door, both in an abstract PriorityQueue and in a tree representing 
	 * valid routes.
	 * 
	 * @author JaredBGreat (Jared Blackburn)
	 *
	 */
	public class Step : IComparable<Step>
	{
		// FIXME: This class has a lot data that turn out to be extraneous

		public readonly int x, z;
		public readonly int changes;   // Changes made to create a player-passable route
		public readonly int distance;  // Tiles traversed from start to get here
		public readonly int heuristic; // Manhattan distance to destination
		public readonly float value;     // (distance + 16*changes)
		public readonly Step parent;

		public float Value { get { return value + heuristic; } }


		public Step(Tile t)
		{
			x = t.x;
			z = t.z;
			distance = 0;
			this.changes = 0;
			heuristic = 0;
			value = 0;
			parent = null;
		}


		public Step(int x, int z)
		{
			this.x = x;
			this.z = z;
			distance = 0;
			this.changes = 0;
			heuristic = 0;
			value = 0;
			parent = null;
		}


		public Step(int x, int z, int traversed, int changes, Tile destination)
		{
			this.x = x;
			this.z = z;
			distance = traversed;
			this.changes = changes;
			heuristic = Mathf.Abs(x - destination.x) + Mathf.Abs(z - destination.z);
			value = (changes * 16) + distance + heuristic;
			parent = null;
		}


		/**
		 * The preferred way to constructor for creating a Step.
		 * 
		 * @param x
		 * @param z
		 * @param previous
		 * @param destination
		 * @param dungeon
		 */
		public Step(int x, int z, Step previous, Step destination, Level dungeon)
		{
			this.x = x;
			this.z = z;
			distance = previous.distance + 1;
			heuristic = Mathf.Abs(x - destination.x) + Mathf.Abs(z - destination.z);
			changes = previous.changes;
			if (dungeon.map.GetWall(x, z)) changes++;
			if (dungeon.map.GetWall(x, z) && (dungeon.map.GetDoorway(x, z) < 1)) changes++;
			if (dungeon.map.GetPool(x, z) > 0) changes++;
			if (Mathf.Abs(dungeon.map.GetFloorY(x, z)
					- dungeon.map.GetFloorY(previous.x, previous.z)) > 1) changes++;
			value = (changes * 16) + previous.value + 1;
			if (!dungeon.map.GetAStared(x, z)) value++;
			parent = previous;
		}


		/**
		 * Returns the first step from a Tile ("door") toward a destination Tile.
		 * 
		 * @param door
		 * @param destination
		 * @return first step from door to destination
		 */
		public static Step firstFromDoorway(Tile door, Tile destination)
		{
			return new Step(door.x, door.z, 0, 0, destination);
		}


		/**
		 * Returns a Tile with the same coordinates as the Step
		 * 
		 * @return The base Tile representation of this Step
		 */
		public Tile GetTile()
		{
			return new Tile(x, z);
		}


		public int CompareTo(Step other)
		{
			return (int)(value - other.value);
		}


		public bool Equals(Step other)
        {
            return (x == other.x) && (z == other.z);
        }


		public override string ToString()
        {
			return "{Step [x = " + x + ", z = " + z + "]}";
        }


		public Vector2Int ToVector()
        {
			return new Vector2Int(x, z);
        }

	}


	public class AStar
	{

		readonly int roomID, oRoomID, x1, x2, z1, z2;   // Room id and bounds
		SimplePriorityQueue<Step, float> edges;  // Steps to consider
		readonly Level dungeon;
		readonly Room room;
		readonly Step root;
		Step end;

		private AStar() {/*Do not use!*/}

		public AStar(Room room, Level dungeon, Doorway start, Doorway finish)
		{
			this.room = room;
			this.roomID = room.id;
			this.dungeon = dungeon;
			x1 = room.beginX;
			x2 = room.endX;
			z1 = room.beginZ;
			z2 = room.endZ;

			oRoomID = dungeon.map.GetRoom(finish.x, finish.z);
			if(oRoomID == roomID)
            {
				oRoomID = dungeon.map.GetRoom(start.x, start.z);
			}

			end = new Step(finish);
			if (end.x < x1)
			{
				x1 = end.x;
			}
			if (end.x > x2)
			{
				x2 = end.x;
			}
			if (end.z < z1)
			{
				z1 = end.z;
			}
			if (end.z > z2)
			{
				z2 = end.z;
			}

			root = Step.firstFromDoorway(start, finish);
			if (root.x < x1)
			{
				x1 = root.x;
			}
			if (root.x > x2)
			{
				x2 = root.x;
			}
			if (root.z < z1)
			{
				z1 = root.z;
			}
			if (root.z > z2)
			{
				z2 = root.z;
			}

			edges = new SimplePriorityQueue<Step, float>();
			edges.Enqueue(root, root.value);
		}


		/**
		 * This uses the data from AStar to make useful changes to the 
		 * dungeon.
		 * 
		 * @param end
		 */
		public void MakeRoute(Step end)
		{
			Step child = end, parent = end.parent;
			if (parent == null) return;

			dungeon.map.SetAstared(end.x, end.z);
			if (dungeon.map.GetWall(end.x, end.z))
				dungeon.map.SetDoorway(GameConstants.BaseDoorHeight, end.x, end.z);
			if(dungeon.map.GetPillar(end.x, end.z))
				dungeon.map.SetPillar(false, end.x, end.z);
			dungeon.map.SetPool(0, end.x, end.z);
			dungeon.map.SetFloorY(room.floorY, end.x, end.z);

			do
			{
				dungeon.map.SetAstared(child.x, child.z);
				if (dungeon.map.GetWall(child.x, child.z))
					AddDoor(parent, child);
				if (dungeon.map.GetPillar(child.x, child.z))
					dungeon.map.UnSetPillar(child.x, child.z);
				dungeon.map.SetPool(0, child.x, child.z);
				dungeon.map.SetFloorY(room.floorY, child.x, child.z);								

				child = parent;
				parent = child.parent;
			} while (parent != null);

			dungeon.map.SetAstared(child.x, child.z);
			if (dungeon.map.GetWall(child.x, child.z))
				dungeon.map.SetDoorway(GameConstants.BaseDoorHeight, child.x, child.z);
			if (dungeon.map.GetPillar(child.x, child.z))
				dungeon.map.UnSetPillar(child.x, child.z);
			dungeon.map.SetPool(0, child.x, child.z);
			dungeon.map.SetFloorY(room.floorY, child.x, child.z);
		}


		protected void AddDoor(Step from, Step to)
		{
			dungeon.map.SetDoorway(GameConstants.BaseDoorHeight, to.x, to.z);
			dungeon.map.SetDoorway(GameConstants.BaseDoorHeight, from.x, from.z);
		}


		protected void FixLiquid(Step to)
		{
			dungeon.map.SetFloorY(dungeon.rooms[roomID].floorY, to.x, to.z);
			dungeon.map.SetPool(0, to.x, to.z);
		}


		protected void FixHeights(Step from, Step to)
		{
			dungeon.map.SetFloorY(dungeon.rooms[roomID].floorY, to.x, to.z);
		}


		/**
		 * This is what actually runs A* (and also calls other methods
		 * to make practical use of the results).
		 */
		public void Seek()
		{
			dungeon.map.ResetUsed(x1, z1, x2 + 1, z2 + 1);
			Step current;
			do
			{
				current = edges.Dequeue();
				AddNextSteps(current);
			} 
			while ((edges.Count > 0) && !current.Equals(end));
			MakeRoute(current);
		}


		// Adds a potential new edge (tile), if it is in the room 
		// and has not already been used.
		protected void AddEdge(Step src, Vector2Int dir)
        {
			int childX = src.x + dir.x;
			int childZ = src.z + dir.y;
			if((((childX > x1) && (childX < x2) 
					&& (childZ > z1) && (childZ < z2)) 
						|| ((childX == end.x) && (childZ == end.z)))
				&& dungeon.map.GetAstarAvailable(childX, childZ, roomID, oRoomID))
            {
				dungeon.map.SetUsed(childX, childZ);
				Step child = new Step(childX, childZ, src, end, dungeon);
				edges.Enqueue(child, child.Value);
			}
		}


		protected virtual void AddNextSteps(Step src)
		{
			AddEdge(src, Vector2Int.up);
			AddEdge(src, Vector2Int.down);
			AddEdge(src, Vector2Int.right);
			AddEdge(src, Vector2Int.left);
		}
	}


}
