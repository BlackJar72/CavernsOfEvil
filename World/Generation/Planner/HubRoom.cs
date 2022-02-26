using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class HubRoom
    {
		public Room theRoom;

		public Room TheRoom { get => theRoom; }

		public HubRoom(int x, int y, int z, Xorshift random, Level dungeon)
		{
			// Nodes should be on the larger end of the size scale for rooms...
			int xdim = random.NextInt((dungeon.size.maxRoomSize / 2) - 3)
					+ (dungeon.size.maxRoomSize / 2) + 4;
			int zdim = random.NextInt((dungeon.size.maxRoomSize / 2) - 3)
					+ (dungeon.size.maxRoomSize / 2) + 4;
			int ymod = (xdim <= zdim) ? (int)Mathf.Sqrt(xdim) : (int)Mathf.Sqrt(zdim);

			int height = random.NextInt(((int)dungeon.style.vertical / 2) + ymod) + 3;

			// Then plant a seed and try to grow the room
			theRoom = new RoomSeed(x, y, z).growRoom(xdim, zdim, height, dungeon, null, null);
			if (theRoom == null) TryAgain(y, random, dungeon);
			if (theRoom != null) theRoom.FixHub();
		}


		public HubRoom(TestRoom room)
		{
			theRoom = room;
		}

		private void TryAgain(int y, Xorshift random, Level dungeon)
        {
			int tries = 0;
			while((theRoom == null) && (tries++ < 12))
            {
				int x = dungeon.random.NextInt(dungeon.size.width);
				int z = dungeon.random.NextInt(dungeon.size.width);
				// Nodes should be on the larger end of the size scale for rooms...
				int xdim = random.NextInt((dungeon.size.maxRoomSize / 2) - 3)
						+ (dungeon.size.maxRoomSize / 2) + 4;
				int zdim = random.NextInt((dungeon.size.maxRoomSize / 2) - 3)
						+ (dungeon.size.maxRoomSize / 2) + 4;
				int ymod = (xdim <= zdim) ? (int)Mathf.Sqrt(xdim) : (int)Mathf.Sqrt(zdim);

				int height = random.NextInt(((int)dungeon.style.vertical / 2) + ymod) + 3;

				// Then plant a seed and try to grow the room
				theRoom = new RoomSeed(x, y, z).growRoom(xdim, zdim, height, dungeon, null, null);
			}
        }


	}

}