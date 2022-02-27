using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

	public enum Symmetry
	{
		NONE = 0,
		X = 1,
		Z = 2,
		XZ = 3,
		TR1 = 4,
		TR2 = 5,
		R = 6,
		SW = 7
	}


	public struct SymmetryVar
	{
		public SymmetryVar(int level, bool halfX, bool halfZ, bool doubler)
		{
			this.level = level;
			this.halfX = halfX;
			this.halfZ = halfZ;
			this.doubler = doubler;
		}
		public readonly int level;
		public readonly bool halfX;
		public readonly bool halfZ;
		public readonly bool doubler;
	}


	public static class SymTable
	{
		public static readonly SymmetryVar NONE = new SymmetryVar(0, false, false, false);
		public static readonly SymmetryVar X = new SymmetryVar(1, true, false, false);
		public static readonly SymmetryVar Z = new SymmetryVar(1, false, true, false);
		public static readonly SymmetryVar XZ = new SymmetryVar(2, true, true, false);
		public static readonly SymmetryVar TR1 = new SymmetryVar(1, false, false, true);
		public static readonly SymmetryVar TR2 = new SymmetryVar(1, false, false, true);
		public static readonly SymmetryVar R = new SymmetryVar(1, false, false, true);
		public static readonly SymmetryVar SW = new SymmetryVar(2, true, true, false);
		public static readonly SymmetryVar[] data = { NONE, X, Z, XZ, TR1, TR2, R, SW };
		public static SymmetryVar Data(Symmetry s) => data[(int)s];

		// Other Data
		public static readonly Shapes[] allshapes = {Shapes.X, Shapes.L, Shapes.O, Shapes.T,
			Shapes.F, Shapes.E, Shapes.I, Shapes.C, Shapes.U, Shapes.S};
		public static readonly Shapes[] xsymmetics = { Shapes.X, Shapes.O, Shapes.E, Shapes.I, Shapes.C };
		public static readonly Shapes[] ysymmetics = {Shapes.X, Shapes.O, Shapes.T, Shapes.I, Shapes.C,
			Shapes.U};
		public static readonly Shapes[] xysymmetrics = { Shapes.X, Shapes.O, Shapes.I, Shapes.C };
		public static readonly Shapes[] transshapes = { Shapes.X, Shapes.L, Shapes.O, Shapes.C, Shapes.U };
		public static readonly Shapes[] rotateds = { Shapes.X, Shapes.O, Shapes.C, Shapes.S };

		public static readonly Shapes[] allPart = {Shapes.L, Shapes.O, Shapes.T,
			Shapes.F, Shapes.E, Shapes.I, Shapes.C, Shapes.U, Shapes.S};
		public static readonly Shapes[] xsymmetricPart = { Shapes.O, Shapes.E, Shapes.I, Shapes.C };
		public static readonly Shapes[] ysymmetricPart = { Shapes.O, Shapes.T, Shapes.I, Shapes.C, Shapes.U };
		public static readonly Shapes[] xysymmetricPart = { Shapes.O, Shapes.I, Shapes.C };
		public static readonly Shapes[] transPart = { Shapes.L, Shapes.O, Shapes.C, Shapes.U };
		public static readonly Shapes[] rotatedPart = { Shapes.X, Shapes.O, Shapes.C, Shapes.S };

		public static readonly Shapes[] allSolids = {Shapes.X, Shapes.L, Shapes.T,
			Shapes.F, Shapes.E, Shapes.I, Shapes.C, Shapes.U, Shapes.S};
		public static readonly Shapes[] xsymmetricSolid = { Shapes.X, Shapes.E, Shapes.I, Shapes.C };
		public static readonly Shapes[] ysymmetricSolid = { Shapes.X, Shapes.T, Shapes.I, Shapes.C, Shapes.U };
		public static readonly Shapes[] xysymmetricSolid = { Shapes.X, Shapes.I, Shapes.C };
		public static readonly Shapes[] transSolid = { Shapes.X, Shapes.L, Shapes.C, Shapes.U };
		public static readonly Shapes[] rotatedSolid = { Shapes.X, Shapes.C, Shapes.S };


		/**
		 * Returns a Symmetry constant based on the dungeons degree of
		 * symmetry; used to get the symmetry type for individual rooms. 
		 * 
		 * @param dungeon
		 * @return a Symmetry constant repressenting a type of semmetry
		 */
		public static Symmetry GetSymmetry(Level dungeon)
		{
			int num = 0;
			if (dungeon.UseDegree(dungeon.style.symmetry)) num += 1;
			if (dungeon.UseDegree(dungeon.style.symmetry)) num += 1;
			if (num == 0) return Symmetry.NONE;
			int which = dungeon.random.NextInt(4 / num);
			if (num == 1) switch (which)
				{
					case 0: return Symmetry.X;
					case 1: return Symmetry.Z;
					case 2:
						if (dungeon.random.NextBool())
						{
							return Symmetry.TR1;
						}
						else
						{
							return Symmetry.TR2;
						}
					case 3: return Symmetry.R;
					default: return Symmetry.NONE;
				}
			else switch (which)
				{
					case 0: return Symmetry.XZ;
					case 1: return Symmetry.SW;
					default: return Symmetry.NONE;
				}
		}
	}

}
