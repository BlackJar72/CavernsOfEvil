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

        private bool unplugged;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            playerAct = GetComponent<PlayerAct>();
            playerMove = GetComponent<MovePlayer>();
            input.actions["Escape to Menu"].started += ToggleMenu;
            InputSystem.onDeviceChange += PauseForController;
            unplugged = false;
        }

        private void ToggleMenu(InputAction.CallbackContext context)
        {
            if (menu != null)
            {
                if (menu.active)
                {
                    unplugged = false; // In case stopped to switch to mouse & keyboard
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


        private void PauseForController(InputDevice device, InputDeviceChange change) {
            if((change == InputDeviceChange.Disconnected) && (menu != null))
            {
                unplugged = true;
                menu.SetActive(true);
                playerAct.enabled = false;
                playerMove.enabled = false;
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
            } else if (unplugged && (change == InputDeviceChange.Reconnected) && (menu != null)) {
                unplugged = false;
                playerAct.usingItem = false;
                menu.GetComponent<ReturnToGame>().Return();
            }
        }

    }
}