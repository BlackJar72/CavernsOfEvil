using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nito.Collections;


namespace DLD
{
	public class RoomBFS
	{
		private Deque<Room> roomQueue = new Deque<Room>();
		private Level dungeon;
		private List<Room> nodes;
		private bool[] ok;


		public RoomBFS(Level dungeon)
		{
			this.dungeon = dungeon;
			nodes = new List<Room>(dungeon.nodes.Length);
			ok = new bool[dungeon.roomCount + 1];
			for (int i = dungeon.nodes.Length + 1; i > 0; i--)
			{
				Room room = dungeon.rooms[i];
				if (room.isNode) nodes.Add(room);
			}
		}


		public List<List<Room>> Check()
		{
			List<List<Room>> sections = new List<List<Room>>();
			while (nodes.Count > 0)
			{
				sections.Add(Search(nodes[0]));
			}
			return sections;
		}


		public List<Room> Search(Room room)
		{
			Room next;
			roomQueue.AddToBack(room);
			List<Room> found = new List<Room>();
			while ((nodes.Count > 0) && (roomQueue.Count > 0))
			{
				next = roomQueue[0];
				roomQueue.RemoveAt(0);
				ok[next.id] = true;
				if (next.isNode)
				{
					nodes.Remove(next);
					found.Add(next);
				}
				foreach (DoorQueue exit in next.connections) {
					if (!ok[exit.First.otherside])
						roomQueue.AddToBack(dungeon.rooms[exit.First.otherside]);
				}
			}
			return found;
		}
	}
}
