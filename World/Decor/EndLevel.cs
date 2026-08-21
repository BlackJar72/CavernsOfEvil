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
            level = Level.Instance; // GameObject.Find("Level").GetComponent<Level>();
            ScoreData.endTime = Time.time;
            ScoreData.totalKills = level.MobsKilled();
            UIManager.Instance.ShowIntermission();
        }


        [Command("jump")]
        public void jump(int to)
        {
            GameData.Level = to - 1;
            level = Level.Instance; // GameObject.Find("Level").GetComponent<Level>();
            ScoreData.endTime = Time.time;
            ScoreData.totalKills = level.MobsKilled();
            UIManager.Instance.ShowIntermission();
        }
    }

}