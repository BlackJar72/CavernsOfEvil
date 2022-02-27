using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public class HubPlacer
    {
        private static readonly Vector2 s00 = new Vector2(0.0f, 0.0f);
        private static readonly Vector2 s01 = new Vector2(0.0f, 0.25f);
        private static readonly Vector2 s02 = new Vector2(0.0f, 0.50f);
        private static readonly Vector2 s03 = new Vector2(0.0f, 0.75f);

        private static readonly Vector2 s10 = new Vector2(0.25f, 0.0f);
        private static readonly Vector2 s11 = new Vector2(0.25f, 0.25f);
        private static readonly Vector2 s12 = new Vector2(0.25f, 0.50f);
        private static readonly Vector2 s13 = new Vector2(0.25f, 0.75f);

        private static readonly Vector2 s20 = new Vector2(0.50f, 0.0f);
        private static readonly Vector2 s21 = new Vector2(0.50f, 0.25f);
        private static readonly Vector2 s22 = new Vector2(0.50f, 0.50f);
        private static readonly Vector2 s23 = new Vector2(0.50f, 0.75f);

        private static readonly Vector2 s30 = new Vector2(0.75f, 0.0f);
        private static readonly Vector2 s31 = new Vector2(0.75f, 0.25f);
        private static readonly Vector2 s32 = new Vector2(0.75f, 0.50f);
        private static readonly Vector2 s33 = new Vector2(0.75f, 0.75f);

        private static readonly Vector2[] sectors = { s00, s01, s02, s03,
                                                     s10, s11, s12, s13,
                                                     s20, s21, s22, s23,
                                                     s30, s31, s32, s33};

        private static readonly Vector2[] qc = { s11, s12, s21, s22 };

        private static readonly Vector2[] eq1 = { s00, s01, s10 };
        private static readonly Vector2[] eq2 = { s02, s03, s13 };
        private static readonly Vector2[] eq3 = { s22, s23, s33 };
        private static readonly Vector2[] eq4 = { s20, s30, s31 };
        private static readonly Vector2[][] eqall = { eq1, eq2, eq3, eq4 };


        public static Vector2Int[] FindLocations(Xorshift random, SizeData levelSize, int num)
        {
            Vector2Int[] locations = new Vector2Int[Mathf.Max(num, 2)];
            List<int> quandrants = new List<int>();
            List<Vector2> sectlist = new List<Vector2>();
            for (int i = 0; i < 4; i++) quandrants.Add(i);
            for (int i = 0; i < sectors.Length; i++) sectlist.Add(sectors[i]);

            int quad = quandrants[random.NextInt(4)];
            locations[0] = FindLocationInSector(random, levelSize, eqall[quad][random.NextInt(3)]);
            sectlist.Remove(locations[0]);
            quandrants.Remove(quad);

            quad = (quad + 2) % 4;
            locations[1] = FindLocationInSector(random, levelSize, eqall[quad][random.NextInt(3)]);
            sectlist.Remove(locations[1]);
            quandrants.Remove(quad);

            if (num > 2)
            {
                quad = quandrants[random.NextInt(2)];
                locations[2] = FindLocationInSector(random, levelSize, eqall[quad][random.NextInt(3)]);
                sectlist.Remove(locations[2]);
                quandrants.Remove(quad);
            }
            if (num > 3)
            {
                quad = quandrants[0];
                locations[3] = FindLocationInSector(random, levelSize, eqall[quad][random.NextInt(3)]);
                sectlist.Remove(locations[3]);
                quandrants.Remove(quad);
            };
            if (num > 4)
            {
                locations[4] = FindLocationInSector(random, levelSize, qc[random.NextInt(4)]);
                sectlist.Remove(locations[4]);
            }
            if(num > 5)
            {
                for(int i = 5; i < num; i++)
                {
                    locations[i] = FindLocationInSector(random, levelSize, sectlist[random.NextInt(sectlist.Count)]);
                    sectlist.Remove(locations[i]);
                }
            }
            return locations;
        }


        public static Vector2Int FindLocationInSector(Xorshift random, SizeData levelSize, Vector2 sector) 
        {
            int x = (int)(sector.x * levelSize.width) + random.NextInt(levelSize.width / 4);
            int y = (int)(sector.y * levelSize.width) + random.NextInt(levelSize.width / 4);
            return new Vector2Int(x, y);
        }
    
}

}