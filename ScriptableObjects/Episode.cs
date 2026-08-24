using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    [CreateAssetMenu(menuName = "DLD/Episode", fileName = "Episode", order = 111)]
    public class Episode : ScriptableObject
    {
        [SerializeField] List<DungeonTheme> themes;
        [SerializeField] int level1Theme;
        [SerializeField] bool hasLastLevel;
        [SerializeField] int lastLevel;
        [SerializeField] Episode nextEpisode;
        [SerializeField] bool isFinalEpisode;
        [SerializeField] bool isDemoEpisode;


        public List<DungeonTheme> Themes { get { return themes; } }


        public DungeonTheme SelectTheme(Xorshift random)
        {
            if(GameData.Level == 1) {
                GameData.ShuffleThemes(random);
                GameData.MoveThemeToFront(level1Theme);
                return themes[GameData.GetThemeID(0)];
            }
            else {
                int index = (GameData.Level - 1) % themes.Count;
                if(index == 0) GameData.ShuffleThemes(random);
                DungeonTheme theme = themes[GameData.GetThemeID(index)];
                return theme;
            }
        }

    }


}
