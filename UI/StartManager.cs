using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

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

        [SerializeField] EventSystem eventSystem;
        [SerializeField] GameObject startButton;
        [SerializeField] GameObject optBackButton;
        [SerializeField] GameObject helpBackButton;


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
            TakeDownBackstory();
        }


        public void TakeDownBackstory() {
            backstoryScreen.SetActive(false);
            eventSystem.SetSelectedGameObject(startButton);
        }


        public void OnDifficultyUpdate()
        {
            difficulty = (DifficultySettings)(difficultyMenu.value + 1);
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


        public void GoToOptions()
        {
            optionsScreen.SetActive(true);
            startScreen.SetActive(false);
            eventSystem.SetSelectedGameObject(optBackButton);
        }


        public void ShowHelpScreen() {
            helpScreen.SetActive(true);
            eventSystem.SetSelectedGameObject(helpBackButton);
        }


        public void HideHelpScreen() {
            helpScreen.SetActive(false);
            eventSystem.SetSelectedGameObject(startButton);
        }
    }

}