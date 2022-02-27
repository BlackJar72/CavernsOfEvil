using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace CevarnsOfEvil
{

    public class StartManager : MonoBehaviour
    {
        public string seedString = ""; 
        public DifficultySettings difficulty = DifficultySettings.norm;

        [SerializeField] TMP_Dropdown difficultyMenu;
        [SerializeField] TMP_InputField seedField;

        void Start()
        {
            Time.timeScale = 1;
            QualitySettings.vSyncCount = 1;
            Cursor.lockState = CursorLockMode.None;
        }

        // Update is called once per frame
        void Update()
        {

        }


        public void OnDifficultyUpdate()
        {
            difficulty = (DifficultySettings)difficultyMenu.value + 1;
        }


        public void OnSeedUpdate()
        {
            seedString = seedField.text;
        }


        public void Exit()
        {
            Application.Quit();
        }


        public void StartGame()
        {
            GameData.Init(seedString, difficulty);
            SceneManager.LoadScene("DungeonScene");
        }


        public void RestoreGame()
        {
            //SaveGame.LoadGame(SaveGame.STAND_IN);
        }
    }

}