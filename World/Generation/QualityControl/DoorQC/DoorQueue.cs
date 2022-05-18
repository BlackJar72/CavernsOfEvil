using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PriorityQueue;


namespace CevarnsOfEvil
{

    public class DoorQueue : PriorityQueue.SimplePriorityQueue<Doorway, Doorway>
    {
		private int roomid;


		public DoorQueue(int id)
		{
			roomid = id;
		}


		/**
		 * True if the other id passed is the same as that of the room
		 * that owns these doors.
		 * 
		 * @param id 
		 * @return id == roomid
		 */
		public bool IsRoom(int id)
		{
			return id == roomid;
		}
	}
}