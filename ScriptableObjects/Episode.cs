using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DLD
{
    [CreateAssetMenu(menuName = "DLD/Episode", fileName = "Episode", order = 111)]
    public class Episode : ScriptableObject
    {
        [SerializeField] DungeonTheme[] themes;
        [SerializeField] bool hasLastLevel;
        [SerializeField] int lastLevel;
        [SerializeField] Episode nextEpisode;
        [SerializeField] bool isFinalEpisode;
        [SerializeField] bool isDemoEpisode;


        public DungeonTheme[] Themes { get { return themes; } }


        public DungeonTheme SelectTheme(Xorshift random)
        {
            return themes[random.NextInt(themes.Length)];
        }
    }

}