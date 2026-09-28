using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
        public const string LB_MAX_LEVEL = "Max Level Reached";
        public const string STEAM_URL = "https://store.steampowered.com/app/1929380/Caverns_of_Evil/";
        public const int MAX_DEMO_LEVEL = 5;

        [SerializeField] TMP_Text levelText;
        [SerializeField] TMP_Text timeText;
        [SerializeField] TMP_Text killsText;
        [SerializeField] TMP_Text hintText;
        [SerializeField] GameObject hintTextObj;

        [SerializeField] GameObject scores;
        [SerializeField] GameObject buttons;
        [SerializeField] GameObject endButtons;
        [SerializeField] GameObject buyText;

        [SerializeField] bool isNormal;
        [SerializeField] GameObject quitButton;

        [SerializeField] string[] hints;
        private static List<string> shuffledHints = new List<string>();
        private static bool hintsShuffled = false;



        public void Init()
        {
            if (GameData.Level > 0) {
                if (isNormal) {
                    if(GameData.Level >= MAX_DEMO_LEVEL) levelText.text = "Demo Completed!";
                    else levelText.text = LocalizationManager.GetTranslation("UIStrings", "LevelN", GameData.Level.ToString());
                    timeText.text = ScoreData.GetTimeString();
                    killsText.text = ScoreData.GetKillsString();
                    ShowHint();
                    GameData.NextLevel();
                    GameData.SaveGame();
                    buttons.SetActive(false);
                    scores.SetActive(false);
                    quitButton.SetActive(!((GameData.Level == 17) && isNormal));
                    StartCoroutine(ShowPieces());
                }
            }
        }

        
        IEnumerator ShowPieces()
        {
            yield return new WaitForSecondsRealtime(1);
            scores.SetActive(true);
            yield return new WaitForSecondsRealtime(1);
            Cursor.lockState = CursorLockMode.None;
            if(GameData.Level <= MAX_DEMO_LEVEL) buttons.SetActive(true);
            else
            {
                buttons.SetActive(false);
                hintTextObj.SetActive(false);
                endButtons.SetActive(true);
                buyText.SetActive(true);
                GameData.Level = 1;
            }
        }


        public void StartNextLevel()
        {
            GameManager.Instance.NextLevel();
        }


        public void ShowSteamPage()
        {
            System.Diagnostics.Process.Start(STEAM_URL);
        }


        public void SaveAndExit()
        {
            SceneManager.LoadScene(GameConstants.START_SCENE);
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
            hintText.gameObject.SetActive(GameData.Level < MAX_DEMO_LEVEL);
            if(GameData.Level >= MAX_DEMO_LEVEL) 
            {
                hintText.gameObject.SetActive(false);
                return;
            }
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

    }


}
