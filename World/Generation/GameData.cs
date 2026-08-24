using System.Collections.Generic;
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
        public List<int> themeShuffle;
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
        private static List<int> themeShuffle;
        private static List<int> musicShuffle;
        private static List<int> hintShuffle;

        public static int GetThemeID(int l) => themeShuffle[l % themeShuffle.Count];

        public static bool resuming;


        static GameData()
        {
            themeShuffle = new() {0, 1, 2, 3, 4, 5, 6, 7};
        }


        public static void ShuffleThemes(Xorshift random)
        {
            themeShuffle.Shuffle(random);
        }


        public static void MoveThemeToFront(int theme)
        {
            if(themeShuffle.Count < 2) return;
            for(int i = 0; i < themeShuffle.Count; i++) {
                if(themeShuffle[i] == theme) {
                    themeShuffle[i] = themeShuffle[0];
                    themeShuffle[0] = theme;
                    return;
                }
            }
        }


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
                currentSeed = random.GetCurrentSeed(),
                level = level,
                difficultySetting = difficultySetting,
                levelSize = levelSize,
                themeShuffle = themeShuffle
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
            themeShuffle = data.themeShuffle;
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


        [QFSW.QC.Command("fake")]
        public static void FakeSave()
        {
            string fileName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) 
                            + System.IO.Path.DirectorySeparatorChar 
                            + "Tmp" + System.IO.Path.DirectorySeparatorChar + saveFileName;
            GameDataPersistent gameData = GetPersistentData();
            PlayerData playerData = Player.PC.GetPlayerData();
            ES3.Save("GameData", gameData, fileName);
            ES3.Save("PlayerData", playerData, fileName);
        }



        public static void LoadGame()
        {
            string fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;
            LoadGameData(fileName);
            LoadPlayerData(fileName);
        }


        public static void LoadGameData(string fileName = null)
        {
            if(fileName == null) fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;
            GameDataPersistent gameData = GetPersistentData();
            gameData = ES3.Load("GameData", fileName, gameData);
            GameData.SetFromPersistentData(gameData);

            baseDifficulty = DifficultyTable.GetDifficultySetting(difficultySetting);
            levelDifficulty = baseDifficulty.FromLevel(Level);
            LoadingScreen.ResetHintShuffle();
            sizeData = SizeTable.GetData(levelSize);
        }


        public static void LoadPlayerData(string fileName = null)
        {
            if(fileName == null) fileName = saveSubdir + System.IO.Path.DirectorySeparatorChar + saveFileName;
            PlayerData playerData = ES3.Load<PlayerData>("PlayerData", fileName);
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