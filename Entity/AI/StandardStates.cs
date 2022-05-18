using System;
using UnityEngine;
using static CevarnsOfEvil.EStandardStates;


namespace CevarnsOfEvil 

{

[Serializable]
public class StandardStates
{
    [SerializeField] BehaviorObject idle;
    [SerializeField] BehaviorObject wander;
    [SerializeField] BehaviorObject chase;
    [SerializeField] BehaviorObject melee;
    [SerializeField] BehaviorObject missle;
    [SerializeField] BehaviorObject pain;
    [SerializeField] BehaviorObject dead;

    [SerializeField] BehaviorObject[] special;



    public BehaviorObject GetState(EStandardStates label) 
    {
        switch(label) {
            case IDLE: return idle;
            case WANDER: return wander;
            case CHASE: return chase;
            case MELEE: return melee;
            case MISSLE: return missle;
            case PAIN: return pain;
            case DEAD: return dead;
            default: return idle;
        }
    }


    public BehaviorObject GetSpecialState(int id) 
    {
        if((special != null) && (id > 0) && (id < special.Length)) return special[id];
        else return null;
    }


}


public enum EStandardStates 
{
    IDLE,
    WANDER,
    CHASE,
    MELEE,
    MISSLE,
    PAIN,
    DEAD
}


}