using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DLD.Symmetry;


namespace DLD
{

    public class PoolAdder : FeatureAdder 
    {
        public PoolAdder(Degree chance) : base(chance) { }


		public override void buildFeature(Level dungeon, Room room)
		{
			float centerX, centerZ, oppX, oppZ;
			float dimX, dimZ;
			int rotation = dungeon.random.NextInt(4);
			Shape[] which;
			dimX = ((room.endX - room.beginX)
					* ((dungeon.random.NextFloat() * 0.25f) + 0.15f));
			dimZ = ((room.endX - room.beginX)
					* ((dungeon.random.NextFloat() * 0.25f) + 0.15f));
			centerX = dungeon.random.NextInt(room.endX - room.beginX - 1) + room.beginX + 1;
			centerZ = dungeon.random.NextInt(room.endZ - room.beginZ - 1) + room.beginZ + 1;
			oppX = room.endX - (centerX - room.beginX);
			oppZ = room.endZ - (centerZ - room.beginZ);
			if (room.symdat.halfX)
			{
				dimX *= 2;
				dimX /= 3;
				oppX = room.endX - ((centerX - room.beginX) / 2);
				centerX = ((centerX - room.beginX) / 2) + room.beginX;
			}
			if (room.symdat.halfZ)
			{
				dimZ *= 2;
				dimZ /= 3;
				oppZ = room.endZ - ((centerZ - room.beginZ) / 2);
				centerZ = ((centerZ - room.beginZ) / 2) + room.beginZ;
			}
			if (room.symdat.doubler)
			{
				dimX *= 0.75f;
				dimZ *= 0.75f;
			}
			centerX++;
			centerZ++;
			oppX++;
			oppZ++;
			if (dungeon.random.NextBool() || !dungeon.UseDegree(dungeon.style.complexity))
			{
				which = Shape.xgroup;
			}
			else
			{
				which = Shape.allshapes[dungeon.random.NextInt(Shape.allshapes.Length)];
			}
			which[rotation].drawLiquid(dungeon, room, room.floorY, centerX, centerZ, dimX, dimZ, false, false);
			// Apply Symmetries
			switch (room.sym)
			{
				case NONE: break;
				case TR1:
					{
						oppX = room.realX + ((centerZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						oppZ = room.realZ + ((centerX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						which[(rotation + 1) % 4].drawLiquid(dungeon, room, room.floorY, 
								oppX,oppZ, dimX, dimZ, false, false);
					}
					break;
				case TR2:
					{
						oppX = room.realX + ((centerZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						oppZ = room.realZ + ((centerX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						oppZ = room.endZ - (oppZ - room.beginZ);
						which[(rotation + 1) % 4].drawLiquid(dungeon, room, room.floorY, 
								oppX, oppZ, dimX, dimZ, false, true);
					}
					break;
				case X:
					{
						which[rotation].drawLiquid(dungeon, room, room.floorY, 
								oppX, centerZ, dimX, dimZ, true, false);
					}
					break;
				case Z:
					{
						which[rotation].drawLiquid(dungeon, room, room.floorY, 
								centerX, oppZ, dimX, dimZ, false, true);
					}
					break;
				case XZ:
					{
						which[rotation].drawLiquid(dungeon, room, room.floorY, 
								oppX, centerZ, dimX, dimZ, true, false);
						which[rotation].drawLiquid(dungeon, room, room.floorY, 
								centerX, oppZ, dimX, dimZ, false, true);
						which[rotation].drawLiquid(dungeon, room, room.floorY, 
								oppX, oppZ, dimX, dimZ, true, true);
					}
					break;
				case R:
					{
						which[(rotation + 2) % 4].drawLiquid(dungeon, room, room.floorY, 
								oppX, oppZ, dimX, dimZ, false, false);
					}
					break;
				case SW:
					{
						float swX1 = room.realX
								+ ((centerZ - room.realZ)
										/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						float swZ1 = room.realZ
								+ ((centerX - room.realX)
										/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						float swX2 = room.realX
								+ ((oppZ - room.realZ)
										/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						float swZ2 = room.realZ
								+ ((oppX - room.realX)
										/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						which[(rotation + 1) % 4].drawLiquid(dungeon, room, room.floorY, 
								swX2, swZ1, dimX, dimZ, false, false);
						which[(rotation + 3) % 4].drawLiquid(dungeon, room, room.floorY, 
								swX1, swZ2, dimX, dimZ, false, false);
						which[(rotation + 2) % 4].drawLiquid(dungeon, room, room.floorY, 
								oppX, oppZ, dimX, dimZ, false, false);
						break;
					}
			}
		}

	}

}
