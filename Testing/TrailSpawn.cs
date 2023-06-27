using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailSpawn : MonoBehaviour
{
    [SerializeField] GameObject trailOject;

    // Update is called once per frame
    void Update()
    {
        Instantiate(trailOject, transform.position, transform.rotation);
    }


    /*void FixedUpdate()
    {
        Instantiate(trailOject, transform.position, transform.rotation);
    }*/


}
