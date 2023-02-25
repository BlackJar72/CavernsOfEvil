using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public static class PickupPlacer
    {
        public static void Process(Level dungeon, int ammoAmount, int healingAmount)
        {
            List<Room> rooms = dungeon.rooms.CopyList();
            rooms.Shuffle(dungeon.random);
            int numBigItems = Mathf.Max(1, dungeon.nodes.Length);
            PlaceBigItems(dungeon, numBigItems, rooms);
            PlaceAmmo(dungeon, ammoAmount, rooms);
            PlaceHealing(dungeon, healingAmount, rooms);
            if(GameData.Level == 1) LevelOneEnhance(dungeon, rooms);
        }


        public static void PlaceAmmo(Level dungeon, int amount, List<Room> rooms)
        {
            rooms.Shuffle(dungeon.random);
            ItemEntry entry;
            PickupList ammoList = dungeon.pickupLists.Ammo;
            int roomnum = 0;
            List<ItemEntry> items = new List<ItemEntry>();
            for(int i = 0; i < ammoList.Items.Length; i++)
            {
                if(ammoList.Items[i].Level < GameData.LevelDifficulty.levelDifficulty)
                {
                    items.Add(ammoList.Items[i]);
                }
            }
            while ((amount > 0) && (roomnum < rooms.Count))
            {
                if (items.Count < 1) entry = ammoList.Items[0];
                else if (items.Count > 1)
                {
                    entry = items[dungeon.random.NextInt(items.Count)];
                }
                else entry = items[0];
                Debug.Assert(entry != null);
                if (entry == null) return;
                amount -= PlaceSmallItem(dungeon, rooms[roomnum], entry, 1) * entry.AmmoValue;
                roomnum++;
            }
        }


        public static void PlaceHealing(Level dungeon, int amount, List<Room> rooms)
        {
            rooms.Shuffle(dungeon.random);
            ItemEntry entry;
            PickupList ammoList = dungeon.pickupLists.Healing;
            int roomnum = 0;
            while ((amount > 0) && (roomnum < rooms.Count))
            {
                entry = ammoList.Items[0];
                Debug.Assert(entry != null);
                if (entry == null) return;
                amount -= PlaceSmallItem(dungeon, rooms[roomnum], entry, 1) * entry.HealthValue;
                roomnum++;
            }
        }


        private static int PlaceSmallItem(Level dungeon, Room room, ItemEntry entry, int number)
        {
            int output = 0;
            int x, z, width = room.endX - room.beginX, length = room.endZ - room.beginZ;
            for (int i = 0; i < number; i++)
            {
                for(int tries = 0; tries < 10; tries++)
                {
                    x = dungeon.random.NextInt(width) + room.beginX;
                    z = dungeon.random.NextInt(length) + room.beginZ;
                    if (dungeon.map.GetGoodMobSpawn(x, z))
                    {
                        dungeon.SpawnPickup(entry, x, z);
                        output++;
                        break;
                    }
                }
            }
            return output;
        }


        private static void PlaceBigItems(Level dungeon, int number, List<Room> rooms)
        {            
            int hubIndex;
            for(int i = 0; i < number; i++)
            {
                hubIndex = i + 3;
                if((hubIndex < dungeon.nodes.Length) 
                    && (dungeon.rooms[hubIndex] != null) 
                    && (dungeon.random.NextFloat() < 0.8))
                {
                    PlaceABigItem(dungeon, dungeon.rooms[hubIndex], true, i % 3);
                }
                else
                {
                    PlaceABigItem(dungeon, rooms[i], false, i % 3);
                }
            }
        }


        private static void PlaceABigItem(Level dungeon, Room room, bool centralize, int selector)
        {
            ItemEntry entry;
            switch (selector) {
                case 0:
                        entry = SelectAWeapon(dungeon, room);
                    break;
                case 1:
                        entry = SelectAnArmor(dungeon, room);
                    break;
                default:
                        if (dungeon.random.NextBool()) entry = SelectAWeapon(dungeon, room);
                        else entry = SelectAnArmor(dungeon, room);
                    break;
            }
            if (centralize && dungeon.map.GetPassableAndSafe(room.midX, room.midZ))
            {
                dungeon.SpawnPickup(entry, room.midX, room.midZ);
                return;
            }
            else
            {
                int x, z, width = room.endX - room.beginX, length = room.endZ - room.beginZ;
                for (int tries = 0; tries < 10; tries++)
                {
                    x = dungeon.random.NextInt(width) + room.beginX;
                    z = dungeon.random.NextInt(length) + room.beginZ;
                    if(dungeon.map.GetGoodMobSpawn(x, z) && (dungeon.map.GetFloorY(x, z) == 0))
                    {
                        dungeon.SpawnPickup(entry, x, z);
                        return;
                    }
                }
                for (int tries = 0; tries < 10; tries++)
                {
                    x = dungeon.random.NextInt(width) + room.beginX;
                    z = dungeon.random.NextInt(length) + room.beginZ;
                    if (dungeon.map.GetPassableAndSafe(x, z))
                    {
                        dungeon.SpawnPickup(entry, x, z);
                        return;
                    }
                }
            }
        }


        public static ItemEntry SelectAnArmor(Level dungeon, Room rooms)
        {
            PickupList armorList = dungeon.pickupLists.Armors;
            for (int i = 0; i < armorList.Items.Length; i++)
            {
                if (armorList.Items[i].Level > GameData.LevelDifficulty.levelDifficulty)
                {
                    return armorList.Items[i - 1];
                }
            }
            return armorList.Items[armorList.Items.Length - 1];
        }


        public static ItemEntry SelectAWeapon(Level dungeon, Room rooms)
        {
            PickupList weaponList = dungeon.pickupLists.Weapons;
            if (GameData.Level == 3) return weaponList.Items[2];
            List<ItemEntry> items = new List<ItemEntry>();
            for (int i = 0; i < weaponList.Items.Length; i++)
            {
                if (weaponList.Items[i].Level < GameData.LevelDifficulty.levelDifficulty)
                {
                    items.Add(weaponList.Items[i]);
                }
            }
            if (items.Count < 2) return weaponList.Items[0];
            else return items[dungeon.random.NextInt(items.Count)];
        }


        private static void LevelOneEnhance(Level dungeon, List<Room> rooms)
        {
            PlaceSmallItem(dungeon, dungeon.rooms[1], dungeon.pickupLists.Ammo.Items[0], 1);
            PlaceSmallItem(dungeon, dungeon.rooms[1], dungeon.pickupLists.Healing.Items[0], 1);
            PlaceAmmo(dungeon, (int)((GameData.BaseDifficulty.areaPerEncounter / 10) - 10), rooms);
            PlaceHealing(dungeon, (int)((GameData.BaseDifficulty.areaPerEncounter / 10) - 10), rooms);

        }


        private static double FindChance(ItemEntry entry, float difficulty, double last, out PickupChance chance)
        {
            chance.entry = entry;
            float odds = entry.Level / difficulty;
            odds = 0.25f - (odds * odds) + (odds);
            odds *= entry.Rarity;
            Debug.Assert(odds > 0f);
            chance.upperLimit = last + odds;
            return chance.upperLimit;
        }


        public static void Init()
        {/* Do Something...?*/}

    }


    public struct PickupChance
    {
        public ItemEntry entry;
        public double upperLimit;
    }

}