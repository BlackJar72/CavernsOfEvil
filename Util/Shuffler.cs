using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil {

    public static class Shuffler
    {
        /// <summary>
        /// Shuffle the list using the provided Xorshift RNG
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="random"></param>
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


        /// <summary>
        /// Shuffle the lsit using UnityEngine.Random
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }


        /// <summary>
        /// This will move a certain element to the front of the list, assuming it is in the list.
        /// If it is not in the list this will do nothing.  If there is more that one instance of
        /// the intended first element, it will move the first one to appear in the lst.
        ///
        /// Note that this compares by referrence (effectively memory address), and thus for it to
        /// work elements must be the exact same object, not just equivalent (i.e., being equal to
        /// the '==' operator is not sufficient).
        ///
        /// This is useful for assigning specific content to the first level of a game when it would
        /// otherwise be shuffled.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="first">The element to be moved to the front of the list.</param>
        public static void MoveToFront<T>(this IList<T> list, T first) {
            if(list.Count < 2) return;
            for(int i = 0; i < list.Count; i++) {
                if(Object.ReferenceEquals(list[i], first)) {
                    list[i] = list[0];
                    list[0] = first;
                    i = list.Count;
                    break;
                }
            }
        }
    }

}
