using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class Disappear : MonoBehaviour {
        [SerializeField] float timeToLive;
        [SerializeField] Material mat;

        private float startTime;
        private float endTime;


        void Update() {
            if(Time.time > endTime) gameObject.SetActive(false);
            else {
                Color color = mat.color;
                color.a = Mathf.Sin(Mathf.Clamp(((endTime - startTime) / (timeToLive + 1)) * Mathf.PI, 0, Mathf.PI));
                mat.color = color;
            }
        }


        void OnEnable() {
            startTime = Time.time;
            endTime = startTime + timeToLive + 1;
            //StartCoroutine(TurnOff());
        }


        IEnumerator TurnOff() {
            yield return new WaitForSeconds(timeToLive);
            gameObject.SetActive(false);
        }


    }

}

