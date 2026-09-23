using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class SpectreVoice : MonoBehaviour
    {
        public void Activate()
        {
            transform.parent = transform.root;
            StartCoroutine(Vanish());
        }


        IEnumerator Vanish()
        {
            yield return new WaitForSeconds(3.0f);
            Destroy(gameObject);
        }



    }


}