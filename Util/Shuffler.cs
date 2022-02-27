using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public static class Shuffler
    {
        public static void Shuffle<T>(this IList<T> list, Xorshift random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.NextInt(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
