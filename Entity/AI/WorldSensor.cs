using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{

    public class WorldSensor : MonoBehaviour
    {
        [SerializeField] IWorldSensorUser owner;

        [Tooltip("The main collider for physical collision; "
        + "\n this can be null but some features will be inactivated. "
        + "\n this should be set as a trigger to work properly.")]
        [SerializeField] Collider sensorCollider;

        private StepDataAI stepData;


        void OnTriggerEnter(Collider other) => owner.OnWorldSensorTriggered(other);
        void OnTriggerExit(Collider other) => owner.OnWorldSensorExit(other);


        // Start is called before the first frame update
        void Start()
        {
            
        }


        // Update is called once per frame
        void Update()
        {
            stepData = DungeonManager.instance
                .GetAIDataForGround(transform.position, owner.Destination, owner as EntityMob);
        }


    }


}