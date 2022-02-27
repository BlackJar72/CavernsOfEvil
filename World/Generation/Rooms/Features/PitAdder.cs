using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public class PitAdder : PlatformAdder
    {
        public PitAdder(Degree chance) : base(chance) 
        {
            isDepression = true;
        }
    }

}
