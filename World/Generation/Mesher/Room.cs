using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace CevarnsOfEvil
{

    public partial class Room
    {
        MapMatrix map;
        Level dungeon;

        public GameObject geometry;
        public GameObject floor;
        public GameObject walls;
        public GameObject pillars;
        public GameObject ceiling;
        public GameObject liquids;


        public void BuildRoom(RoomComponents parts)
        {
            geometry = new GameObject("Room " + id);
            geometry.transform.SetParent(dungeon.transform);
            geometry.transform.position = Vector3.zero;
            GameObject[] meshables = parts.MakeNew();

            floor = meshables[0];
            floor.transform.SetParent(geometry.transform);
            floor.GetComponent<Mesher>().Substance = theme.floorSubstance;

            walls = meshables[1];
            walls.transform.SetParent(geometry.transform);
            walls.GetComponent<Mesher>().Substance = theme.wallSubstance;

            pillars = meshables[2];
            pillars.transform.SetParent(geometry.transform);
            pillars.GetComponent<Mesher>().Substance = theme.pillarSubstance;

            ceiling = meshables[3];
            ceiling.transform.SetParent(geometry.transform);
            ceiling.GetComponent<Mesher>().Substance = theme.ceilingSubstance;

            liquids = meshables[4];
            liquids.transform.SetParent(geometry.transform);
            liquids.GetComponent<Mesher>().Substance = theme.liquidSubstance;
            if(theme.liquidSubstance.IsLiquid)
            {
                liquids.GetComponent<Collider>().enabled = false;
            }
            else 
            {
                if (theme.liquidSubstance.Damage > 0) liquids.layer = 13;
                else liquids.layer = 9;
            }

            map.MeshRoom(this);
            floor.isStatic = true;
            walls.isStatic = true;
            pillars.isStatic = true;
            ceiling.isStatic = true;
            liquids.isStatic = true;
        }

    }
}
