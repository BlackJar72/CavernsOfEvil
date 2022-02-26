using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public static class GameData
    {
        private static string seedString = "";
        private static ulong initialSeed;
        private static Xorshift random;
        private static int level;
        private static DifficultySettings difficultySetting = DifficultySettings.norm;
        private static DifficultySetting baseDifficulty;
        private static DifficultySetting levelDifficulty;
        private static Size levelSize;
        private static SizeData sizeData;


        public static string SeedString
        {
            get { return seedString; }
            set
            {
                seedString = value;
                if (seedString.Equals(""))
                {
                    random = new Xorshift();
                    initialSeed = random.GetSeed();
                    seedString = initialSeed.ToString();
                }
                else
                {
                    if (!ulong.TryParse(seedString, out initialSeed))
                    {
                        initialSeed = (ulong)seedString.GetHashCode();
                    }
                    random = new Xorshift(initialSeed);
                }

            }
        }


        public static ulong InitialSeed { get { return initialSeed; } }
        public static Xorshift Xrandom { get { return random; } }
        public static int Level { get { return level; } }
        public static DifficultySettings GameDifficulty { get { return difficultySetting; } }
        public static DifficultySetting BaseDifficulty { get { return baseDifficulty; } }
        public static DifficultySetting LevelDifficulty { get { return levelDifficulty; } }
        public static Size LevelSize { get { return levelSize; } }
        public static SizeData LevelSizeData { get { return sizeData; } }


        /// <summary>
        /// Called at the start of a new game to (re)set all data for game start.
        /// </summary>
        public static void Init(string seed, DifficultySettings difficulty)
        {
            level = 1;
            difficultySetting = difficulty;
            SeedString = seed;
            baseDifficulty = DifficultyTable.GetDifficultySetting(difficultySetting);
            levelDifficulty = baseDifficulty.FromLevel(level);
            // TODO: Set this with episode data
            levelSize = Size.tiny;
            sizeData = SizeTable.GetData(levelSize);
            PickupPlacer.Init();
            Sword.SwordInit();
            WandOfFire.WandInit();
            WandOfLightning.WandInit();
            StaffOfFallingStars.WandInit();
            Player.Init();
        }


        /// <summary>
        /// Called start of new levels (after beginning with level 2) to increment data.
        /// </summary>
        public static void NextLevel()
        {
            level++;
            levelDifficulty = baseDifficulty.FromLevel(level);
            levelSize = BetterIncSize(levelSize, level);
            sizeData = SizeTable.GetData(levelSize);
        }


        private static Size IncSize(Size s) => (Size)Mathf.Clamp((int)s + 1, 0, 5);


        private static Size BetterIncSize(Size s, int level) =>
            (Size)Mathf.Clamp((int)s + 1, 0, Mathf.Min(5, DifficultyCalculator.CalcDifficulty(level) * 6));

    }

}