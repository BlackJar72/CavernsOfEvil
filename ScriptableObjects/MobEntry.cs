using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {


    [CreateAssetMenu(menuName = "DLD/Mob Entry", fileName = "MobEntry", order = 112)]
    public class MobEntry : ScriptableObject
    {
        [SerializeField] protected string mobName;
        [SerializeField] GameObject mobPrefab;
        [SerializeField] protected int mobLevel;
        [SerializeField] MobSize size = MobSize.medium;


        public string MobName => mobName;
        public GameObject MobPrefab => mobPrefab;
        public int MobLevel => mobLevel;
        public MobSize Size { get { return size; } }


        /// <summary>
        /// This is the preferred way to get a mob prefab, as it will be 
        /// equivalent to MobPrefab for this class, but is overriden to 
        /// select a random mob in MobEntryMulti.  Thus, using this 
        /// consistently will facillitate the polymorphic use of both 
        /// classes.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public virtual GameObject GetMob(Xorshift random) => mobPrefab;
    }

}