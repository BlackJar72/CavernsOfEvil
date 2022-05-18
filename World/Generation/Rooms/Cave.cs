using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

	/**
	 * This class represents alternative caves created by cellular automata 
	 * with a "2 1/2 D" architecture.
	 * 
	 * The algorithm uses is based on one originally created for Rogue-like games
	 * and a variation is also use by the Oblige level generator for creating 
	 * "natural" areas in Doom levels.
	 * 
	 *  Googling "cellular automata caves" will produce many relevant results.
	 * 
	 * @author Jared Blackburn (JaredBGreat)
	 */
	public class Cave : Room
	{
		static readonly float[] HEIGHT_TABLE;
		static readonly float[] SCALE_TABLE;

		private static float[,] ceilNoise;
		private static float[,] floorNoise;

		private int xSize, zSize, factor;
		private int[,] cells;
		private int[,] scratchpad;
		private float[,] height;
		private float[,] scale;
		private Vector3[,] floorverts;
		private Vector3[,] ceilverts;

		private int sum;


		static Cave()
        {
			HEIGHT_TABLE = new float[11];
			SCALE_TABLE = new float[11];
			float fifthroot = Mathf.Pow(10, 0.2f);
			for (int i = 1; i < 11; i++)
            {
				HEIGHT_TABLE[i] = Mathf.Pow(fifthroot, i) / 100;
				SCALE_TABLE[i] = ((float)i / 10f); ;
			}
			// The value for 0 should not be what might be expected mathematically
			HEIGHT_TABLE[0] = 0f;
			SCALE_TABLE[0] = 0.05f;

		}


        #region Room Generation
        public Cave(int beginX, int endX, int beginZ, int endZ, int floorY,
				int ceilY, Level dungeon, Room parent, Room previous, bool isBigRoom = false)
				: base(beginX, endX, beginZ, endZ, floorY, ceilY, dungeon, previous, previous) {}


		public static void InitForLevel()
        {
			HeightMap2D noise = new HeightMap2D(256, 256, 16, 1);
			ceilNoise = noise.Process(GameData.Xrandom);
			floorNoise = noise.Process(GameData.Xrandom);
		}


		///<summary>
		/// Plans the room.  Chests and spawner are added in the same way 
		/// as for basic rooms, but walls and variation in floor / ceiling
		/// height are determined by cellular automata to produce a more
		/// cave-like layout.
		///</summary>
		public override Room Plan(Level dungeon, Room parent)
		{
			xSize = endX - beginX;
			zSize = endZ - beginZ;
			if((xSize % 2 == 0) && (zSize % 2 == 0)) {
				if (isBigRoom && (xSize % 4 == 0) && (zSize % 4 == 0) 
					&& dungeon.random.NextBool())
				{
					MakeBetterMap(dungeon, 2, xSize, zSize);
				}
				else MakeBetterMap(dungeon, 1, xSize, zSize);
			}
			else MakeFineMap(dungeon, parent, xSize, zSize);
			return this;
		}


		private void MakeFineMap(Level dungeon, Room parent, int xSize, int zSize)
		{
			factor = 1;
			cells = new int[xSize, zSize];
			for (int i = 0; i < xSize; i++)
			{
				for (int j = 0; j < zSize; j++)
				{
					cells[i, j] = dungeon.random.NextInt(2);
				}
			}
			LayerConvert1(cells, 5 + dungeon.random.NextInt(2), dungeon);
		}


		private void MakeBetterMap(Level dungeon, int expandsions, int xSize, int zSize)
		{
			factor = (int)Mathf.Pow(2, expandsions);
			xSize /= factor;
			zSize /= factor;
			cells = new int[xSize, zSize];
			for (int i = 0; i < xSize; i++)
			{
				for (int j = 0; j < zSize; j++)
				{
					cells[i, j] = dungeon.random.NextInt(2);
				}
			}
			for (int i = 0; i < expandsions; i++) 
			{
				LayerConvert2(cells, 5 + dungeon.random.NextInt(2), dungeon);
				cells = Expand(cells, dungeon.random);
				xSize *= 2;
				zSize *= 2;
				factor /= 2;
			}
			LayerConvert1(cells, 5 + dungeon.random.NextInt(2), dungeon);
		}


		public void MakeFloorNCielingData(Level dungeon)
        {
			float roomHeight = ceilY - floorY;
			float fheightf = roomHeight * 0.625f;
			float cheightf = roomHeight * 0.75f;
			float floorScale = Mathf.Min((float)dungeon.style.vertical, roomHeight / 4);
			float ceilScale = Mathf.Min(Mathf.Max(Mathf.Min((float)dungeon.style.vertical, roomHeight / 2), 
				(roomHeight - floorScale) / 4) + 2, roomHeight - 3 - floorScale);
			Summate(dungeon);
			MakeHeightNScale(dungeon);
			floorverts = new Vector3[height.GetLength(0), height.GetLength(1)];
			ceilverts = new Vector3[height.GetLength(0), height.GetLength(1)];
			for (int i = 0; i < height.GetLength(0); i++)
				for(int j = 0; j < height.GetLength(1); j++)
                {
					floorverts[i, j] = new Vector3(i + beginX, 
						(scale[i, j] * floorNoise[i + beginX, j + beginX] * floorScale) 
							+ (height[i, j] * fheightf), 
						j + beginZ);
					ceilverts[i, j] = new Vector3(i + beginX,
						ceilY - (scale[i, j] * ceilNoise[i + beginX, j + beginX] * ceilScale)
							- (height[i, j] * cheightf),
						j + beginZ);
				}
		}


		/// <summary>
		/// This should be called after other dungeons generation (notable, AStar based QC) has been run.
		/// I will convert the data generated through celluar automata into factors that can be used in 
		/// generating height maps for the floor and cieling terrain meshes.
		/// </summary>
		/// <param name="dungeon"></param>
		public void MakeHeightNScale(Level dungeon)
		{			
			float[,,] tiledat = new float[2, xSize, zSize];
			for(int i = 0; i < xSize; i++)
				for(int j = 0; j < zSize; j++)
                {
					tiledat[0, i, j] = HEIGHT_TABLE[cells[i, j]];
					tiledat[1, i, j] = SCALE_TABLE[cells[i, j]];
				}
			height = new float[xSize + 1, zSize + 1];
			scale = new float[xSize + 1, zSize + 1];
			float[] localscratch = new float[4];
			for (int i = 0; i < scale.GetLength(0); i++)
				for(int j = 0;j < scale.GetLength(1); j++)
                {
					localscratch[0] = GetNumberForSquare(i - 1, j - 1, dungeon, tiledat);
					localscratch[1] = GetNumberForSquare(i, j - 1, dungeon, tiledat);
					localscratch[2] = GetNumberForSquare(i - 1, j, dungeon, tiledat);
					localscratch[3] = GetNumberForSquare(i, j, dungeon, tiledat);
					if((localscratch[0] > 1.1) || (localscratch[1] > 1.1) ||
						(localscratch[2] > 1.1) || (localscratch[3] > 1.1))
                    {
						height[i, j] = 0;
						scale[i, j] = 0;
                    }
					else if ((localscratch[0] < -0.1f) || (localscratch[1] < -0.1f) ||
						(localscratch[2] < -0.1f) || (localscratch[3] < -0.1f))
					{
						height[i, j] = 0;
						scale[i, j] = 0;
					}
					else
					{
						height[i, j] = (localscratch[0] + localscratch[1] + localscratch[2] + localscratch[3]) / 4;
						localscratch[0] = GetScaleForSquare(i - 1, j - 1, tiledat);
						localscratch[1] = GetScaleForSquare(i, j - 1, tiledat);
						localscratch[2] = GetScaleForSquare(i - 1, j, tiledat);
						localscratch[3] = GetScaleForSquare(i, j, tiledat);
						scale[i, j] = (localscratch[0] + localscratch[1] + localscratch[2] + localscratch[3]) / 4;
					}
				}
        }


		private float GetNumberForSquare(int i, int j, Level dungeon, float[,,] tiledat)
        {
			int x = i + xSize; int z = j + zSize;
			if((x < 0) || (z < 0) 
				|| (x >=dungeon.size.width) || (z >= dungeon.size.width)) return 2;
			if (dungeon.map.GetAStared(x, z) || (dungeon.map.GetDoorway(x, z) > 0)) return -1;
			if ((i < 0) || (j < 0)
				|| (i >= tiledat.GetLength(1)) || (j >= tiledat.GetLength(2))) return 1;
			return tiledat[0, i, j];
		}


		private float GetScaleForSquare(int i, int j, float[,,] tiledat)
		{
			if ((i < 0) || (j < 0)
				|| (i >= tiledat.GetLength(1)) || (j >= tiledat.GetLength(2))) return 0;
			return tiledat[1, i, j];
		}


		/**
		 * Use cellular automata to find area that should be walls.
		 * 
		 * @param layer
		 * @param thresshold
		 * @return
		 */
		private void LayerConvert1(int[,] layer, int thresshold, Level dungeon)
		{
			MakeScratchpad(dungeon.map, layer);
			for (int i = layer.GetLength(0) - 2; i > 0; i--)
			{
				for (int j = layer.GetLength(1) - 2; j > 0; j--)
				{
					ProcessCell1(layer, i, j, thresshold);
				}
			}
			for (int i = 0; i > xSize; i++)
			{
				for (int j = 0; j > zSize; j++)
				{
					cells[i, j] = scratchpad[i, j];
				}
			}
		}


		/**
		 * Use cellular automata to find area that should be walls.
		 * 
		 * @param layer
		 * @param thresshold
		 * @return
		 */
		private void LayerConvert2(int[,] layer, int thresshold, Level dungeon)
		{
			MakeScratchpad(dungeon.map, layer);
			for (int i = layer.GetLength(0) - 2; i > 0; i--)
			{
				for (int j = layer.GetLength(1) - 2; j > 0; j--)
				{
					ProcessCell2(layer, i, j, thresshold);
				}
			}
			for (int i = 0; i > xSize; i++)
			{
				for (int j = 0; j > zSize; j++)
				{
					cells[i, j] = scratchpad[i, j];
				}
			}
		}


		/**
		 * This process the cells for a tile, convert it to one or zero based on 
		 * the sum of it and its neighbors.  Note that most implementation do not
		 * consider the initial value of the cell itself, but I get good results 
		 * doing so and it simplifies the code, so I do here.
		 * 
		 * @param layer
		 * @param x
		 * @param z
		 * @param thresshold
		 */
		private void ProcessCell1(int[,] layer, int x, int z, int thresshold)
		{
			sum = 0;
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					sum += layer[i,j];
				}
			}
			if (sum >= thresshold)
			{
				scratchpad[x,z] = 1;
			}
			else
			{
				scratchpad[x,z] = 0;
			}
		}


		/**
		 * This process the cells for a tile, convert it to one or zero based on 
		 * the sum of it and its neighbors.  Note that most implementation do not
		 * consider the initial value of the cell itself, but I get good results 
		 * doing so and it simplifies the code, so I do here.
		 * 
		 * @param layer
		 * @param x
		 * @param z
		 * @param thresshold
		 */
		private void ProcessCell2(int[,] layer, int x, int z, int thresshold)
		{
			sum = 0;
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					sum += layer[i, j];
				}
			}
			if (sum > thresshold)
			{
				scratchpad[x, z] = 1;
			}
			else if (sum < thresshold)
			{
				scratchpad[x, z] = 0;
			}
			else
			{
				scratchpad[x, z] = -1;
			}
		}


		private int[,] Expand(int[,] layer, Xorshift random)
        {
			int[,] output = new int[layer.GetLength(0) * 2, layer.GetLength(1) * 2];
			for(int i = 0; i < output.GetLength(0); i++)
				for(int j = 0; j < output.GetLength(1); j++)
                {
					if(layer[i / 2, j / 2] < 0) output[i, j] = random.NextInt(2);
					else output[i,j] = layer[i / 2, j / 2];
                }
			return output;
        }


		private void Summate(Level dungeon)
        {
            MakeScratchpad10(dungeon.map);
			for (int i = xSize - 2; i > 0; i--)
			{
				for (int j = zSize - 2; j > 0; j--)
				{
					scratchpad[i, j] = SummateCell(i, j);
				}
			}
			for (int i = xSize - 2; i > 0; i--)
			{
				for (int j = zSize - 2; j > 0; j--)
				{
					cells[i, j] = scratchpad[i, j];
				}
			}
		}


		private int SummateCell(int x, int z)
		{
			sum = cells[x, z];
			for (int i = x - 1; i <= x + 1; i++)
			{
				for (int j = z - 1; j <= z + 1; j++)
				{
					sum += cells[i, j];
				}
			}
			return sum;
		}


		/**
		 * Initialize the classes internal (read, private) scratchpad for a 
		 * new round of cellular automata processing.
		 */
		private void MakeScratchpad(MapMatrix map, int[,] layer)
		{
			int xs = layer.GetLength(0), zs = layer.GetLength(1);
			scratchpad = new int[xs, zs];
			for (int i = 0; i < xs; i++)
			{
				scratchpad[i, 0] = 0;
				scratchpad[i, zs - 1] = 0;
			}
			for (int i = 0; i < zs; i++)
			{
				scratchpad[0, i] = 0;
				scratchpad[xs - 1, i] = 0;
			}
		}


		/**
		 * Initialize the classes internal (read, private) scratchpad for a 
		 * new round of cellular automata processing.
		 */
		private void MakeScratchpad10(MapMatrix map)
		{
			scratchpad = new int[xSize, zSize];
			for (int i = 0; i < xSize; i++)
			{
				scratchpad[i, 0] = Mathf.Max(1 - map.GetDoorway((i * factor)
					+ beginX, beginZ), 0) * 10;
				scratchpad[i, zSize - 1] = Mathf.Max(1 - map.GetDoorway((i * factor)
					+ beginX, ((zSize * factor) - 1) + beginZ), 0) * 10;
			}
			for (int i = 0; i < zSize; i++)
			{
				scratchpad[0, i] = Mathf.Max(1 - map.GetDoorway(beginX, (i * factor)
					+ beginZ), 0) * 10;
				scratchpad[xSize - 1, i] = Mathf.Max(1 - map.GetDoorway(((xSize * factor) - 1)
					+ beginX, (i * factor) + beginZ), 0) * 10;
			}
		}
        #endregion


        #region Meshing
		public Mesh BuildCaveFloor(Mesh mesh)
        {
			floor.GetComponent<Mesher>();
			int width = floorverts.GetLength(0);
			int numverts = floorverts.GetLength(0) * floorverts.GetLength(1);
			int numtiles = cells.GetLength(0) * cells.GetLength(1);
			Vector3[] verts = new Vector3[numverts];
			Vector3[] norms = new Vector3[numverts];
			int[] corners = new int[numtiles * 6];
			for(int i = 0; i < numverts; i++)
            {
				verts[i] = floorverts[i % width, i / width];
            }
			for(int i = 0;i < numtiles; i++)
            {
				int q = i * 6;
				int x = i % cells.GetLength(0);
				int z = i / cells.GetLength(0);
				corners[q + 0] = x + (z * width);
				corners[q + 1] = x + ((z + 1) * width);
				corners[q + 2] = (x + 1) + (z * width); 
				corners[q + 3] = (x + 1) + (z * width);
				corners[q + 4] = x + ((z + 1) * width); 
				corners[q + 5] = (x + 1) + ((z + 1) * width);
			}
			mesh.vertices = verts;
			mesh.normals = norms;
			mesh.triangles = corners;
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			return mesh;
		}


		public Mesh BuildCaveRoof(Mesh mesh)
		{
			ceiling.GetComponent<Mesher>();
			int width = ceilverts.GetLength(0);
			int numverts = ceilverts.GetLength(0) * ceilverts.GetLength(1);
			int numtiles = cells.GetLength(0) * cells.GetLength(1);
			Vector3[] verts = new Vector3[numverts];
			Vector3[] norms = new Vector3[numverts];
			int[] corners = new int[numtiles * 6];
			for (int i = 0; i < numverts; i++)
			{
				verts[i] = ceilverts[i % width, i / width];
			}
			for (int i = 0; i < numtiles; i++)
			{
				int q = i * 6;
				int x = i % cells.GetLength(0);
				int z = i / cells.GetLength(0);
				corners[q + 0] = x + (z * width);
				corners[q + 1] = (x + 1) + (z * width); 
				corners[q + 2] = x + ((z + 1) * width);
				corners[q + 3] = (x + 1) + (z * width);
				corners[q + 4] = (x + 1) + ((z + 1) * width); 
				corners[q + 5] = x + ((z + 1) * width);
			}
			mesh.vertices = verts;
			mesh.normals = norms;
			mesh.triangles = corners;
			mesh.RecalculateNormals();
			mesh.RecalculateTangents();
			return mesh;
		}

		#endregion


	}


}