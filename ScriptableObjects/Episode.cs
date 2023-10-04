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

        private static List<DungeonTheme> themeList = new List<DungeonTheme>();


        public DungeonTheme[] Themes { get { return themes; } }


        public DungeonTheme SelectTheme(Xorshift random)
        {
            if(GameData.Level == 1) {
                Init();
                themeList.Shuffle(random);
                themeList.MoveToFront(level1Theme);
                return level1Theme;
            }
            else {
                int index = (GameData.Level - 1) % themeList.Count;
                if(index == 0) themeList.Shuffle(random);
                DungeonTheme theme = themeList[index];
                // Should now always be false, but left in as a fail safe
                if((GameData.Level == 2) && (theme == level1Theme)) {
                    int tries = 0;
                    while ((theme == level1Theme) && (tries < 12)) {
                        theme = themes[random.NextInt(themes.Length)];
                    }
                }
                Debug.Log(theme.name);
                return theme;
            }
        }


        private void Init() {
            for(int i = 0; i < themes.Length; i++) {
                themeList.Add(themes[i]);
            }
        }


        public void NewEpisode() {
            Init();
        }

    }


}
