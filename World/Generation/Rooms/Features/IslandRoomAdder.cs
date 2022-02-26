using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DLD.Symmetry;


namespace DLD
{

    public class IslandRoomAdder : FeatureAdder
    {
		private bool built;

        public IslandRoomAdder(Degree chance) : base(chance) { }


		public override bool addFeature(Level dungeon, Room room)
		{
			built = dungeon.UseDegree(chance);
			if (built) buildFeature(dungeon, room);
			return built;
		}

		public override void buildFeature(Level dungeon, Room room)
		{
			built = buildSubroom(dungeon, room);
		}


		private bool buildSubroom(Level dungeon, Room room)
		{
			int dimX = (int)((room.endX - room.beginX)
					* (0.2f + (0.3f * dungeon.random.NextFloat())));
			int dimZ = (int)((room.endZ - room.beginZ)
					* (0.2f + (0.3f * dungeon.random.NextFloat())));
			float centerX, centerZ, oppX, oppZ;
			centerX = dungeon.random.NextInt(room.endX - room.beginX) + room.beginX;
			centerZ = dungeon.random.NextInt(room.endZ - room.beginZ) + room.beginZ;
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
				dimX = (int)(dimX * 0.75f);
				dimZ = (int)(dimZ * 0.75f);
			}
			if ((dimX < 5) || (dimZ < 5)) return false;
			int ymod = (dimX <= dimZ) ? (int)Mathf.Sqrt(dimX) : (int)Mathf.Sqrt(dimZ);
			int height = dungeon.random.NextInt(((int)dungeon.style.vertical / 2) + ymod + 1) + 2;
			Room created =
					new RoomSeed((int)centerX, room.floorY,
							(int)centerZ).growRoom(dimX, dimZ, height, dungeon, room, room);
			if (created == null) return false;
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
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)oppZ).growRoom(dimZ, dimX, height, dungeon, room, room);
					}
					break;
				case TR2:
					{
						oppX = room.realX + ((centerZ - room.realZ)
								/ (room.endZ - room.beginZ)) * (room.endX - room.beginX);
						oppZ = room.realZ + ((centerX - room.realX)
								/ (room.endX - room.beginX)) * (room.endZ - room.beginZ);
						oppZ = room.endZ - (oppZ - room.beginZ);
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)oppZ).growRoom(dimZ, dimX, height, dungeon, room, room);
					}
					break;
				case X:
					{
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)centerZ).growRoom(dimX, dimZ, height, dungeon, room, room);
					}
					break;
				case Z:
					{
						created =
								new RoomSeed((int)centerX, room.floorY,
										(int)oppZ).growRoom(dimX, dimZ, height, dungeon, room, room);
					}
					break;
				case XZ:
					{
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)centerZ).growRoom(dimX, dimZ, height, dungeon, room, room);
						created =
								new RoomSeed((int)centerX, room.floorY,
										(int)oppZ).growRoom(dimX, dimZ, height, dungeon, room, room);
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)oppZ).growRoom(dimX, dimZ, height, dungeon, room, room);
					}
					break;
				case R:
					{
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)oppZ).growRoom(dimX, dimZ, height, dungeon, room, room);
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
						created =
								new RoomSeed((int)swX2, room.floorY,
										(int)swZ1).growRoom(dimZ, dimX, height, dungeon, room, room);
						created =
								new RoomSeed((int)swX1, room.floorY,
										(int)swZ2).growRoom(dimZ, dimX, height, dungeon, room, room);
						created =
								new RoomSeed((int)oppX, room.floorY,
										(int)oppZ).growRoom(dimX, dimZ, height, dungeon, room, room);
					}
					break;
			}
			return true;
		}


	}

}
