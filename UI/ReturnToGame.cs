using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class ReturnToGame : MonoBehaviour
    {
        [SerializeField] MovePlayer playerMove;
        [SerializeField] PlayerAct playerAct;

        [SerializeField] GameObject startScreen;
        [SerializeField] GameObject optionsScreen;


        public void Return()
        {
            gameObject.SetActive(false);
            if(!playerAct.PlayerScript.IsDead)
            {
                playerAct.enabled = true;
                playerMove.enabled = true;
            }
            GameManager.instance.SetupAudio();
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }


        public void GoToOptions()
        {
            optionsScreen.SetActive(true);
            startScreen.SetActive(false);
        }

    }
}