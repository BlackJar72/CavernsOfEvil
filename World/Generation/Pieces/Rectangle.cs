using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class Rectangle
    {
		// Width in each dimension and center coordinates
		private readonly float xdim, zdim, xcoord, zcoord;

		public Rectangle(float xdim, float zdim, float xcoord, float zcoord)
		{
			this.xdim = xdim;
			this.zdim = zdim;
			this.xcoord = xcoord;
			this.zcoord = zcoord;
		}

		/***********************/
		/* Accessor Properties */
		/***********************/

		public float Xdim { get => xdim; }
		public float Zdim { get => zdim; }
		public float Xcoord { get => xcoord; }
		public float Zcoord { get => zcoord; }


		/***********************/
		/* Do The Actual Work  */
		/***********************/


		/**
		 * Add the rectangle to the dungeon as pool of "liquid."
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx x coordinate
		 * @param sz z coordinate
		 * @param sdimx length on x
		 * @param sdimz length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawLiquid(Level dungeon, Room room, int height, float sx, float sz, float sdimx, float sdimz,
				bool invertX, bool invertZ)
		{
			int xbegin, xend, zbegin, zend;

			// Find actual beginning and ending coordinates
			if (invertX)
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
			}
			else
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
			}
			if (invertZ)
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
			}
			else
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
			}

			xbegin = Mathf.Clamp(xbegin, room.beginX + 1, room.endX - 1);
			zbegin = Mathf.Clamp(zbegin, room.beginZ + 1, room.endZ - 1);
			xend = Mathf.Clamp(xend, room.beginX + 1, room.endX - 1);
			zend = Mathf.Clamp(zend, room.beginZ + 1, room.endZ - 1);

			// Place floor, ceiling, and empty space
			for (int k = zbegin; k < zend; k++)
				for (int j = xbegin; j < xend; j++)
				{
					if ((j < 0) || (j >= dungeon.size.width) || (k < 0) || (k >= dungeon.size.width)) continue;
					if ((dungeon.map.GetRoom(j, k) == room.id) && (dungeon.map.GetDoorway(j, k) < 1))
					{
						dungeon.map.SetFloorY(Mathf.Min(dungeon.map.GetFloorY(j, k), height - 1), j, k);
						dungeon.map.SetPool(height - dungeon.map.GetFloorY(j, k), j, k);
					}
				}
		}


		/**
		 * Add the rectangle to as a walkway (normal floor) through a previously added pool.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx x coordinate
		 * @param sz z coordinate
		 * @param sdimx length in x 
		 * @param sdimz length in z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawWalkway(Level dungeon, Room room, float sx, float sz, byte sdimx, byte sdimz,
					bool invertX, bool invertZ)
		{
			byte xbegin, xend, zbegin, zend;
			//System.out.println("Writing rectange.");
			if (invertX)
			{
				xbegin = (byte)(sx - ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
				xend = (byte)(sx + ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
			}
			else
			{
				xbegin = (byte)(sx - ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
				xend = (byte)(sx + ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
			}
			if (invertZ)
			{
				zbegin = (byte)(sz - ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
				zend = (byte)(sz + ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
			}
			else
			{
				zbegin = (byte)(sz - ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
				zend = (byte)(sz + ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
			}
			for (byte k = zbegin; k < zend; k++)
				for (byte j = xbegin; j < xend; j++)
				{
					if ((j < 0) || (j >= dungeon.size.width) || (k < 0) || (k >= dungeon.size.width)) continue;
					dungeon.map.SetFloorY(dungeon.map.GetFloorY(j, k) + dungeon.map.GetPool(j, k), j, k);
					dungeon.map.SetPool(0, j, k);
				}
			//System.out.println("Rectangle has been drawn.");
		}


		/**
		 * Adds the rectangle as a section filled with walls from floor to ceiling (an 
		 * area cut out from the rooms actual space).
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx x coordinate
		 * @param sz z coordinate
		 * @param sdimx length on x
		 * @param sdimz length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawCutout(Level dungeon, Room room, float sx, float sz, float sdimx, float sdimz,
				bool invertX, bool invertZ)
		{
			int xbegin, xend, zbegin, zend;
			if (invertX)
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
			}
			else
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
			}
			if (invertZ)
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
			}
			else
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
			}

			xbegin = Mathf.Clamp(xbegin, room.beginX + 1, room.endX - 1);
			zbegin = Mathf.Clamp(zbegin, room.beginZ + 1, room.endZ - 1);
			xend = Mathf.Clamp(xend, room.beginX + 1, room.endX - 1);
			zend = Mathf.Clamp(zend, room.beginZ + 1, room.endZ - 1);

			for (int k = zbegin; k < zend; k++)
				for (int j = xbegin; j < xend; j++)
				{
					if ((j < 0) || (j >= dungeon.size.width) || (k < 0) || (k >= dungeon.size.width)) continue;
					if (dungeon.map.GetRoom(j, k) != room.id) continue;
					dungeon.map.SetWall(j, k);
				}
		}


		/**
		 * Add the rectangle as a area of empty space, that is, an area from which previously
		 * place walls have been removed.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx x coordinate
		 * @param sz z coordinate
		 * @param sdimx length in x
		 * @param sdimz length in z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawCutin(Level dungeon, Room room, float sx, float sz, byte sdimx, byte sdimz,
				bool invertX, bool invertZ)
		{
			byte xbegin, xend, zbegin, zend;
			if (invertX)
			{
				xbegin = (byte)(sx - ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
				xend = (byte)(sx + ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
			}
			else
			{
				xbegin = (byte)(sx - ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
				xend = (byte)(sx + ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
			}
			if (invertZ)
			{
				zbegin = (byte)(sz - ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
				zend = (byte)(sz + ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
			}
			else
			{
				zbegin = (byte)(sz - ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
				zend = (byte)(sz + ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
			}
			for (byte k = zbegin; k < zend; k++)
				for (byte j = xbegin; j < xend; j++)
				{
					if ((j < 0) || (j >= dungeon.size.width) || (k < 0) || (k >= dungeon.size.width)) continue;
					dungeon.map.UnSetWall(j, k);
				}
		}


		/**
		 * This will add a platform built based on the rectangle and its specifications.
		 * The specifications include the scaling factors in x and z, coordinated to 
		 * place it, and if it should be inverted.  This technically resent the floor 
		 * height, and is thus also used to add depression be setting the floor to a lower
		 * height rather than higher.
		 * 
		 * @param dungeon
		 * @param room
		 * @param floorY The new floor height
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawPlatform(Level dungeon, Room room, int floorY, float sx, float sz, float sdimx, float sdimz,
				bool invertX, bool invertZ)
		{
			int xbegin, xend, zbegin, zend;
			if (invertX)
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) - (xcoord * sdimx));
			}
			else
			{
				xbegin = (int)(sx - ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
				xend = (int)(sx + ((sdimx * xdim) / 2.0f) + (xcoord * sdimx));
			}
			if (invertZ)
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) - (zcoord * sdimz));
			}
			else
			{
				zbegin = (int)(sz - ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
				zend = (int)(sz + ((sdimz * zdim) / 2.0f) + (zcoord * sdimz));
			}

			xbegin = Mathf.Clamp(xbegin, room.beginX + 1, room.endX - 1);
			zbegin = Mathf.Clamp(zbegin, room.beginZ + 1, room.endZ - 1);
			xend = Mathf.Clamp(xend, room.beginX + 1, room.endX - 1);
			zend = Mathf.Clamp(zend, room.beginZ + 1, room.endZ - 1);

			for (int k = zbegin; k < zend; k++)
				for (int j = xbegin; j < xend; j++)
				{
					if ((j < 0) || (j >= dungeon.size.width) || (k < 0) || (k >= dungeon.size.width)) continue;
					if ((dungeon.map.GetRoom(j, k) != room.id) || (dungeon.map.GetPool(j, k) > 0)
						|| (dungeon.map.GetDoorway(j, k) > 0)) continue;
					dungeon.map.SetFloorY(floorY, j, k);
				}
		}




		/***********************/
		/*     Rectangles      */
		/***********************/


		// Rectangle (Works)	
		public static readonly Rectangle simple = new Rectangle(1.0f, 1.0f, 0.0f, 0.0f);

		// 'L' (Works)
		public static readonly Rectangle lback000 = new Rectangle(0.5f, 1.0f, -0.25f, 0.0f);
		public static readonly Rectangle lbottom000 = new Rectangle(0.5f, 0.5f, 0.25f, -0.25f);
		public static readonly Rectangle lback090 = new Rectangle(1.0f, 0.5f, 0.0f, -0.25f);
		public static readonly Rectangle lbottom090 = new Rectangle(0.5f, 0.5f, 0.25f, 0.25f);
		public static readonly Rectangle lback270 = new Rectangle(0.5f, 1.0f, 0.25f, 0.0f);
		public static readonly Rectangle lbottom270 = new Rectangle(0.5f, 0.5f, -0.25f, 0.25f);
		public static readonly Rectangle lback180 = new Rectangle(1.0f, 0.5f, 0.0f, -0.25f);
		public static readonly Rectangle lbottom180 = new Rectangle(0.5f, 0.5f, 0.25f, 0.25f);

		// 'O' (Works)	
		public static readonly Rectangle oleft = new Rectangle(0.3333333333f, 1.0f, -0.3333333333f, 0.0f);
		public static readonly Rectangle oright = new Rectangle(0.3333333333f, 1.0f, 0.3333333333f, 0.0f);
		public static readonly Rectangle obottom = new Rectangle(0.3333333333f, 0.3333333333f, 0.0f, -0.3333333333f);
		public static readonly Rectangle otop = new Rectangle(0.3333333333f, 0.3333333333f, 0.0f, 0.3333333333f);

		// 'T' (I think its working -- not sure out its orientation)...
		public static readonly Rectangle ttop000 = new Rectangle(1.0f, 0.3333333333f, 0.0f, 0.3333333333f);
		public static readonly Rectangle tbottom000 = new Rectangle(0.3333333333f, 0.6666666667f, 0.0f, -0.1666666667f);
		public static readonly Rectangle ttop090 = new Rectangle(0.3333333333f, 1.0f, -0.3333333333f, 0.0f);
		public static readonly Rectangle tbottom090 = new Rectangle(0.6666666667f, 0.3333333333f, 0.1666666667f, 0.0f);
		public static readonly Rectangle ttop180 = new Rectangle(1.0f, 0.3333333333f, 0.0f, -0.3333333333f);
		public static readonly Rectangle tbottom180 = new Rectangle(0.3333333333f, 0.6666666667f, 0.0f, 0.1666666667f);
		public static readonly Rectangle ttop270 = new Rectangle(0.3333333333f, 1.0f, 0.3333333333f, 0.0f);
		public static readonly Rectangle tbottom270 = new Rectangle(0.6666666667f, 0.3333333333f, -0.1666666667f, 0.0f);

		// 'E' / 'F' (Working)
		public static readonly Rectangle eback000 = new Rectangle(0.3333333333f, 1.0f, -0.3333333333f, 0.0f);
		public static readonly Rectangle etop000 = new Rectangle(0.6666666667f, 0.2f, 0.1666666667f, 0.4f);
		public static readonly Rectangle emiddle000 = new Rectangle(0.6666666667f, 0.2f, 0.1666666667f, 0.0f);
		public static readonly Rectangle ebottom000 = new Rectangle(0.6666666667f, 0.2f, 0.1666666667f, -0.4f);
		public static readonly Rectangle eback090 = new Rectangle(1.0f, 0.3333333333f, 0.0f, -0.3333333333f);
		public static readonly Rectangle etop090 = new Rectangle(0.2f, 0.6666666667f, -0.4f, 0.1666666667f);
		public static readonly Rectangle emiddle090 = new Rectangle(0.2f, 0.6666666667f, 0.0f, 0.1666666667f);
		public static readonly Rectangle ebottom090 = new Rectangle(0.2f, 0.6666666667f, 0.4f, 0.1666666667f);
		public static readonly Rectangle eback180 = new Rectangle(0.3333333333f, 1.0f, 0.3333333333f, 0.0f);
		public static readonly Rectangle etop180 = new Rectangle(0.6666666667f, 0.2f, -0.1666666667f, 0.4f);
		public static readonly Rectangle emiddle180 = new Rectangle(0.6666666667f, 0.2f, -0.1666666667f, 0.0f);
		public static readonly Rectangle ebottom180 = new Rectangle(0.6666666667f, 0.2f, -0.1666666667f, -0.4f);
		public static readonly Rectangle eback270 = new Rectangle(1.0f, 0.3333333333f, 0.0f, 0.3333333333f);
		public static readonly Rectangle etop270 = new Rectangle(0.2f, 0.6666666667f, 0.4f, -0.1666666667f);
		public static readonly Rectangle emiddle270 = new Rectangle(0.2f, 0.6666666667f, 0.0f, -0.1666666667f);
		public static readonly Rectangle ebottom270 = new Rectangle(0.2f, 0.6666666667f, -0.4f, -0.1666666667f);

		// 'I' / 'H' (Working)
		public static readonly Rectangle itop = new Rectangle(1.0f, 0.3333333333f, 0.0f, 0.3333333333f);
		public static readonly Rectangle imiddle = new Rectangle(0.3333333333f, 0.3333333333f, 0.0f, 0.0f);
		public static readonly Rectangle ibottom = new Rectangle(1.0f, 0.3333333333f, 0.0f, -0.3333333333f);
		public static readonly Rectangle iright = new Rectangle(0.3333333333f, 1.0f, 0.3333333333f, 0.0f);
		public static readonly Rectangle ileft = new Rectangle(0.3333333333f, 1.0f, -0.3333333333f, 0.0f);

		// Plus-shape (Works)
		public static readonly Rectangle crosstop = new Rectangle(0.3333333333f, 0.3333333333f, 0.0f, 0.3333333333f);
		public static readonly Rectangle crossmiddle = new Rectangle(1.0f, 0.3333333333f, 0.0f, 0.0f);
		public static readonly Rectangle crossbottom = new Rectangle(0.3333333333f, 0.3333333333f, 0.0f, -0.3333333333f);

		// 'U' (Works)
		public static readonly Rectangle uleft000 = new Rectangle(0.3333333333f, 0.6666666667f,
																 -0.3333333333f, 0.1666666667f);
		public static readonly Rectangle uright000 = new Rectangle(0.3333333333f, 0.6666666667f,
																  0.3333333333f, 0.1666666667f);
		public static readonly Rectangle ubottom000 = new Rectangle(1.0f, 0.3333333333f,
																  0.0f, -0.3333333333f);
		public static readonly Rectangle uleft090 = new Rectangle(0.6666666667f, 0.3333333333f,
																  0.1666666667f, -0.3333333333f);
		public static readonly Rectangle uright090 = new Rectangle(0.6666666667f, 0.3333333333f,
																  0.1666666667f, 0.3333333333f);
		public static readonly Rectangle ubottom090 = new Rectangle(0.3333333333f, 1.0f,
																 -0.3333333333f, 0.0f);
		public static readonly Rectangle uright180 = new Rectangle(0.3333333333f, 0.6666666667f,
																  0.3333333333f, -0.1666666667f);
		public static readonly Rectangle uleft180 = new Rectangle(0.3333333333f, 0.6666666667f,
																 -0.3333333333f, -0.1666666667f);
		public static readonly Rectangle ubottom180 = new Rectangle(1.0f, 0.3333333333f,
																  0.0f, 0.3333333333f);
		public static readonly Rectangle uright270 = new Rectangle(0.6666666667f, 0.3333333333f,
																 -0.1666666667f, -0.3333333333f);
		public static readonly Rectangle uleft270 = new Rectangle(0.6666666667f, 0.3333333333f,
																 -0.1666666667f, 0.3333333333f);
		public static readonly Rectangle ubottom270 = new Rectangle(0.3333333333f, 1.0f,
																  0.333333333f, 0.0f);

		// 'S' (Working)
		public static readonly Rectangle stop000 = new Rectangle(1.0f, 0.2f, 0.0f, -0.4f);
		public static readonly Rectangle smiddle000 = new Rectangle(1.0f, 0.2f, 0.0f, 0.0f);
		public static readonly Rectangle sbottom000 = new Rectangle(1.0f, 0.2f, 0.0f, 0.4f);
		public static readonly Rectangle sleft000 = new Rectangle(0.2f, 0.2f, -0.4f, 0.2f);
		public static readonly Rectangle sright000 = new Rectangle(0.2f, 0.2f, 0.4f, -0.2f);
		public static readonly Rectangle stop090 = new Rectangle(0.2f, 1.0f, -0.4f, 0.0f);
		public static readonly Rectangle smiddle090 = new Rectangle(0.2f, 1.0f, 0.0f, 0.0f);
		public static readonly Rectangle sbottom090 = new Rectangle(0.2f, 1.0f, 0.4f, 0.0f);
		public static readonly Rectangle sleft090 = new Rectangle(0.2f, 0.2f, -0.2f, 0.4f);
		public static readonly Rectangle sright090 = new Rectangle(0.2f, 0.2f, 0.2f, -0.4f);
	}

}
