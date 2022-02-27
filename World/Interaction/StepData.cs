using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public enum FloorEffect
    {
        none = 0, // No effect, normal floor
        slow = 1, // Water, mud, lava, etc., force lowest speed factor for player
        ice  = 2   // Slippery; more physical and less responsinve movement
    }


    /// <summary>
    /// Data on a give location in 3D space, used to pass data relevant ot PCs and mobs, mostly 
    /// related to any effect caused by being that location.
    /// 
    /// This is not intended for use in AI planning; a StepDataAI should instead be the basis 
    /// of that.
    /// </summary>
    // FIXME?: Should this be a class or a struct?  I'm not sure...?
    public struct StepData
    {
        public int roomID; // Should this just be a raw Room refference? (-1 for invalid location.)
        public bool onGround; // Is the entities y ~= the tiles y?
        public bool inLiquid; // Tile has a liquid and enity y < floor y + liquid depth
        public FloorEffect floorEffect;
        public Damages? damages; // Environmental damage caused standing there
    }

}