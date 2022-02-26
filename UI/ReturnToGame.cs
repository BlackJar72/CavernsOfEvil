using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class ReturnToGame : MonoBehaviour
    {
        [SerializeField] MovePlayer playerMove;
        [SerializeField] PlayerAct playerAct;

        public void Return()
        {
            gameObject.SetActive(false);
            if(!playerAct.PlayerScript.IsDead)
            {
                playerAct.enabled = true;
                playerMove.enabled = true;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }

    }
}