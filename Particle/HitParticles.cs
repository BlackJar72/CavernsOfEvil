using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitParticles : MonoBehaviour
{
    private float timeToDie;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<ParticleSystem>().Play();
        timeToDie = Time.time + 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeToDie)
        {
            Destroy(gameObject);
        }
    }
}
