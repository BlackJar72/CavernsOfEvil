using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class RoomList : IList<Room>
    {
        private readonly List<Room> data = new List<Room>();
        private int numRooms = 0;


        public RoomList()
        {
            data.Insert(0, Room.roomNull);
        }


        public int Count => numRooms;

        public int TotalCount => data.Count;

        public bool IsReadOnly => false;

        public Room this[int index] { get => data[index]; set => data[index] = value; }

        public int IndexOf(Room item) => data.IndexOf(item);

        public void Insert(int index, Room item)
        {
            Debug.LogError("Insert(int Index) is not supported for RoomList!");
        }

        public void RemoveAt(int index)
        {
            Debug.LogError("RemoveAt(int Index) is not supported for RoomList!");
        }

        public void Add(Room item)
        {
            if (!data.Contains(item))
            {
                numRooms++;
                data.Add(item);
            }
        }

        public int AddRoomFast(Room item)
        {
                numRooms++;
                data.Add(item);
                return numRooms;
        }


        public int AddRoomSafe(Room item)
        {
            if (!data.Contains(item))
            {
                numRooms++;
                data.Add(item);
                return numRooms;
            }
            else return -1;
        }

        public void Clear()
        {
            numRooms = 0;
            data.Clear();
        }

        public bool Contains(Room item) => data.Contains(item);

        public void CopyTo(Room[] array, int arrayIndex) => data.CopyTo(array, arrayIndex + 1);

        public List<Room> CopyList()
        {
            List<Room> list = new List<Room>(numRooms);
            for (int i = 1; i < data.Count; i++)
            {
                if(data[i] != null) list.Add(data[i]);
            }
            list.TrimExcess();
            return list;
        }

        public bool Remove(Room item)
        {
            bool output = data.Remove(item);
            if(output) numRooms--;
            return output;
        }

        public IEnumerator<Room> GetEnumerator()
        {
            IEnumerator<Room> output = data.GetEnumerator();
            output.MoveNext();
            return output;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        public bool IsEmpty() => numRooms == 0;

    }

}
