using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    public struct StepDataAI
    {
        public bool passable;
        public bool reachable;
        public bool reversable;
        public bool safe;
        public bool Desireable => passable && reachable && reversable && safe;
        public bool Valid => passable && reachable;
    }

}