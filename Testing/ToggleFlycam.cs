using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public class ToggleFlycam : MonoBehaviour
    {
        [SerializeField] GameObject player;
        [SerializeField] Collider[] colliders;
        [SerializeField] Camera plyaerViewer;

        private Player playerScript;
        private PlayerHealth playerHealth;
        private MovePlayer playerMover;
        private PlayerAct playerActor;
        private Rigidbody playerPhysics;

        private bool flying;


        // Start is called before the first frame update
        void Start()
        {
            playerScript = player.GetComponent<Player>();
            playerHealth = player.GetComponent<PlayerHealth>();
            playerMover = player.GetComponent<MovePlayer>();
            playerActor = player.GetComponent<PlayerAct>();
            playerPhysics = player.GetComponent<Rigidbody>();
            flying = false;
        }

/*
#if UNITY_EDITOR
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.F10))
            {
                if(flying) ExitFlyMode();
                else EnterFlyMode();
            }
        }
#endif
*/

        private void EnterFlyMode()
        {
            playerMover.flying = flying = true;
            playerScript.enabled = false;
            playerHealth.enabled = false;
            playerActor.enabled = false;
            playerPhysics.isKinematic = true;
            foreach(Collider collider in colliders)
            {
                collider.enabled = false;
            }
            plyaerViewer.enabled = false;
        }


        private void ExitFlyMode()
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
            plyaerViewer.enabled = true;
            playerScript.enabled = true;
            playerHealth.enabled = true;
            playerMover.enabled = true;
            playerActor.enabled = true;
            playerPhysics.isKinematic = false;
            playerMover.flying = flying = false;
        }

    }


}