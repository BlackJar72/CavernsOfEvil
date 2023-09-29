using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CevarnsOfEvil.Rectangle;
using static CevarnsOfEvil.Shape;


namespace CevarnsOfEvil
{

    public class Shape
    {
		private readonly Rectangle[] rectangles;	
	
		public Shape(Rectangle[] rectangles)
			{
				this.rectangles = rectangles;
			}





		/**
		 * Add a pool of "liquid" in this shape to the dungeon.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawLiquid(Level dungeon, Room room, int height, float sx, float sz, float sdimx, float sdimz,
				bool invertX, bool invertZ)
		{
			foreach (Rectangle rect in rectangles)
			{
				rect.drawLiquid(dungeon, room, height, sx, sz, sdimx, sdimz, invertX, invertZ);
			}
		}


		/**
		 * Add a walkway (normal floor at normal height) through a pool of 
		 * previously added liquid.  This is mostly foreacheachuse with whole room
		 * shapes.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawWalkway(Level dungeon, Room room, float sx, float sz, byte sdimx, byte sdimz,
					bool invertX, bool invertZ)
		{
			foreach (Rectangle rect in rectangles)
			{
				rect.drawWalkway(dungeon, room, sx, sz, sdimx, sdimz, invertX, invertZ);
			}
		}


		/**
		 * Effectively remove an area of in the shape from the dungeon by filling it 
		 * with walls.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawCutout(Level dungeon, Room room, float sx, float sz, float sdimx, float sdimz,
				bool invertX, bool invertZ)
		{
			foreach (Rectangle rect in rectangles)
			{
				rect.drawCutout(dungeon, room, sx, sz, sdimx, sdimz, invertX, invertZ);
			}
		}


		/**
		 * Removes walls from an area of this shape (cuts into the wall); this is
		 * mostly foreacheachuse with whole room shapes.
		 * 
		 * @param dungeon
		 * @param room
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawCutin(Level dungeon, Room room,
						  float sx, float sz, byte sdimx, byte sdimz, bool invertX, bool invertZ)
		{
			foreach (Rectangle rect in rectangles)
			{
				rect.drawCutin(dungeon, room, sx, sz, sdimx, sdimz, invertX, invertZ);
			}
		}


		/**
		 * Adds a platforeachm (or depression) of this shape.
		 * 
		 * @param dungeon
		 * @param room
		 * @param floorY the new floor height
		 * @param sx the x coordinate
		 * @param sz the z coordinate
		 * @param sdimx the length on x
		 * @param sdimz the length on z
		 * @param invertX
		 * @param invertZ
		 */
		public void drawPlatform(Level dungeon, Room room, int floorY,
						  float sx, float sz, float sdimx, float sdimz, bool invertX, bool invertZ)
		{
			foreach (Rectangle rect in rectangles)
			{
				rect.drawPlatform(dungeon, room, floorY, sx, sz, sdimx, sdimz, invertX, invertZ);
			}
		}



		/***********************/
		/*       Shapes        */
		/***********************/

		// Rectangle (Works)
		public static readonly Shape simpleRect = new Shape(new Rectangle[] { simple });
		public static readonly Shape X = simpleRect;
		public static readonly Shape[] xgroup = { X, X, X, X };

		// 'L' (Works)
		public static readonly Shape L000 = new Shape(new Rectangle[] { lback000, lbottom000 });
		public static readonly Shape L090 = new Shape(new Rectangle[] { lback090, lbottom090 });
		public static readonly Shape L180 = new Shape(new Rectangle[] { lback180, lbottom180 });
		public static readonly Shape L270 = new Shape(new Rectangle[] { lback270, lbottom270 });
		public static readonly Shape[] lgroup = { L000, L090, L180, L270 };

		// 'O' (Works)
		public static readonly Shape O = new Shape(new Rectangle[] { obottom, oleft, oright, otop });
		public static readonly Shape[] ogroup = { O, O, O, O };

		// 'T' (I think its working -- not sure out its orientation)...
		public static readonly Shape T000 = new Shape(new Rectangle[] { ttop000, tbottom000 });
		public static readonly Shape T090 = new Shape(new Rectangle[] { ttop090, tbottom090 });
		public static readonly Shape T180 = new Shape(new Rectangle[] { ttop180, tbottom180 });
		public static readonly Shape T270 = new Shape(new Rectangle[] { ttop270, tbottom270 });
		public static readonly Shape[] tgroup = { T000, T090, T180, T270 };

		// 'F' (Working)
		public static readonly Shape F000 = new Shape(new Rectangle[] { eback000, etop000, emiddle000 });
		public static readonly Shape F090 = new Shape(new Rectangle[] { eback090, etop090, emiddle090 });
		public static readonly Shape F180 = new Shape(new Rectangle[] { eback180, etop180, emiddle180 });
		public static readonly Shape F270 = new Shape(new Rectangle[] { eback270, etop270, emiddle270 });
		public static readonly Shape[] fgroup = { F000, F090, F180, F270 };

