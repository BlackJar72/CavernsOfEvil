using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using LeastSquares;

namespace CevarnsOfEvil

{

    public static class ScoreData
    {
        public static float startTime;
        public static float endTime;
        public static int totalKills;
        public static int totalMobs;


        public static void Reset()
        {
            totalKills = 0;
            totalMobs = 1;
            startTime = 0;
            endTime = 0; 
        }

        public static void NewLevel(int mobs)
        {
            totalKills = 0;
            totalMobs = mobs;
            startTime = Time.time;
            endTime = float.PositiveInfinity;
        }

        public static string GetTimeString()
        {
            int minutes = 0;
            int seconds = (int)(endTime - startTime);

            if(seconds > 60)
            {
                minutes = seconds / 60;
                seconds = seconds % 60;
            }
            return LocalizationManager.GetTranslation("UIStrings", "TimeN", minutes.ToString(), seconds.ToString());
        }

        public static string GetKillsString()
        {
            string[] append = new string[3];
            append[0] = totalKills.ToString();
            append[1] = totalMobs.ToString();
            append[2] = ((int)(((float)totalKills / (float)totalMobs) * 100)).ToString();
            return LocalizationManager.GetTranslation("UIStrings", "KillsN", append);
        }
    }



    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text timeText;
        [SerializeField] TMP_Text killsText;
        [SerializeField] TMP_Text hintText;

        [SerializeField] GameObject scores;
        [SerializeField] GameObject buttons;

        [SerializeField] bool isNormal;
        [SerializeField] GameObject quitButton;

        [SerializeField] string[] hints;
        private static List<string> shuffledHints = new List<string>();
        private static bool hintsShuffled = false;

        [SerializeField] SteamAchievementsAndStats steam;


        private void Start()
        {
            if (GameData.Level > 0) {
                if (isNormal) {
                    levelText.text = LocalizationManager.GetTranslation("UIStrings", "LevelN", GameData.Level.ToString());
                    timeText.text = ScoreData.GetTimeString();
                    killsText.text = ScoreData.GetKillsString();
                    ShowHint();
                    GameData.NextLevel();
                    StartCoroutine(ShowPieces());
                    if (steam != null) {
                        steam.AddStat("HIGH_LEVEL", GameData.Level);
                    }
                }
            }
        }


        /*void OnEnable() {
            //Level.LevelBuiltEvent += OnSceneLoaded;
        }


        void OnDisable() {
            //Level.LevelBuiltEvent -= OnSceneLoaded;
        }*/

        
        IEnumerator ShowPieces()
        {
            yield return new WaitForSeconds(1);
            scores.SetActive(true);
            yield return new WaitForSeconds(1);
            Cursor.lockState = CursorLockMode.None;
            // 17 because the level has now incremented, though to end at 16 we need 17
            if((GameData.Level == 17) && isNormal) quitButton.SetActive(false);
            buttons.SetActive(true);
        }


        public void StartNextLevel()
        {
            // 17 because the level has now incremented, though to end at 16 we need 17
            if((GameData.Level == 17) && isNormal) {
                SceneManager.LoadScene("VictoryScene");
            } else {
                SceneManager.LoadScene("DungeonScene");
            }
        }


        public void SaveAndExit()
        {
            //SaveGame.SaveGameData(SaveGame.STAND_IN);
            SceneManager.LoadScene("Start");
        }


        public void ShuffleHints() {
            shuffledHints.Clear();
            for(int i = 0; i < hints.Length; i++) {
                shuffledHints.Add(hints[i]);
            }
            shuffledHints.Shuffle<string>();
            hintsShuffled = true;
        }


        private void ShowHint() {
            if(!hintsShuffled || (shuffledHints.Count < 1)) ShuffleHints();
            int which = GameData.Level - 1;
            if(which < shuffledHints.Count) {
                hintText.text = LocalizationManager.GetTranslation("Hints", shuffledHints[which]);
            } else {
                hintText.text = LocalizationManager.GetTranslation("Hints",
                        hints[Random.Range(0, hints.Length)]);
            }
        }


        public static void ResetHintShuffle() {
            hintsShuffled = false;
        }


        /*public void OnSceneLoaded() {
            //Debug.Log("SceneLoaded");
        }*/

    }


}
