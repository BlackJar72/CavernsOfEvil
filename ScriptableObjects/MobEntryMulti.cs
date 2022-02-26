using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{

    [CreateAssetMenu(menuName = "DLD/Multi-Mob Entry", fileName = "MobEntry", order = 113)]
    public class MobEntryMulti : MobEntry
    {
        [SerializeField] GameObject[] otherPrefabs;

        private GameObject[] allPrefabs;


        void Awake()
        {
            allPrefabs = new GameObject[otherPrefabs.Length + 1];
            allPrefabs[0] = base.MobPrefab;
            for (int i = 0; i < otherPrefabs.Length; i++)
            {
                allPrefabs[i + 1] = otherPrefabs[i];
            }
        }


        public override GameObject GetMob(Xorshift random)
        {
            return allPrefabs[random.NextInt(allPrefabs.Length)];
        }

    }

}