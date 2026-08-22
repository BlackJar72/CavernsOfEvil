using UnityEngine;


namespace CevarnsOfEvil
{
    [System.Serializable]
    public struct GameDataPersistent
    {
        public string seedString;
        public ulong initialSeed;
        public ulong currentSeed;
        public int level;
        public DifficultySettings difficultySetting;
        public Size levelSize;
    }


    public static class GameData
    {   
        public const string saveSubdir = "saves";
        public const string saveFileExtension = ".es3";
        public const string saveFileName = "previous.es3";

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


        public static GameDataPersistent GetPersistentData()
        {
            GameDataPersistent result = new()
            {
                seedString = seedString,
                initialSeed = initialSeed,
                currentSeed = random.GetSeed(),
                level = level,
                difficultySetting = difficultySetting,
                levelSize = levelSize
            };
            return result;
        }


        public static void SetFromPersistentData(GameDataPersistent data)
        {
            seedString = data.seedString;
            initialSeed = data.initialSeed;
            random = new Xorshift(data.currentSeed);
            level = data.level;
            difficultySetting = data.difficultySetting;
            baseDifficulty = DifficultyTable.GetDifficultySetting(difficultySetting);
            levelDifficulty = baseDifficulty.FromLevel(Level);
            levelSize = data.levelSize;
        }


        public static ulong InitialSeed { get { return initialSeed; } }
        public static Xorshift Xrandom { get { return random; } }
        public static int Level { get { return level; } set { level = value; } }
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
            Level = 1;
            difficultySetting = difficulty;
            SeedString = seed;
            baseDifficulty = DifficultyTable.GetDifficultySetting(difficultySetting);
            levelDifficulty = baseDifficulty.FromLevel(Level);
            LoadingScreen.ResetHintShuffle();
            levelSize = Size.tiny;
            sizeData = SizeTable.GetData(levelSize);
            MusicManager.Init();
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
            Level++;
            levelDifficulty = baseDifficulty.FromLevel(Level);
            levelSize = BetterIncSize(levelSize, Level);
            sizeData = SizeTable.GetData(levelSize);
        }


        private static Size IncSize(Size s) => (Size)Mathf.Clamp((int)s + 1, 0, 5);


        private static Size BetterIncSize(Size s, int level) =>
            (Size)Mathf.Clamp((int)s + 1, 0, Mathf.Min(5, DifficultyCalculator.CalcDifficulty(level) * 6));



        public static void SaveGame()
        {
            GameDataPersistent gameData = GetPersistentData();
            PlayerData playerData = Player.PC.GetPlayerData();

            string fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;

            ES3.Save("GameData", gameData, fileName);
            ES3.Save("PlayerData", playerData, fileName);
        }



        public static void LoadGame()
        {
            GameDataPersistent gameData = GetPersistentData();
            PlayerData playerData = Player.PC.GetPlayerData();

            string fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;

            gameData = ES3.Load("GameData", fileName, gameData);
            playerData = ES3.Load("PlayerData", fileName, playerData);

            GameData.SetFromPersistentData(gameData);
            Player.PC.SetPlayerData(playerData);
        }


        public static void DeleteSavedGame()
        {
            string fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;
            ES3.DeleteFile(fileName);
            
        }


        public static bool DoesSaveExist()
        {
            string fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;
            return ES3.FileExists(fileName);
        }

    }

}