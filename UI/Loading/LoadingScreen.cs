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
            int hours = 0;
            int minutes = 0;
            int seconds = (int)(endTime - startTime);

            if(seconds > 60)
            {
                minutes = seconds / 60;
                seconds = seconds % 60;
            }
            if(minutes > 60)
            {
                hours = minutes / 60;
                minutes = minutes % 60;
            }

            StringBuilder sb = new StringBuilder();
            if (hours > 0)
            {
                sb.Append(hours);
                if(hours > 1) sb.Append(" hours, ");
                else sb.Append(" hour, ");
            }
            if (minutes > 0)
            {
                sb.Append(minutes);
                if(minutes > 1) sb.Append(" minutes, ");
                else sb.Append(" minute, ");
            }
            sb.Append(seconds);
            if(seconds == 1) sb.Append(" second");
            else sb.Append(" seconds");
            return sb.ToString();
        }

        public static string GetKillsString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(totalKills);
            sb.Append(" of ");
            sb.Append(totalMobs);
            sb.Append(" (");
            sb.Append((int)(((float)totalKills / (float)totalMobs) * 100));
            sb.Append("%)");
            return sb.ToString();
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
                    levelText.text = "Level " + GameData.Level;
                    timeText.text = "Time: " + ScoreData.GetTimeString();
                    killsText.text = "Kills: " + ScoreData.GetKillsString();
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
                hintText.text = shuffledHints[which];
            } else {
                hintText.text = hints[Random.Range(0, hints.Length)];
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