using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

	public struct DifficultySetting
	{
		public DifficultySetting(int mobChance, int mobLevel, int maxLevel, int bossLevel,
				int bossMax, int areaPerEncounter, bool startMobs)
		{
			this.mobChance = mobChance;
			this.numFactor = 1f;
			this.mobLevel  = mobLevel;
			this.maxLevel  = maxLevel;
			this.bossLevel = bossLevel;
			this.bossMax   = bossMax;
			this.areaPerEncounter = areaPerEncounter;
			this.startMobs = startMobs;
			this.levelDifficulty = 1;
			this.upgradeL1chance = 0.25f;
		}

		DifficultySetting(int mobChance, float numFactor, float mobLevel, int maxLevel, 
				float bossLevel, int bossMax, float areaPerEncounter, bool startMobs,
				float levelDifficulty, float ugchance)
		{
			this.mobChance = mobChance;
			this.numFactor = numFactor * 2;
			this.mobLevel  = Mathf.Min(mobLevel, 6);
			this.maxLevel  = Mathf.Min(Mathf.RoundToInt(Mathf.Min((mobLevel * 2.5f), maxLevel)));
			this.bossLevel = Mathf.Min(bossLevel, 10);
			this.bossMax   = Mathf.Min(bossMax, 10);
			this.areaPerEncounter = areaPerEncounter;
			this.startMobs = startMobs;
			this.levelDifficulty = levelDifficulty;
			this.upgradeL1chance = ugchance;
		}

		public DifficultySetting FromLevel(int level)
		{
			float difficulty = DifficultyCalculator.CalcDifficulty(level);
			float ugchance = 0;
			if (level > 0) ugchance = difficulty * 0.75f;
			return new DifficultySetting(Mathf.CeilToInt(Mathf.Sqrt(difficulty) * mobChance),
						difficulty, difficulty * mobLevel, maxLevel, 
						difficulty * bossLevel, Mathf.CeilToInt(difficulty * bossMax),
						areaPerEncounter / Mathf.Sqrt(difficulty + 0.5f), startMobs, 
						difficulty, ugchance);
		}

		public DifficultySetting FromDifficulty(float difficulty)
		{
			float ugchance = 0;
			if (DifficultyCalculator.CalcLevel(difficulty) > 0) ugchance = difficulty * 0.75f;
			return new DifficultySetting(Mathf.CeilToInt(Mathf.Sqrt(difficulty) * mobChance),
						difficulty, difficulty * mobLevel, maxLevel,
						difficulty * bossLevel, Mathf.CeilToInt(difficulty * bossMax),
						areaPerEncounter / Mathf.Sqrt(difficulty + 0.5f), startMobs, 
						difficulty, ugchance);
		}

		public int GetMonsterLevel(Xorshift random)
        {
			int output = Mathf.RoundToInt(Mathf.Clamp(mobLevel
				+ (random.NextGaussian() * mobLevel), 0, maxLevel));
			if(output == 0) output = random.NextInt(Mathf.CeilToInt(mobLevel)) + 1;
			if((output == 1) && (random.NextFloat() > upgradeL1chance)) output = 1;
			return output;
		}

		public int GetNumberAppearing(Xorshift random, int level, MobEntry mob)
		{
			float mean = Mathf.Sqrt(11 - mob.MobLevel) * (mobLevel / mob.MobLevel) * Mathf.Sqrt(level / mob.MobLevel);
			float zscore = mean * numFactor;
			return Mathf.RoundToInt(Mathf.Clamp(mean
				+ (random.NextGaussian() * zscore), 1, mean * 2));
		}

		public int GetBossLevel(Xorshift random)
		{
			float mean = (bossLevel + bossMax) / 2;
			float zscore = (bossMax - bossLevel) / 3;
			int output = Mathf.Max(Mathf.RoundToInt(Mathf.Clamp(mean + (random.NextGaussian() * zscore),
				bossLevel, bossMax)), 2);
			if ((output < (mean - (zscore * 2)) && (random.NextFloat() > upgradeL1chance))) 
				output = Mathf.CeilToInt(mean);
			return output;
		}

		public int GetNumberBosses(Xorshift random, int level, MobEntry mob)
		{
			float mean = Mathf.Sqrt(11 - mob.MobLevel) * ((bossLevel + bossMax) / (mob.MobLevel * 2.0f))
				 * Mathf.Sqrt(level / mob.MobLevel);
			float zscore = mean * numFactor;
			return Mathf.RoundToInt(Mathf.Clamp(mean
				+ (random.NextGaussian() * zscore), 1, mean * 2));
		}

		public bool ShouldHaveMobs(Xorshift random)
        {
			return (random.NextInt(10) < mobChance);

		}

		public int NumRoomsWithMobs(int numrooms)
		{
			return (numrooms * mobChance) / 10;

		}

		public readonly int   mobChance;
		public readonly float numFactor;
		public readonly float mobLevel;
		public readonly float bossLevel;
		public readonly int   maxLevel;
		public readonly int   bossMax;
		public readonly float areaPerEncounter;
		public readonly bool  startMobs;
		public readonly float levelDifficulty;
		public readonly float upgradeL1chance;
	}


	[System.Serializable]
	public enum DifficultySettings
	{
		none = 0,
		baby = 1,
		noob = 2,
		norm = 3,
		hard = 4,
		nuts = 5
	}


	public static class DifficultyTable
	{
		public static readonly DifficultySetting None = new DifficultySetting(0, 0, 0, 0, 0, -1, false);
		public static readonly DifficultySetting Baby = new DifficultySetting(3, 2, 4, 4, 6, 884, false);
		public static readonly DifficultySetting Noob = new DifficultySetting(4, 3, 6, 4, 8, 590, false);
		public static readonly DifficultySetting Norm = new DifficultySetting(5, 4, 8, 6, 10, 427, false);
		public static readonly DifficultySetting Hard = new DifficultySetting(6, 5, 8, 8, 10, 323, true);
		public static readonly DifficultySetting Nuts = new DifficultySetting(7, 6, 10, 8, 10, 100, true);
		public static readonly DifficultySetting[] settings = new DifficultySetting[]{None, Baby, Noob, Norm, Hard, Nuts};
		public static DifficultySetting GetDifficultySetting(DifficultySettings value) => 
			settings[(int)value];
		public static DifficultySetting GetDifficultySetting(int value) =>
			settings[value];
	}

}
