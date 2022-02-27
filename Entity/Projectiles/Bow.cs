using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class Bow : MonoBehaviour
    {
        Animator anim;

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
        }


        public void shoot()
        {
            if (anim != null) anim.SetTrigger("Shoot");
        }
    }

}