		// 'E' (Works)
		public static readonly Shape E000 = new Shape(new Rectangle[] { eback000, etop000, emiddle000, ebottom000 });
		public static readonly Shape E090 = new Shape(new Rectangle[] { eback090, etop090, emiddle090, ebottom090 });
		public static readonly Shape E180 = new Shape(new Rectangle[] { eback180, etop180, emiddle180, ebottom180 });
		public static readonly Shape E270 = new Shape(new Rectangle[] { eback270, etop270, emiddle270, ebottom270 });
		public static readonly Shape[] egroup = { E000, E090, E180, E270 };

		// 'I' / 'H' (Working)
		public static readonly Shape I000 = new Shape(new Rectangle[] { itop, imiddle, ibottom });
		public static readonly Shape I090 = new Shape(new Rectangle[] { ileft, imiddle, iright });
		public static readonly Shape I180 = I000;
		public static readonly Shape I270 = I090;
		public static readonly Shape[] igroup = { I000, I090, I180, I270 };

		// Plus-shape (Works)
		public static readonly Shape CROSS = new Shape(new Rectangle[] { crosstop, crossmiddle, crossbottom });
		public static readonly Shape[] cgroup = { CROSS, CROSS, CROSS, CROSS };

		// 'U' (Works)
		public static readonly Shape U000 = new Shape(new Rectangle[] { uleft000, uright000, ubottom000 });
		public static readonly Shape U090 = new Shape(new Rectangle[] { uleft090, uright090, ubottom090 });
		public static readonly Shape U180 = new Shape(new Rectangle[] { uleft180, uright180, ubottom180 });
		public static readonly Shape U270 = new Shape(new Rectangle[] { uleft270, uright270, ubottom270 });
		public static readonly Shape[] ugroup = { U000, U090, U180, U270 };

		// 'S' (Works)
		public static readonly Shape S000 = new Shape(new Rectangle[]{stop000, smiddle000, sbottom000,
																sleft000, sright000});
		public static readonly Shape S090 = new Shape(new Rectangle[]{stop090, smiddle090, sbottom090,
																sleft090, sright090});
		public static readonly Shape S180 = S000;
		public static readonly Shape S270 = S090;
		public static readonly Shape[] sgroup = { S000, S090, S180, S270 };



		public static readonly Shape[][] allshapes = {xgroup, lgroup, ogroup, tgroup, fgroup,
											   egroup, igroup, cgroup, ugroup, sgroup};

		public static readonly Shape[][] allSolids = {xgroup, lgroup, tgroup, fgroup,
											   egroup, ugroup};

	}


	public enum Shapes
    {
		X = 0,
		L = 1,
		O = 2,
		T = 3,
		F = 4,
		E = 5,
		I = 6,
		C = 7,
		U = 8,
		S = 9
	}


	public struct ShapeData
    {
		public readonly int minx, miny;
		public readonly Shape[] family;
		public ShapeData(int minx, int miny, Shape[] family)
        {
			this.minx = minx;
			this.miny = miny;
			this.family = family;
        }
    }


	public static class ShapeDataTables
    {
		public static readonly ShapeData x = new ShapeData(1, 1, xgroup);
		public static readonly ShapeData l = new ShapeData(2, 2, lgroup);
		public static readonly ShapeData o = new ShapeData(3, 3, ogroup);
		public static readonly ShapeData t = new ShapeData(3, 3, tgroup);
		public static readonly ShapeData f = new ShapeData(4, 5, fgroup);
		public static readonly ShapeData e = new ShapeData(4, 5, egroup);
		public static readonly ShapeData i = new ShapeData(3, 3, igroup);
		public static readonly ShapeData c = new ShapeData(3, 3, cgroup);
		public static readonly ShapeData u = new ShapeData(3, 3, ugroup);
		public static readonly ShapeData s = new ShapeData(5, 5, sgroup);

		public static readonly ShapeData[] data = { x, l, o, t, f, e, i, c, u, s };

		public static ShapeData Data(Shapes value) => data[(int)value];


		/**
		 * Will return a random shape that fits a given symmetry.
		 * 
		 * @param sym
		 * @param random
		 * @return
		 */
		public static ShapeData wholeShape(Symmetry symmetry, Xorshift random)
		{
			SymmetryVar sym = SymTable.Data(symmetry);
			switch (symmetry)
			{
				case Symmetry.NONE:
					return Data(SymTable.allPart[random.NextInt(SymTable.allPart.Length)]);
				case Symmetry.R:
				case Symmetry.SW:
					return Data(SymTable.rotatedPart[random.NextInt(SymTable.rotatedPart.Length)]);
				case Symmetry.TR1:
				case Symmetry.TR2:
					return Data(SymTable.transPart[random.NextInt(SymTable.transPart.Length)]);
				case Symmetry.X:
					return Data(SymTable.xsymmetricPart[random.NextInt(SymTable.xsymmetricPart.Length)]);
				case Symmetry.XZ:
					return Data(SymTable.xysymmetricPart[random.NextInt(SymTable.xysymmetricPart.Length)]);
				case Symmetry.Z:
					return Data(SymTable.ysymmetricPart[random.NextInt(SymTable.ysymmetricPart.Length)]);
				default:
					return Data(SymTable.allPart[random.NextInt(SymTable.allPart.Length)]);
			}
		}

	}

}