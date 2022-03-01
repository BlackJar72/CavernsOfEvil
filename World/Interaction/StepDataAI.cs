using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public struct StepDataAI
    {
        public bool passable;
        public bool reachable;
        public bool reversable;
        public bool safe;
        public float height;
        public float deltay;
        public bool Desireable => passable && reachable && reversable && safe;
        public bool Valid => passable && reachable;

        public override string ToString()
        {
            return "\n Passible: " + passable
                + "\n Reachable: " + reachable
                + "\n Reversable: " + reversable
                + "\n Safe: " + safe
                + "\n Height: " + height
                + "\n Delta Y: " + deltay + "\n";
        }
    }

}