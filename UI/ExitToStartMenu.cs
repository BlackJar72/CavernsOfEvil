using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil {

    public class ExitToStartMenu : MonoBehaviour
    {
        public void ToStartScreen()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(GameConstants.START_SCENE);
        }
    }

}
