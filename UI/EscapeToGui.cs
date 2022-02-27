using UnityEngine;
using UnityEngine.InputSystem;


namespace CevarnsOfEvil
{

    public class EscapeToGui : MonoBehaviour
    {
        [SerializeField] GameObject menu;

        private PlayerInput input;
        private MovePlayer playerMove;
        private PlayerAct playerAct;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            playerAct = GetComponent<PlayerAct>();
            playerMove = GetComponent<MovePlayer>();
            input.actions["Escape to Menu"].started += ToggleMenu;
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (menu != null)
            {
                if (menu.active)
                {
                    playerAct.usingItem = false;
                    menu.GetComponent<ReturnToGame>().Return();
                }
                else
                {
                    menu.SetActive(true);
                    playerAct.enabled = false;
                    playerMove.enabled = false;
                    Time.timeScale = 0;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

    }
}