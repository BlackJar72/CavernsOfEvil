using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/Episode", fileName = "Episode", order = 111)]
    public class Episode : ScriptableObject
    {
        [SerializeField] List<DungeonTheme> themes;
        [SerializeField] DungeonTheme level1Theme;
        [SerializeField] bool hasLastLevel;
        [SerializeField] int lastLevel;
        [SerializeField] Episode nextEpisode;
        [SerializeField] bool isFinalEpisode;
        [SerializeField] bool isDemoEpisode;


        public List<DungeonTheme> Themes { get { return themes; } }


        public DungeonTheme SelectTheme(Xorshift random)
        {
            if(GameData.Level == 1) {
                themes.Shuffle(random);
                themes.MoveToFront(level1Theme);
                return level1Theme;
            }
            else {
                int index = (GameData.Level - 1) % themes.Count;
                if(index == 0) themes.Shuffle(random);
                DungeonTheme theme = themes[index];
                // Should now always be false, but left in as a fail safe
                if((GameData.Level == 2) && (theme == level1Theme)) {
                    int tries = 0;
                    while ((theme == level1Theme) && (tries < 12)) {
                        theme = themes[random.NextInt(themes.Count)];
                    }
                }
                return theme;
            }
        }

    }


}
