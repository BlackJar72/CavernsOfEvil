using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    public class PitAdder : PlatformAdder
    {
        public PitAdder(Degree chance) : base(chance) 
        {
            isDepression = true;
        }
    }

}
