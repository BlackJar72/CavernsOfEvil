using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

	public class Route
	{


		private readonly HubRoom start, finish;
		private Room current1, current2, temp;
		private float realXDist, realZDist;
		private int bXDist, bZDist, dir1, dir2;
		private bool xMatch, zMatch, finishTurn, complete, comp1, comp2;
		private List<Room> side1, side2;

		 
		public Route(HubRoom start, HubRoom finish)
		{
			this.start = start;
			this.finish = finish;
			current1 = start.TheRoom;
			current2 = finish.TheRoom;
			side1 = new List<Room>();
			side1.Add(current1);
			side2 = new List<Room>();
			side2.Add(current2);
			finishTurn = false;
			complete = false;

			if (realXDist > 3.0f)
			{
				bXDist = current2.endX - current1.beginX;
				xMatch = false;
			}
			else if (realXDist < -3.0f)
			{
				bXDist = current2.beginX - current1.endX;
				xMatch = false;
			}
			else
			{
				bXDist = (int)realXDist;
				xMatch = true;
			}

			if (realZDist > 3.0f)
			{
				bZDist = current2.endZ - current1.beginZ;
				zMatch = false;
			}
			else if (realZDist < -3.0f)
			{
				bZDist = current2.beginZ - current1.endZ;
				zMatch = false;
			}
			else
			{
				bZDist = (int)realZDist;
				zMatch = true;
			}
		}


		/**
		 * This will attempt to draw the connecting serious of rooms by 
		 * calling drawConnection until either the connection is flagged as 
		 * complete or the dungeon has reach the maximum number of rooms. 
		 * 
		 * Note that the connection may be flagged complete either because 
		 * an actual connection has been found or because both sequences 
		 * (that from the start and that from the
		 * 
		 * @param dungeon
		 */
		public void drawConnections(Level dungeon)
		{
			int limit = dungeon.size.maxRooms;
			while (!complete && (limit > 0))
			{
				limit--;
				if (dungeon.rooms.Count >= dungeon.size.maxRooms) return;
				drawConnection(dungeon);
				if (complete || (limit < 0)) return;
			}
		}


		/**
		 * This will try to add a room to one of the sequences.  Which sequence gets 
		 * this turn is determined by the internal finishTurn variable, which alternates 
		 * with each call.  The location of the new rooms will always be closer on either 
		 * the x or z axis, though which is determined randomly, though weighted to favor 
		 * the axis on which the distance is greatest.
		 * 
		 * The connection is considered complete when either the sequence of rooms from 
		 * either end meet or when each fails to add a room on consecutive turns (a 
		 * give-up condition).
		 * 
		 * @param dungeon
		 */
		public void drawConnection(Level dungeon)
		{
			//System.out.println("Running drawConnections(Dungeon dungeon)");
			getGrowthDir(dungeon.random);
			int x = dungeon.random.NextInt(dungeon.size.width);
			int z = dungeon.random.NextInt(dungeon.size.width);
			int xdim = dungeon.random.NextInt(dungeon.size.maxRoomSize - 5) + 6;
			int zdim = dungeon.random.NextInt(dungeon.size.maxRoomSize - 5) + 6;
			if (close(dungeon.size.maxRoomSize - 1)) xdim = zdim = dungeon.size.maxRoomSize;
			int ymod = (xdim <= zdim) ? (int)Mathf.Sqrt(xdim) : (int)Mathf.Sqrt(zdim);
			int roomHeight = dungeon.random.NextInt(((int)dungeon.style.vertical / 2) + ymod) + 3;
			if (finishTurn)
			{
				dir1 = (dir1 + 2) % 4;
				dir2 = (dir2 + 2) % 4;
				temp = current2.Connector(dungeon, dir1, xdim, zdim, roomHeight, this);
				if ((temp == null) || side2.Contains(temp)) temp = current2.Connector(dungeon, dir2, xdim, zdim, roomHeight, this);
				if ((temp == null) || side2.Contains(temp))
				{
					comp2 = true; // for now, give up
				}
				else if (side1.Contains(temp))
				{
					complete = true; // Success!
				}
				else
				{
					side2.Add(temp);
					current2 = temp;
				}
			}
			else
			{
				temp = current1.Connector(dungeon, dir1, xdim, zdim, roomHeight, this);
				if (temp == null || side1.Contains(temp)) temp = current1.Connector(dungeon, dir2, xdim, zdim, roomHeight, this);
				if (temp == null || side1.Contains(temp))
				{
					comp1 = true; // for now, give up
				}
				else if (side2.Contains(temp))
				{
					complete = true; // Success!
				}
				else
				{
					side1.Add(temp);
					current1 = temp;
				}
			}
			if (!complete) complete = comp1 && comp2;
			if (comp1) finishTurn = true;
			else if (comp2) finishTurn = false;
			else finishTurn = !finishTurn;
		}





		// Helper methods

		/**
		 * @return ture if the terminal rooms in both sequences overlap on the x axis.
		 */
		private bool xOverlap()
		{
			return ((current1.endX > current2.beginX) && (current2.endX > current1.beginX));
		}


		/**
		 * @return true if the terminal rooms in both sequences overlap on the z axis. 
		 */
		private bool zOverlap()
		{
			return ((current1.endZ > current2.beginZ) && (current2.endZ > current1.beginZ));
		}


		/**
		 * @return true if the room edges align on x and the rooms overlap on z
		 */
		private bool touchesOnX()
		{
			if (!zOverlap()) return false;
			return ((current1.beginX == current2.endX) || (current1.endX == current2.beginX));
		}


		/**
		 * @return true if the room edges align on z and overlap on x.
		 */
		private bool touchesOnZ()
		{
			if (!xOverlap()) return false;
			return ((current1.beginZ == current2.endZ) || (current1.endZ == current2.beginZ));
		}


		/**
		 * @return true if the the terminal rooms in each sequence are directly adjacent
		 */
		private bool touching()
		{
			return (touchesOnX() || touchesOnZ());
		}


		/**
		 * This will determine which wall is touching between the terminal rooms on each 
		 * sequence.  Specifically this finds which direction to move from the terminal room in
		 * the sequence to that in the finish sequence.
		 * 
		 * @return the direction from the start sequence to the finish sequence.
		 */
		private int touchDir()
		{
			if (zOverlap())
			{
				if (current1.endX == current2.beginX) return 0;
				if (current1.beginX == current2.endX) return 2;
			}
			else if (xOverlap())
			{
				if (current1.endZ == current2.beginZ) return 1;
				if (current1.beginZ == current2.endZ) return 3;
			}
			return -1; // Not touching
		}


		/**
		 * This determines if the terminal rooms in the each sequence are close, that is
		 * if they are close enough to fill the gap with a single room.  This is used to 
		 * prevent potential connections are not ruined by placing an room whose target 
		 * size is too short to connect but leave too small a gap for another room.
		 * 
		 * @param range
		 * @return
		 */
		private bool close(int range)
		{
			if (Mathf.Abs(current1.beginX - current2.endX) > range) return false;
			if (Mathf.Abs(current2.beginX - current1.endX) > range) return false;
			if (Mathf.Abs(current1.beginZ - current2.endZ) > range) return false;
			if (Mathf.Abs(current2.beginZ - current1.endZ) > range) return false;
			return true;
		}


		/**
		 * This uses randomness and the coordinate of the terminal rooms in each 
		 * sequence to determine in which direction a new rooms should be added.
		 * 
		 * The direction is picked in such a way that sequences attempt to "grow" 
		 * toward each other while also maintaining a degree of randomness.
		 * 
		 * @param random
		 */
		private void getGrowthDir(Xorshift random)
		{
			bool posX = (current1.realX < current2.realX);
			bool posZ = (current1.realZ < current2.realZ);
			if (xOverlap())
			{
				if (posZ)
				{
					dir1 = 1;
					dir2 = random.NextInt(2) * 2;
				}
				else
				{
					dir1 = 3;
					dir2 = random.NextInt(2) * 2;
				}
			}
			else if (zOverlap())
			{
				if (posX)
				{
					dir1 = 0;
					dir2 = 1 + random.NextInt(2) * 2;
				}
				else
				{
					dir1 = 2;
					dir2 = 1 + random.NextInt(2) * 2;
				}
			}
			else if (random.NextInt((int)(Mathf.Abs(realXDist) + Mathf.Abs(realZDist) + 1))
				  > (int)(Mathf.Abs(realXDist)))
			{
				if (posX)
				{
					dir1 = 0;
					if (posZ) dir2 = 1;
					else dir2 = 3;
				}
				else
				{
					dir1 = 2;
					if (posZ) dir2 = 1;
					else dir2 = 3;
				}
			}
			else
			{
				if (posZ)
				{
					dir1 = 1;
					if (posX) dir2 = 0;
					else dir2 = 2;
				}
				else
				{
					dir1 = 3;
					if (posX) dir2 = 0;
					else dir2 = 2;
				}
			}
		}


	}

}
