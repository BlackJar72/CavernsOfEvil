using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToStartMenu : MonoBehaviour
{
    public void ToStartScreen()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
    }
}
