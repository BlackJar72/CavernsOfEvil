using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public enum Degree
    {
        NONE	=  0,
        FEW 	=  1,
        SOME	=  3,
        PLENTY	=  5,
        HEAPS	=  9,
        ALWAYS  =  10
    }



    [CreateAssetMenu(menuName = "DLD/Dungeon Theme", fileName = "Theme", order = 110)]
    public class DungeonTheme : ScriptableObject
    {

        [SerializeField] bool isCaves = false;
        public bool IsCaves => isCaves;


        /* STYLE ELEMENTS */
        [SerializeField] ThemeArchitecturalData architecturalData;
        public DegreeList Pools => architecturalData.Pools;
        public DegreeList Doors => architecturalData.Doors; 
        public DegreeList Subrooms => architecturalData.Subrooms;
        public DegreeList Pillars => architecturalData.Pillars;
        public DegreeList Symmetry => architecturalData.Symmetry;
        public DegreeList Variability => architecturalData.Variability;
        public DegreeList Complexity => architecturalData.Complexity;
        public DegreeList Verticality => architecturalData.Verticality;
        public DegreeList Naturals => architecturalData.Naturals;


        /* MATERIAL ELEMENTS */
        [SerializeField] ThemeSubstanceData substanceData;
        public Substance[] WallSubstances => substanceData.WallSubstances;
        public Substance[] FloorSubstances => substanceData.FloorSubstances;
        public Substance[] CeilingSubstances => substanceData.CeilingSubstances;
        public Substance[] LiquidSubstances => substanceData.LiquidSubstances;
        public Substance[] PillarSubstances => substanceData.PillarSubstances;
        public Substance[] CaveSubstances => substanceData.CaveSubstances;


        /* MOB ELEMENTS */
        [SerializeField] ThemeMobData mobLists;
        public ThemeMobData MobLists => mobLists;


        public static bool UseDegree(Degree degree, Xorshift random) 
            => random.NextInt(10) < (int)degree;

    }


    [System.Serializable]
    public class ThemeSubstanceData
    {
        [SerializeField] Substance[] wallSubstances;
        [SerializeField] Substance[] floorSubstances;
        [SerializeField] Substance[] ceilingSubstances;
        [SerializeField] Substance[] liquidSubstances;
        [SerializeField] Substance[] pillarSubstances;
        [SerializeField] Substance[] caveSubstances;

        public Substance[] WallSubstances => wallSubstances;
        public Substance[] FloorSubstances => floorSubstances;
        public Substance[] CeilingSubstances => ceilingSubstances;
        public Substance[] LiquidSubstances => liquidSubstances;
        public Substance[] PillarSubstances => pillarSubstances;
        public Substance[] CaveSubstances => caveSubstances;
    }


    [System.Serializable]
    public class ThemeArchitecturalData
    {
        [SerializeField] DegreeList pools;
        [SerializeField] DegreeList doors;
        [SerializeField] DegreeList subrooms;
        [SerializeField] DegreeList pillars;
        [SerializeField] DegreeList symmetry;
        [SerializeField] DegreeList variability;
        [SerializeField] DegreeList complexity;
        [SerializeField] DegreeList verticality;
        [SerializeField] DegreeList naturals;

        public DegreeList Pools => pools;
        public DegreeList Doors => pillars;
        public DegreeList Subrooms => subrooms;
        public DegreeList Pillars => pillars;
        public DegreeList Symmetry => symmetry;
        public DegreeList Variability => variability;
        public DegreeList Complexity => complexity;
        public DegreeList Verticality => verticality;
        public DegreeList Naturals => naturals;
    }

}
