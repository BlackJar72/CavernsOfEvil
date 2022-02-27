using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/Degree List", fileName = "DegreeList", order = 104)]
    public class DegreeList : ScriptableObject
    {
        public int none;
        public int few;
        public int some;
        public int plenty;
        public int heaps;
        public int always;

        public Degree Select(Xorshift random)
        {
            int roll = random.NextInt(none + few + some + plenty + heaps + always);
            roll -= none;
            if (roll < 0) return Degree.NONE;
            roll -= few;
            if (roll < 0) return Degree.FEW;
            roll -= some;
            if (roll < 0) return Degree.SOME;
            roll -= plenty;
            if (roll < 0) return Degree.PLENTY;
            roll -= heaps;
            if (roll < 0) return Degree.HEAPS;
            return Degree.ALWAYS;
        }

        public Degree Select()
        {
            int roll = Random.Range(0, none + few + some + plenty + heaps + always);
            roll -= none;
            if (roll < 0) return Degree.NONE;
            roll -= few;
            if (roll < 0) return Degree.FEW;
            roll -= some;
            if (roll < 0) return Degree.SOME;
            roll -= plenty;
            if (roll < 0) return Degree.PLENTY;
            roll -= heaps;
            if (roll < 0) return Degree.HEAPS;
            return Degree.ALWAYS;
        }
    }
}