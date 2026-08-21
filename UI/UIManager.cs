using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace CevarnsOfEvil
{

    public class UIManager : MonoBehaviour
    {
        private static UIManager instance;

        [SerializeField] GameObject gameCanvas;
        [SerializeField] GameObject loadingCanvas;
        [SerializeField] GameObject victoryCanvas;
        [SerializeField] MusicManager musicManager;

        public static UIManager Instance => instance;
        public MusicManager SoundManager => musicManager;


        private void Awake()
        {
            instance = this;
        }


        public void EnterPlayMode()
        {
            loadingCanvas.SetActive(false);
            victoryCanvas.SetActive(false);
            gameCanvas.SetActive(true);
            // TODO?? Stuff previously done when loading as a scene
            Cursor.lockState = CursorLockMode.Locked;
            Player.PCObject.SetActive(true);
            musicManager.Start();
        }


        public void ShowIntermission()
        {
            victoryCanvas.SetActive(false);
            gameCanvas.SetActive(false);
            loadingCanvas.SetActive(true);
            // TODO?? Stuff previously done when loading as a scene
            musicManager.Stop();
            LoadingScreen loader = loadingCanvas.GetComponent<LoadingScreen>();
            if(loader == null)
            {
                Debug.Log("Loading Screen script missing!");    
            } 
            else
            {
                loader.Init();
                Time.timeScale = 0.0f;
                musicManager.MuteGame();
                Player.PC.DisableInput();
                Level.Instance.gameObject.SetActive(false);
            }
        }


        public void ShowVictory()
        {
            gameCanvas.SetActive(false);
            loadingCanvas.SetActive(false);
            victoryCanvas.SetActive(true);
            // TODO?? Stuff previously done when loading as a scene
            musicManager.Stop();
            LoadingScreen loader = victoryCanvas.GetComponent<LoadingScreen>();
            if(loader == null)
            {
                Debug.Log("Loading Screen script missing!");    
            } 
            else
            {
                loader.Init();
                Time.timeScale = 0.0f;
                musicManager.MuteGame();
                Player.PC.DisableInput();
                Level.Instance.gameObject.SetActive(false);
            }
        }

    }


}
