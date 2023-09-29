using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Nito.Collections;


namespace CevarnsOfEvil
{
    /// <summary>
    /// A
    /// </summary>
    public class RoomAreas
    {
        public List<Vector2Int> reachableTiles;
        public List<Vector2Int> spawnableTiles;
        public List<Vector2Int> spawnableNHigh;
        public List<List<Vector2Int>> highs = new List<List<Vector2Int>>();
        public List<List<Vector2Int>> lows = new List<List<Vector2Int>>();

        private List<Vector2Int> allTiles = new List<Vector2Int>();
        private Level level;
        private MapMatrix map;
        private Room room;

        private int roomArea;

        // I AM DUMBNESSMAN!!!
        public RoomAreas(Room room, Level level)
        {
            Debug.Log("Starting for room " + room.id);
            this.level = level;
            this.map = level.map;
            this.room = room;
            roomArea = (room.endX - room.beginX) * (room.endZ - room.beginZ);
            map.ResetUsed(room);
            for (int i = room.beginX; i < room.endX; i++) 
                for(int j = room.beginZ; j < room.endZ; j++)
                {
                    // First, find all available tiles
                    if ((map.GetRoom(i, j) == room.id) && map.GetPassableAndSafe(i, j)) {
                        allTiles.Add(new Vector2Int(i, j));
                    }
                }
            if((room.doors != null) && (room.doors.Count > 0))
                reachableTiles = FindConnected(room.doors[0].GetTile().ToVector());
            /*
            spawnableTiles = new List<Vector2Int>();
            foreach(Vector2Int tile in reachableTiles)
            {
                if (!map.GetAStared(tile.x, tile.y)) spawnableTiles.Add(tile);
            }
            while(allTiles.Count > 0)
            {
                if(map.GetFloorY(allTiles[0].x, allTiles[0].y) > room.floorY)
                {
                    highs.Add(FindConnected(allTiles[0]));
                }
                else
                {
                    lows.Add(FindConnected(allTiles[0]));
                }
            }
            spawnableNHigh = new List<Vector2Int>(spawnableTiles);
            foreach(List<Vector2Int> high in highs)
            {
                foreach(Vector2Int tile in high) spawnableNHigh.Add(tile);
            }*/
            map.ResetUsed(room);
            Debug.Log("Finished for room " + room.id);
        }


        private List<Vector2Int> FindConnected(Vector2Int start)
        {
            List<Vector2Int> output = new List<Vector2Int>();
            Deque<Vector2Int> queue = new Deque<Vector2Int>();
            Vector2Int nextTile;
            queue.AddToBack(start);
            while(queue.Count > 0)
            {
                nextTile = queue.RemoveFromFront();
                ConsiderNeighbors(nextTile, queue);
            }
            Debug.Log("Found " + output.Count + " connected tiles");
            return output;
        }


        private void ConsiderNeighbors(Vector2Int tile, Deque<Vector2Int> queue)
        {
            ConsiderANeighbor(tile, new Vector2Int(tile.x - 1, tile.y - 1), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x - 1, tile.y), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x - 1, tile.y + 1), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x, tile.y - 1), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x, tile.y + 1), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x + 1, tile.y - 1), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x + 1, tile.y), queue);
            ConsiderANeighbor(tile, new Vector2Int(tile.x + 1, tile.y + 1), queue);
        }


        private void ConsiderANeighbor(Vector2Int src, Vector2Int neighbor, Deque<Vector2Int> queue)
        {
            if((OnMap(neighbor) && map.GetRoom(neighbor.x, neighbor.y) == room.id) 
                && !map.GetUsed(neighbor.x, neighbor.y)
                && map.GetPassableAndSafe(neighbor.x, neighbor.y))
            {
                int diff = map.GetFloorY(src.x, src.y) - map.GetFloorY(neighbor.x, neighbor.y);
                if((diff * diff) < 2)
                {
                    queue.AddToBack(neighbor);
                    allTiles.Remove(neighbor);
                    map.SetUsed(neighbor.x, neighbor.y);
                }
            }
        }


        private bool OnMap(Vector2Int tile)
        {
            return (tile.x > -1) && (tile.y > -1) 
                && (tile.x < level.size.width) && (tile.y < level.size.width);
        }


        #region Use Data
        public int GetSpawnableSpaces()
        {
            return spawnableTiles.Count;
        }


        public Vector2Int? GetASpawnable(Xorshift random)
        {
            if(spawnableTiles.Count > 0)
            return spawnableTiles[random.NextInt(spawnableTiles.Count)];
            else return null;
        }


        public Vector2Int? GetASpawnableWRemoval(Xorshift random)
        {
            if (spawnableTiles.Count > 0)
            {
                Vector2Int output = spawnableTiles[random.NextInt(spawnableTiles.Count)];
                spawnableTiles.Remove(output);
                return output;
            }
            else return null;
        }


        public Vector2Int? GetASpawnableOrHigh(Xorshift random)
        {
            if (spawnableNHigh.Count > 0)
                return spawnableNHigh[random.NextInt(spawnableTiles.Count)];
            else return null;
        }


        public Vector2Int? GetASpawnableOrHighRemoval(Xorshift random)
        {
            if (spawnableNHigh.Count > 0)
            {
                Vector2Int output = spawnableNHigh[random.NextInt(spawnableTiles.Count)];
                spawnableNHigh.Remove(output);
                return output;
            }
            else return null;
        }
        #endregion
    }

}