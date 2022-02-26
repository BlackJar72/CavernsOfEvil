using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DLD.RoomType;


namespace DLD
{

    public class RoomFactory
    {



		/**
		 * A factory method for creating rooms of a random type.  The 
		 * actual type will be based on dungeon-wide theme-derived 
		 * variables.
		 * 
		 * @param beginX
		 * @param endX
		 * @param beginZ
		 * @param endZ
		 * @param floorY
		 * @param ceilY
		 * @param dungeon
		 * @param parent
		 * @param previous
		 * @return
		 */
		public static Room makeRoom(int beginX, int endX, int beginZ, int endZ, int floorY, int ceilY,
				Level dungeon, Room parent, Room previous)
		{
			RoomType type = RoomType.ROOM;
			if (DungeonTheme.UseDegree(dungeon.style.naturals, dungeon.random))
			{
				type = RoomType.CAVE;
			}
			switch (type)
			{
				/*case CAVE:
					{
						Cave base = new Cave(beginX, endX, beginZ, endZ, floorY, ceilY,
								dungeon, parent, previous);
						return base.plan(dungeon, parent);
					}*/
				case ROOM:
				default:
					{
						Room room = new Room(beginX, endX, beginZ, endZ, floorY, ceilY,
							dungeon, parent, previous);
						return room.Plan(dungeon, parent);
					}
			}
		}


		/**
		 * A factory method for creating rooms of a specified type.  Specifying 
		 * and unimplemented type will result in a default room of type ROOM / 
		 * class Room -- that is, a basic room built from shape primitives.
		 * 
		 * This is not currently used, but is kept for possible expansion.
		 * 
		 * @param beginX
		 * @param endX
		 * @param beginZ
		 * @param endZ
		 * @param floorY
		 * @param ceilY
		 * @param dungeon
		 * @param parent
		 * @param previous
		 * @param type
		 * @return
		 */
		public static Room makeRoom(int beginX, int endX, int beginZ, int endZ, int floorY, int ceilY,
				Level dungeon, Room parent, Room previous, RoomType type)
		{
			switch (type)
			{
				/*case CAVE:
					{
						Cave base = new Cave(beginX, endX, beginZ, endZ, floorY, ceilY,
							dungeon, parent, previous);
						return base.plan(dungeon, parent);
					}*/
				case ROOM:
				default:
					{
						Room room = new Room(beginX, endX, beginZ, endZ, floorY, ceilY,
								dungeon, parent, previous);
						return room.Plan(dungeon, parent);
					}
			}
		}

	}

}
