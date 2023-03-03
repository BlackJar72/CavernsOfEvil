using UnityEngine;
using TMPro;
using System.Text;
using System.Collections;
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

        [SerializeField] GameObject scores;
        [SerializeField] GameObject buttons;

        private void Start()
        {
            levelText.text = "Level " + GameData.Level;
            timeText.text = "Time: " + ScoreData.GetTimeString();
            killsText.text = "Kills: " + ScoreData.GetKillsString();
            GameData.NextLevel();
            StartCoroutine(ShowPieces());
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
            buttons.SetActive(true);
        }


        public void StartNextLevel()
        {
            SceneManager.LoadScene("DungeonScene");
        }


        public void SaveAndExit()
        {
            //SaveGame.SaveGameData(SaveGame.STAND_IN);
            SceneManager.LoadScene("Start");
        }


        /*public void OnSceneLoaded() {
            //Debug.Log("SceneLoaded");
        }*/

    }


}