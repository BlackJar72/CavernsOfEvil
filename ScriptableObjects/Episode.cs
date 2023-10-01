using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/Episode", fileName = "Episode", order = 111)]
    public class Episode : ScriptableObject
    {
        [SerializeField] DungeonTheme[] themes;
        [SerializeField] DungeonTheme level1Theme;
        [SerializeField] bool hasLastLevel;
        [SerializeField] int lastLevel;
        [SerializeField] Episode nextEpisode;
        [SerializeField] bool isFinalEpisode;
        [SerializeField] bool isDemoEpisode;


        public DungeonTheme[] Themes { get { return themes; } }


        public DungeonTheme SelectTheme(Xorshift random)
        {
            if(GameData.Level == 1) return level1Theme;
            else {
                DungeonTheme theme = themes[random.NextInt(themes.Length)];
                if(GameData.Level == 2) {
                    int tries = 0;
                    while ((theme == level1Theme) && (tries < 12)) {
                        theme = themes[random.NextInt(themes.Length)];
                    }
                }
                return theme;
            }
        }
    }

}
