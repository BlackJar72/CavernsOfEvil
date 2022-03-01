using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponents : MonoBehaviour
{
    public GameObject floor;
    public GameObject walls;
    public GameObject pillars;
    public GameObject ceiling;
    public GameObject liquids;
    public GameObject submerged;

    public GameObject[] MakeNew()
    {
        GameObject[] output = new GameObject[6];
        output[0] = Instantiate(floor);
        output[1] = Instantiate(walls);
        output[2] = Instantiate(pillars);
        output[3] = Instantiate(ceiling);
        output[4] = Instantiate(liquids);
        output[5] = Instantiate(submerged);
        return output;
    }
}
