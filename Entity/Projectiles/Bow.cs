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


        public void Shoot()
        {
            anim.SetTrigger("Shoot");
        }


        public void Release()
        {
            anim.SetBool("Release", true);
        }
    }

}