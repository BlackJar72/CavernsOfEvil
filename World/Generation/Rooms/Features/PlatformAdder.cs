using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DLD.Symmetry;


namespace DLD
{

    public class PlatformAdder : FeatureAdder
    {
		protected bool isDepression;

		public PlatformAdder(Degree chance) : base(chance) 
		{
			isDepression = false;
		}


		public override void buildFeature(Level dungeon, Room room)
		{
			int available = room.ceilY - room.floorY;
			if (available < 4) return;
			float dimX, dimZ, centerX, centerZ, oppX, oppZ;
			int platY;
			int rotation = dungeon.random.NextInt(4);
			Shape[] which;
			dimX = ((room.endX - room.beginX) * ((dungeon.random.NextFloat() * 0.25f) + 0.15f));
			dimZ = ((room.endX - room.beginX) * ((dungeon.random.NextFloat() * 0.25f) + 0.15f));
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
			if (isDepression)
			{
				available -= 2;
				if (available > (((int)dungeon.style.vertical / 2) + 1))
					available = (((int)dungeon.style.vertical / 2) + 1);
				int depth = 1;
				if (dungeon.UseDegree(chance)) depth++;
				platY = (room.floorY - depth);
			}
			else
			{
				platY = (room.floorY + 1 + (dungeon.random.NextInt(2)));
				if (available > 4) platY += (byte)(dungeon.random.NextInt(available - 3));
			}
			if (dungeon.random.NextBool() || !dungeon.UseDegree(dungeon.style.complexity))
			{
				which = Shape.xgroup;
			}
			else
			{
				which = Shape.allSolids[dungeon.random.NextInt(Shape.allSolids.Length)];
			}			
			which[rotation].drawPlatform(dungeon, room, platY, centerX,
					centerZ, dimX, dimZ, false, false);
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
						which[(rotation + 3) % 4].drawPlatform(dungeon, room, platY, oppX,
								oppZ, dimX, dimZ, false, false);
					}
					break;
				case TR2:
					{
						oppX = room.realX + ((centerZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						oppZ = room.realZ + ((centerX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						oppZ = room.endZ - (oppZ - room.beginZ);
						which[(rotation + 3) % 4].drawPlatform(dungeon, room, platY, oppX,
								oppZ, dimX, dimZ, false, true);
					}
					break;
				case X:
					{
						which[rotation].drawPlatform(dungeon, room, platY, oppX,
								centerZ, dimX, dimZ, true, false);
					}
					break;
				case Z:
					{
						which[rotation].drawPlatform(dungeon, room, platY, centerX,
								oppZ, dimX, dimZ, false, true);
					}
					break;
				case XZ:
					{
						which[rotation].drawPlatform(dungeon, room, platY, oppX,
								centerZ, dimX, dimZ, true, false);
						which[rotation].drawPlatform(dungeon, room, platY, centerX,
								oppZ, dimX, dimZ, false, true);
						which[rotation].drawPlatform(dungeon, room, platY, oppX,
								oppZ, dimX, dimZ, true, true);
					}
					break;
				case R:
					{
						which[(rotation + 2) % 4].drawPlatform(dungeon, room, platY, oppX,
								oppZ, dimX, dimZ, false, false);
					}
					break;
				case SW:
					{
						float swX1 = room.realX + ((centerZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						float swZ1 = room.realZ + ((centerX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						float swX2 = room.realX + ((oppZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						float swZ2 = room.realZ + ((oppX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						which[(rotation + 1) % 4].drawPlatform(dungeon, room, platY, swX2, swZ1,
								dimX, dimZ, false, false);
						which[(rotation + 3) % 4].drawPlatform(dungeon, room, platY, swX1, swZ2,
								dimX, dimZ, false, false);
						which[(rotation + 2) % 4].drawPlatform(dungeon, room, platY, oppX, oppZ,
								dimX, dimZ, false, false);
						break;
					}
			}
		}
	}

}