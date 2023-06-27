using QFSW.QC;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil {

    public class EndLevel : MonoBehaviour
    {
        private Level level;

        [Command("win")]
        public void End()
        {
            level = GameObject.Find("Level").GetComponent<Level>();
            ScoreData.endTime = Time.time;
            ScoreData.totalKills = level.MobsKilled();
            SceneManager.LoadScene("LoadingScreen");
        }


        [Command("jump")]
        public void jump(int to)
        {
            GameData.Level = to - 1;
            level = GameObject.Find("Level").GetComponent<Level>();
            ScoreData.endTime = Time.time;
            ScoreData.totalKills = level.MobsKilled();
            SceneManager.LoadScene("LoadingScreen");
        }
    }

}