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
        private static bool justLoaded = true;

        public string seedString = ""; 
        public DifficultySettings difficulty = DifficultySettings.norm;

        [SerializeField] TMP_Dropdown difficultyMenu;
        [SerializeField] TMP_InputField seedField;

        [SerializeField] GameObject startScreen;
        [SerializeField] GameObject optionsScreen;
        [SerializeField] GameObject backstoryScreen;
        [SerializeField] GameObject helpScreen;

        void Start()
        {
            Time.timeScale = 1;
            QualitySettings.vSyncCount = 1;
            Cursor.lockState = CursorLockMode.None;
            optionsScreen.GetComponent<Options>().Init();
            if(justLoaded) {
                StartCoroutine(TakeDownBackstory(30));
            } else {
                TakeDownBackstory();
            }
            justLoaded = false;
        }

        // Update is called once per frame
        /*void Update()
        {

        }*/


        IEnumerator TakeDownBackstory(float time) {
            yield return new WaitForSeconds(time);
            backstoryScreen.SetActive(false);
        }


        public void TakeDownBackstory() {
            backstoryScreen.SetActive(false);
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


        public void GoToOptions()
        {
            optionsScreen.SetActive(true);
            startScreen.SetActive(false);
        }


        public void ShowHelpScreen() {
            helpScreen.SetActive(true);
        }


        public void HideHelpScreen() {
            helpScreen.SetActive(false);
        }
    }

}