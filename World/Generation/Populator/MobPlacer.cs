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


        public static bool PlaceMobs(Room room, Level dungeon)
        {
            int maxMobs = room.GeometricArea / 5;
            bool boss = ((room.id == 2) 
                || (room.isNode && GameData.LevelDifficulty.ShouldHaveMobs(dungeon.random)));
            HashSet<Vector2Int> used = new HashSet<Vector2Int>();
            int level, num, rolledLevel;
            if (boss) level = GameData.LevelDifficulty.GetBossLevel(dungeon.random);
            else level = GameData.LevelDifficulty.GetMonsterLevel(dungeon.random);
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
                if (boss) level = GameData.LevelDifficulty.GetBossLevel(dungeon.random);
                else level = GameData.LevelDifficulty.GetMonsterLevel(dungeon.random);
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
            int x, z;
            int width = room.endX - room.beginX;
            int length = room.endZ - room.beginZ;
            MapMatrix map = dungeon.map;
            for(int tries = 0; tries < 10; tries++)
            {
                x = room.beginX + dungeon.random.NextInt(width);
                z = room.beginZ + dungeon.random.NextInt(length);
                Vector2Int location = new Vector2Int(x, z);
                if (map.GetGoodMobSpawn(x, z) && !used.Contains(location))
                {
                    dungeon.SpawnGameMob(entry.MobPrefab, x, z);
                    used.Add(location);
                    return true;
                }
            }
            return false;
        }
    }

}