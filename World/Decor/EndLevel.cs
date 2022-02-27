using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil {

    public class EndLevel : MonoBehaviour
    {
        private Level level;

        public void End()
        {
            level = GameObject.Find("Level").GetComponent<Level>();
            ScoreData.endTime = Time.time;
            ScoreData.totalKills = level.MobsKilled();
            SceneManager.LoadScene("LoadingScreen");
        }
    }

}