using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class TempEffect : MonoBehaviour {
        [SerializeField] float timeToLive;

        private float timeToDie;

        void Start() {
            timeToDie = Time.time + timeToLive;
        }

        // Update is called once per frame
        void Update() {
            if(timeToDie < Time.time) Destroy(gameObject);
        }
    }


}