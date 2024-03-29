using System.Collections.Generic;
using UnityEngine;

namespace CevarnsOfEvil
{
    public static class MobPlacer
    {
        public static bool Process(Room room, Level level)
        {
            if (GameData.LevelDifficulty.ShouldHaveMobs(level.random) || room.isNode)
            {
                if(level.random.NextBool())
                    return PlaceMobs(room, level);
                else
                    return PlaceMultiMobs(room, level);
            }
            return false;
        }


        public static void ProcessConsistent(RoomList rooms, Level level)
        {
            for(int i = 2; i <= level.nodes.Length; i++)
            {
                if(rooms[i] != null)
                {
                    if (level.random.NextBool())
                        PlaceMobs(rooms[i], level);
                    else
                        PlaceMultiMobs(rooms[i], level);
                }
            }
            List<Room> normal = new List<Room>();
            for(int i = level.nodes.Length + 1; i < rooms.Count; i++)
            {
                normal.Add(rooms[i]);
            }
            normal.Shuffle(level.random);
            int withMobs;
            int withMobs1 = GameData.LevelDifficulty.NumRoomsWithMobs(normal.Count);
            int withMobs2 = GameData.LevelDifficulty.NumRoomsWithMobs(rooms.Count);
            if((withMobs2 - withMobs1) > 1) withMobs = withMobs1 + level.random.NextInt(withMobs1 - withMobs2);
            else withMobs = withMobs1;
            for(int i = 0; (i < withMobs) && (i < normal.Count); i++)
            {
                if (normal[i] != null)
                {
                    if (level.random.NextBool())
                        PlaceMobs(normal[i], level);
                    else
                        PlaceMultiMobs(normal[i], level);
                }
            }
        }


        public static bool PlaceMobs(Room room, Level dungeon)
        {
            int maxMobs = room.GeometricArea / 5;
            bool boss = ((room.id == 2) 
                || (room.isNode && GameData.LevelDifficulty.ShouldHaveMobs(dungeon.random)));
            HashSet<Vector2Int> used = new HashSet<Vector2Int>();
            int level, num, rolledLevel;
            {
                if (boss) level = GameData.LevelDifficulty.GetBossLevel(dungeon.random);
                else level = GameData.LevelDifficulty.GetMonsterLevel(dungeon.random);
            }
            rolledLevel = level;
            MobList list = null;
            while (((list == null) || (list.Empty)) && (level > -1))
            {
                level--;
                list = dungeon.theme.MobLists.Lists[level];
            }
            if (((list == null) || (list.Empty))) return false;
            MobEntry entry = list.GetEntry(dungeon.random);
            bool success = false;
            if (boss) num = GameData.LevelDifficulty.GetNumberBosses(dungeon.random, rolledLevel, entry);
            else num = GameData.LevelDifficulty.GetNumberAppearing(dungeon.random, rolledLevel, entry);
            num = Mathf.Min(num, maxMobs);
            for (int i = 0; i < num; i++)
            {
                success |= PlaceAMob(room, dungeon, entry, used);
            }
            return success;
        }


        public static bool PlaceMultiMobs(Room room, Level dungeon)
        {
            int maxMobs = room.GeometricArea / 5;
            float ratio = (dungeon.random.NextFloat() * 0.6f) + 0.2f;
            bool boss = ((room.id == 2)
                || (room.isNode && GameData.LevelDifficulty.ShouldHaveMobs(dungeon.random)));
            HashSet<Vector2Int> used = new HashSet<Vector2Int>();
            bool success = false;
            // Some of one kind
            {
                int level, num, rolledLevel;
                {
                    if (boss) level = GameData.LevelDifficulty.GetBossLevel(dungeon.random);
                    else level = GameData.LevelDifficulty.GetMonsterLevel(dungeon.random);
                }
                rolledLevel = level;
                MobList list = null;
                while (((list == null) || (list.Empty)) && (level > -1))
                {
                    level--;
                    list = dungeon.theme.MobLists.Lists[level];
                }
                if (((list == null) || (list.Empty))) return false;
                MobEntry entry = list.GetEntry(dungeon.random);
                if (boss) num = GameData.LevelDifficulty.GetNumberBosses(dungeon.random, rolledLevel, entry);
                else num = GameData.LevelDifficulty.GetNumberAppearing(dungeon.random, rolledLevel, entry);
                num = Mathf.Max(1, Mathf.CeilToInt(ratio * Mathf.Min(num, maxMobs)));
                for (int i = 0; i < num; i++)
                {
                    success |= PlaceAMob(room, dungeon, entry, used);
                }
            }
            // Some of another (maybe) (the second will never be a "boss.")
            {
                int level, num, rolledLevel;
                level = GameData.LevelDifficulty.GetMonsterLevel(dungeon.random);
                rolledLevel = level;
                MobList list = null;
                while (((list == null) || (list.Empty)) && (level > -1))
                {
                    level--;
                    list = dungeon.theme.MobLists.Lists[level];
                }
                if (((list == null) || (list.Empty))) return false;
                MobEntry entry = list.GetEntry(dungeon.random);
                num = GameData.LevelDifficulty.GetNumberAppearing(dungeon.random, rolledLevel, entry);
                num = Mathf.FloorToInt((1.0f - ratio) * Mathf.Min(num, maxMobs));
                for (int i = 0; i < num; i++)
                {
                    success |= PlaceAMob(room, dungeon, entry, used);
                }
            }
            return success;
        }


