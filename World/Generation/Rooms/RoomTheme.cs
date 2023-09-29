using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public class RoomTheme
    {
        public Substance wallSubstance;
        public Substance floorSubstance;
        public Substance ceilingSubstance;
        public Substance liquidSubstance; //Are these treated the same way?
        public Substance pillarSubstance;
        public Substance caveSubstance;

		/// <summary>
		/// Creates a room theme based on the dungeon theme -- used for hub rooms.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="random"></param>
        public RoomTheme(DungeonTheme theme, Xorshift random)
        {
			wallSubstance = theme.WallSubstances[random.NextInt(theme.WallSubstances.Length)];
            floorSubstance = theme.FloorSubstances[random.NextInt(theme.FloorSubstances.Length)];
            ceilingSubstance = theme.CeilingSubstances[random.NextInt(theme.CeilingSubstances.Length)];
            liquidSubstance = theme.LiquidSubstances[random.NextInt(theme.LiquidSubstances.Length)];
			pillarSubstance = theme.PillarSubstances[random.NextInt(theme.PillarSubstances.Length)];
			caveSubstance = theme.CaveSubstances[random.NextInt(theme.CaveSubstances.Length)];
			if (theme.IsCaves) Cavify();
        }


        public RoomTheme(DungeonTheme theme)
        {
            wallSubstance = theme.WallSubstances[Random.Range(0, theme.WallSubstances.Length)];
            floorSubstance = theme.FloorSubstances[Random.Range(0, theme.FloorSubstances.Length)];
            ceilingSubstance = theme.CeilingSubstances[Random.Range(0, theme.CeilingSubstances.Length)];
            liquidSubstance = theme.LiquidSubstances[Random.Range(0, theme.LiquidSubstances.Length)];
			pillarSubstance = theme.PillarSubstances[Random.Range(0, theme.PillarSubstances.Length)];
			caveSubstance = theme.CaveSubstances[Random.Range(0, theme.CaveSubstances.Length)];
			if (theme.IsCaves) Cavify();
		}

		/// <summary>
		/// Creates a new room theme for a room based on the room in branched from as well 
		/// as the dungeon theme.  Generating room themes in this way allows for (variable 
		/// levels of) consistency as the rooms inherit theme data while being able to modify
		/// it to varying degrees.
		/// </summary>
		/// <param name="theme"></param>
		/// <param name="previousRoom"></param>
		/// <param name="dungeon"></param>
		/// <param name="random"></param>
        public RoomTheme(DungeonTheme theme, Room previousRoom, Level dungeon, Xorshift random)
		{
			RoomTheme previous = null;
			if(previousRoom != null)
            {
				previous = previousRoom.theme;
            }

			if ((previous != null) && !DungeonTheme.UseDegree(dungeon.style.variability, random))
			{
				wallSubstance = previous.wallSubstance;
				floorSubstance = previous.floorSubstance;
				ceilingSubstance = previous.ceilingSubstance;
				pillarSubstance = previous.pillarSubstance;
				liquidSubstance = previous.liquidSubstance;
				caveSubstance = previous.caveSubstance;
			}
			else if (DungeonTheme.UseDegree(dungeon.style.variability, random))
			{
				if (DungeonTheme.UseDegree(dungeon.style.variability, random))
				{
					wallSubstance = theme.WallSubstances[random.NextInt(theme.WallSubstances.Length)];
					floorSubstance = theme.FloorSubstances[random.NextInt(theme.FloorSubstances.Length)];
					ceilingSubstance = theme.CeilingSubstances[random.NextInt(theme.CeilingSubstances.Length)];
					pillarSubstance = theme.PillarSubstances[random.NextInt(theme.PillarSubstances.Length)];
					liquidSubstance = theme.LiquidSubstances[random.NextInt(theme.LiquidSubstances.Length)];
					caveSubstance = theme.CaveSubstances[random.NextInt(theme.CaveSubstances.Length)];
				}
				else
				{
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						wallSubstance = dungeon.defaultRoomTheme.wallSubstance;
					else wallSubstance = theme.WallSubstances[random.NextInt(theme.WallSubstances.Length)];
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						floorSubstance = dungeon.defaultRoomTheme.floorSubstance;
					else floorSubstance = theme.FloorSubstances[random.NextInt(theme.FloorSubstances.Length)];
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						ceilingSubstance = dungeon.defaultRoomTheme.ceilingSubstance;
					else ceilingSubstance = theme.CeilingSubstances[random.NextInt(theme.CeilingSubstances.Length)];
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						pillarSubstance = dungeon.defaultRoomTheme.pillarSubstance;
					else pillarSubstance = theme.PillarSubstances[random.NextInt(theme.PillarSubstances.Length)];
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						liquidSubstance = dungeon.defaultRoomTheme.liquidSubstance;
					else liquidSubstance = theme.LiquidSubstances[random.NextInt(theme.LiquidSubstances.Length)];
					if (!DungeonTheme.UseDegree(dungeon.style.variability, random))
						caveSubstance = dungeon.defaultRoomTheme.caveSubstance;
					else caveSubstance = theme.CaveSubstances[random.NextInt(theme.CaveSubstances.Length)];
				}
			}
			else
			{
				wallSubstance = dungeon.defaultRoomTheme.wallSubstance;
				floorSubstance = dungeon.defaultRoomTheme.floorSubstance;
				ceilingSubstance = dungeon.defaultRoomTheme.ceilingSubstance;
				pillarSubstance = dungeon.defaultRoomTheme.pillarSubstance;
				liquidSubstance = dungeon.defaultRoomTheme.liquidSubstance;
				caveSubstance = dungeon.defaultRoomTheme.caveSubstance;
			}
			if (theme.IsCaves) Cavify();
		}

		/// <summary>
		/// Modifies room theme to to be like a "natural"/cave area, with all solid 
		/// substances being the same (and based on the cave substance).
		/// </summary>
		/// <param name="theme"></param>
		/// <returns></returns>
		public RoomTheme Cavify()
		{
			wallSubstance = caveSubstance;
			floorSubstance = caveSubstance;
			ceilingSubstance = caveSubstance;
			pillarSubstance = caveSubstance;
			return this;
		}


	}
}