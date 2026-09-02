using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    /*
    This might be a waste of time.  While this is probably how I should have done this originally (and would in a new 
    project), and would have simplified the development of a lot of other things, the old system is stable and sufficient. 
    All the challenges and problems a system like this could have spared me have already been solved in the old system. 
    */


    [System.Serializable]
    public enum AIStateLabel
    {
        idle = 0,
        wander = 1,
        aggro = 2,
        flee = 3,    // This might should be a sub-state of aggro
        death = 4,
        playerdead = 5
    }


    /*
    There are two ways I could do this:

        (1) I could keep these are pure C# objects, contructing them when the character is created (probable in Awake) 
        as was the original plan.  This allows maximum control and avoids any overhead or possible gotchas associated 
        with scriptable objects.  The one drawback is each entity type (possibly including unique individuals) would 
        need to be coded as a separate subclass of EntityActor or EntityTalking. 

        (2) I could make this SystemSerializable and make AIState a scriptable object (which would act as a prototype). 
        As I now realize scriptable objects can be cloned with Instantiate into separate run-time versions I could 
        replace all the provided scriptable objects with newly instatiated clones (again, probably in Awake).  The 
        cloned versions could contain instance specific data.  This would allow for a more fully component based approach 
        and creating specific entities in the inspector, at least to a point.  One drawback is that the owner of the 
        AI could not be a readonly field set in the constructor.

    */


    [System.Serializable]
    public class AIStates
    {
        private AIState idle;
        private AIState wander;
        private AIState aggro;
        private AIState flee;    // This might should be a sub-state of aggro
        private AIState death;
        private AIState playerdead;
        

        [SerializeField] AIStateID idleID;
        [SerializeField] AIStateID wanderID;
        [SerializeField] AIStateID aggroID;
        [SerializeField] AIStateID fleeID;    // This might should be a sub-state of aggro
        [SerializeField] AIStateID deathID;
        [SerializeField] AIStateID playerdeadID;


        AIState current;
        AIState previous;
        AIStateLabel currentID;
        AIStateLabel previousID;
        AIStateLabel currentLabel;


        public void Init(EntityMob owner)
        {
            idle = AIStateLibrary.GetState(idleID, owner);
            wander = AIStateLibrary.GetState(wanderID, owner);
            aggro = AIStateLibrary.GetState(aggroID, owner);
            flee = AIStateLibrary.GetState(fleeID, owner);
            death = AIStateLibrary.GetState(deathID, owner);
            playerdead = AIStateLibrary.GetState(playerdeadID, owner);
            //SetState(owner.DefaultState); // TODO: Replace this line once EntityMob supports new system
        }


        public void Act()
        {
            current.Act();
        }


        public void SetState(AIStateLabel state)
        {
            previous = current == null ? idle : current;
            previousID = previousID;
            currentID = state;
            // Should I be using an array look-up instead?
            switch (state)
            {
                case AIStateLabel.idle:
                    ReallySetState(idle);
                    break;
                case AIStateLabel.wander:
                    ReallySetState(wander);
                    break;
                case AIStateLabel.aggro:
                    ReallySetState(aggro);
                    break;
                case AIStateLabel.flee:
                    ReallySetState(flee);
                    break;
                case AIStateLabel.death:
                    ReallySetState(death);
                    break;
                case AIStateLabel.playerdead:
                    ReallySetState(playerdead);
                    break;
                default:
                    break;
            }
        }


        private void ReallySetState(AIState next)
        {
            if (current == null)
            {
                previous = next;
            }
            else
            {
                current.StateExit();
                previous = current;
            }
            current = next;
            current.StateEnter();
        }


        public AIStateLabel GetAIState => currentID;


        public AIState GetCurrentState => current;


        // Need to determing if stealth attacks are really stealth
        public bool IsAggro => current == aggro;



    }


}