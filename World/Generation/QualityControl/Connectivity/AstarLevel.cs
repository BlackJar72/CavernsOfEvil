using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;


namespace CevarnsOfEvil
{
	public class AStarLevel
	{

		SimplePriorityQueue<LevelStep, float> edges;  // Steps to consider
		readonly Level dungeon;
		readonly LevelStep root;
		LevelStep end;

		private AStarLevel() {/*Do not use!*/}

		public AStarLevel(Level dungeon, Room startRoom, Room endRoom)
		{
			this.dungeon = dungeon;

			Tile start = new Tile(startRoom.midX, startRoom.midZ);
			Tile finish = new Tile(endRoom.midX, endRoom.midZ);

			end = new LevelStep(finish);

			root = LevelStep.FirstFromStart(start, finish);

			edges = new SimplePriorityQueue<LevelStep, float>();
			edges.Enqueue(root, root.Value);
		}


		public void MakeRoute(LevelStep end)
		{
			LevelStep child = end, parent = end.parent;
			if (parent == null) return;
			dungeon.map.SetAstared(end.x, end.z);
			do {
				dungeon.map.SetAstared(child.x, child.z);
				child = parent;
				parent = child.parent;
			} while (parent != null);
			dungeon.map.SetAstared(child.x, child.z);
		}


		/**
		 * This is what actually runs A* (and also calls other methods
		 * to make practical use of the results).  It will run until 
         * it has either found the (shortest) path or run out of reachable 
         * nodes.
         * 
         * Returns true if a it succeeded, or false if no path was found.
		 */
		public bool Seek()
		{
			bool complete = false;
			dungeon.map.ResetUsed();
			LevelStep current;
			do
			{
				current = edges.Dequeue();
				AddNextSteps(current);
				complete = current.Equals(end);
			} 
			while ((edges.Count > 0) && !complete);
			if(complete) MakeRoute(current);
			return complete;
		}


		// Adds a potential new edge (tile), if it is in the room 
		// and has not already been used.
		protected void AddEdge(LevelStep src, Vector2Int dir)
        {
			int childX = src.x + dir.x;
			int childZ = src.z + dir.y;
			if((((childX > 0) && (childX < dungeon.size.width) 
					&& (childZ > 0) && (childZ < dungeon.size.width)))
				&& dungeon.map.GetLevelAstarAvailable(childX, childZ)
				&& dungeon.map.GetFloorY(childX, childZ) == dungeon.map.GetFloorY(src.x, src.z))
            {
				dungeon.map.SetUsed(childX, childZ);
				LevelStep child = new LevelStep(childX, childZ, src, end, dungeon);
				edges.Enqueue(child, child.Value);
			}
		}


		protected virtual void AddNextSteps(LevelStep src)
		{
			AddEdge(src, Vector2Int.up);
			AddEdge(src, Vector2Int.down);
			AddEdge(src, Vector2Int.right);
			AddEdge(src, Vector2Int.left);
		}
	}




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
	public class LevelStep : IComparable<LevelStep>
	{
		// FIXME: This class has a lot data that turn out to be extraneous

		public readonly int x, z;
		public readonly int changes;   // Changes made to create a player-passable route
		public readonly int distance;  // Tiles traversed from start to get here
		public readonly int heuristic; // Manhattan distance to destination
		public readonly float value;     // (distance + 16*changes)
		public readonly LevelStep parent;

		public float Value { get { return value + heuristic; } }


		public LevelStep(Tile t)
		{
			x = t.x;
			z = t.z;
			distance = 0;
			this.changes = 0;
			heuristic = 0;
			value = 0;
			parent = null;
		}


		public LevelStep(int x, int z)
		{
			this.x = x;
			this.z = z;
			distance = 0;
			this.changes = 0;
			heuristic = 0;
			value = 0;
			parent = null;
		}


		public LevelStep(int x, int z, int traversed, int changes, Tile destination)
		{
			this.x = x;
			this.z = z;
			distance = traversed;
			this.changes = changes;
			heuristic = Mathf.Abs(x - destination.x) + Mathf.Abs(z - destination.z);
			value = (changes * 16) + distance;
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
		public LevelStep(int x, int z, LevelStep previous, LevelStep destination, Level dungeon)
		{
			this.x = x;
			this.z = z;
			distance = previous.distance + 1;
			heuristic = Mathf.Abs(x - destination.x) + Mathf.Abs(z - destination.z);
			changes = previous.changes;
			if (!dungeon.map.GetAStared(x, z)) changes++;
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
		public static LevelStep FirstFromStart(Tile start, Tile destination)
		{
			return new LevelStep(start.x, start.z, 0, 0, destination);
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


		public int CompareTo(LevelStep other)
		{
			return (int)(value - other.value);
		}


		public bool Equals(LevelStep other)
		{
			return (x == other.x) && (z == other.z);
		}


		public override string ToString()
		{
			return "{LevelStep [x = " + x + ", z = " + z + "]}";
		}


		public Vector2Int ToVector()
		{
			return new Vector2Int(x, z);
		}

	}


}
