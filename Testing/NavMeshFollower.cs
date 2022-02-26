using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DLD;

public class NavMeshFollower : MonoBehaviour
{
    private NavMeshAgent agent;
    private HubRoom[] hubs;
    private int nextHub = 1;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if((transform.position - agent.destination).sqrMagnitude < 1)
        {
            nextHub = (nextHub + 1) % hubs.Length;
            Room room = hubs[nextHub].theRoom;
            agent.destination = new Vector3(room.realX, room.floorY, room.realZ);
        }
    }


    public void SetTarget(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }


    public void Init(HubRoom[] hubs) 
    {
        this.hubs = hubs;
    }
}
