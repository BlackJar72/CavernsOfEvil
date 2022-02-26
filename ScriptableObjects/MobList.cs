using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [System.Serializable]
    public enum MobSize
    {
        tiny,
        small,
        medium,
        large,
        huge
    }



    [CreateAssetMenu(menuName = "DLD/Mob List", fileName = "MobList", order = 112)]
    public class MobList : ScriptableObject
    {
        [SerializeField] int level = 1;
        [SerializeField] MobEntry[] mobs;

        
        public int MobLevel { get { return level; } }
        public MobEntry[] Mobs { get { return mobs; } }
        public int EntryCount { get { return mobs.Length; } }
        public bool Empty { get { return mobs.Length < 1; } }


        public GameObject GetPrefab(Xorshift random)
        {
            return mobs[random.NextInt(mobs.Length)].GetMob(random);
        }


        public MobEntry GetEntry(Xorshift random)
        {
            return mobs[random.NextInt(mobs.Length)];
        }
    }


    [System.Serializable]
    public class ThemeMobData
    {
        [SerializeField, Tooltip("The Mob Lists for this Theme")] MobList[] mobLists = new MobList[10];
        public MobList[] Lists => mobLists;
        public MobList GetList(int level) =>  mobLists[level - 1];
    }

}