        public static bool PlaceAMob(Room room, Level dungeon, MobEntry entry, HashSet<Vector2Int> used)
        {
            if(entry.Size < MobSize.large) {
                return PlaceSmallAMob(room, dungeon, entry, used);
            } else {
                return PlaceALargeMob(room, dungeon, entry, used);
            }
        }


        public static void PlaceFinalBoss(Room room, Level dungeon, HashSet<Vector2Int> used) {
            MobEntry entry = dungeon.theme.MobLists.Lists[7].Mobs[0];
            dungeon.SpawnGameMob(entry.MobPrefab, room.midX, room.midZ - 1, false, true);
            Vector2Int subloc = new Vector2Int();
            for(int i = 0; i < 4; i++) {
                subloc.x = room.midX + (i % 2);
                subloc.y = room.midZ + (i / 2) - 1;
                used.Add(subloc);
            }
        }


        public static bool PlaceSmallAMob(Room room, Level dungeon, MobEntry entry, HashSet<Vector2Int> used)
        {
            int x, z;
            int width = room.endX - room.beginX;
            int length = room.endZ - room.beginZ;
            MapMatrix map = dungeon.map;
            for(int tries = 0; tries < 10; tries++)
            {
                x = room.beginX + dungeon.random.NextInt(width);
                z = room.beginZ + dungeon.random.NextInt(length);
                Vector2Int location = new Vector2Int(x, z);
                if (map.GetGoodSmallMobSpawn(x, z) && !used.Contains(location))
                {
                    dungeon.SpawnGameMob(entry.MobPrefab, x, z);
                    used.Add(location);
                    return true;
                }
            }
            return false;
        }


        public static bool PlaceALargeMob(Room room, Level dungeon, MobEntry entry, HashSet<Vector2Int> used)
        {
            int x, z;
            int width = room.endX - room.beginX - 1;
            int length = room.endZ - room.beginZ - 1;
            bool good;
            MapMatrix map = dungeon.map;
            for(int tries = 0; tries < 10; good = false, tries++)
            {
                x = room.beginX + dungeon.random.NextInt(width);
                z = room.beginZ + dungeon.random.NextInt(length);
                Vector2Int location = new Vector2Int(x, z);
                good = map.GetGoodLargeMobSpawn(x, z) && map.GetGoodLargeMobSpawn(x + 1, z)
                    && map.GetGoodLargeMobSpawn(x, z + 1) & map.GetGoodLargeMobSpawn(x + 1, z + 1);
                Vector2Int subloc = location;
                for(int i = 0; i < 4; i++) {
                    subloc.x = location.x + (i % 2);
                    subloc.x = location.x + (i / 2);
                    good = good && !used.Contains(subloc);
                }
                if (good)
                {
                    dungeon.SpawnGameMob(entry.MobPrefab, x, z, true, true);
                    for(int i = 0; i < 4; i++) {
                        subloc.x = location.x + (i % 2);
                        subloc.y = location.y + (i / 2);
                        used.Add(subloc);
                    }
                    return true;
                }
            }
            return false;
        }
    }

}