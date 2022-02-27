using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

	/**
	 * This is a 1x1 column of block, or put differently, a block
	 * in 2D.  This is used to repressent a location in the 2D map,
	 * especially for use with pathfinding / graph algorithms, for 
	 * which tiles are the vertices and adjency between tiles is an
	 * edge.
	 * 
	 * This being used mostly as a C-struct, so everything is left 
	 * public. 
	 * 
	 * @author Jared Blackburn
	 *
	 */
	public class Tile
	{
		public int x, z;
		public Tile(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public bool Equals(Tile other)
		{
			return ((x == other.x) && (z == other.z));
		}

        public int Hash()
		{
			return x + (z << 8) + (z << 16) + (x << 24);
		}


		public Vector2Int ToVector()
        {
			return new Vector2Int(x, z);
        }
	}



	/**
	 * This class represents a doorway for use with classes in the astar 
	 * package, notable DoorChecker and AStar.
	 * 
	 * Note that this represents the door in relation to a specific room;
	 * the same room will have a separate listing for each room it connects
	 * within that rooms list of doors.
	 * 
	 * @author Jared Blackburn
	 *
	 */
	public class Doorway : Tile, IComparable 
	{
		// The direction the door points
		public Vector2Int direction;
		/**
		 * True if aligned along the x axis, false if aligned along the z axis.
		 */
		public bool xOriented;
		/**
		 * How desirable this door is as the primary connection between rooms; this
		 * is effected by its position in relation to pool and its previous uses in
		 * connecting rooms. 
		 */
		public int priority;
		/**
		 * The id (index) of the room on the other side.
		 */
		public int otherside;

		/* Very depricated...unless I get stuct in trying to adapt it*/
		//public Doorway(int x, int z, bool xOriented) : base(x, z)
		//{
		//	this.xOriented = xOriented;
		//	priority = 0;
		//}


		public Doorway(int x, int z, Vector2Int direction) : base(x, z)
		{
			this.direction = direction;
			xOriented = direction.x != 0;
			priority = 0;
		}


		public Doorway(Doorway door) : base(door.x, door.z)
		{
			xOriented = door.xOriented;
			priority = door.priority;
			otherside = door.otherside;
		}


		public Doorway(Doorway door, int otherside) : base(door.x, door.z)
		{
			direction = door.direction;
			xOriented = door.xOriented;
			priority = door.priority;
			this.otherside = otherside;
		}


		public Doorway(Doorway door, Room otherside) : base(door.x, door.z)
		{
			direction = door.direction;
			xOriented = door.xOriented;
			priority = door.priority;
			this.otherside = otherside.id;
		}


		/**
		 * This will find the number of sides bordering a "liguid"
		 * pool and add it to the priority value; since the Java
		 * priority queue sorts to lowest value first increasing this
		 * effectively decreases the priority so that pools with no
		 * or fewer pools beside them will be chosen first as doors
		 * for AStar to connect.
		 * 
		 * @param dungeon
		 * @param start
		 */
		public void prioritize(Level dungeon, int start)
		{
			{
				if (dungeon.map.GetPool(x + direction.x, z + direction.y) > 0) priority++;
				if (dungeon.map.GetPool(x - direction.x, z - direction.y) > 0) priority++;
				otherside = dungeon.map.GetRoom(x + direction.x, z + direction.y);
			}
		}


		public int CompareTo(Doorway o)
		{
			return priority - o.priority;
		}


		/**
		 * Returns the same location represented as a Tile.
		 * 
		 * @return basic tile at the same location
		 */
		public Tile GetTile()
		{
			return new Tile(x, z);
		}


        public int CompareTo(object o)
        {
			if (o is Doorway) return priority - ((Doorway)o).priority;
			return int.MaxValue;
        }
    }
}
