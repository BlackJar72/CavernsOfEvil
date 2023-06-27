using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace kfutils {

    public static class KFMath {

        /**
         * This is will produce an always positive modulus,
         * that is, a remainder from the next lower number
         * even when negative.  Many situations require this,
         * such as when locating a value in a 2D grid stored
         * as a 1D array.
         *
         * @param a the dividend
         * @param b the divisor
         * @return the positive modulus
         */
        public static int ModRight(int a, int b) {
            return (a & 0x7fffffff) % b;
        }


        /**
         * A convenience method, but one probably better coded locally in
         * most situations for efficiency (at least in intended uses).  In
         * some ways this is a reminder, but could be handy in non-performance
         * critical code.
         *
         * n is the number being converted to an asymptopic form.
         * start is the place where the output should start to curve.
         * rate is the reciprical of the value it should approach minus the start.
         *
         * @param n
         * @param start
         * @param rate
         * @return
         */
        public static float Asymptote(float n, float start, float rate) {
            if(n > start) {
                float output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }


        /**
         * A convenience method, but one probably better coded locally in
         * most situations for efficiency (at least in intended uses).  In
         * some ways this is a reminder, but could be handy in non-performance
         * critical code.
         *
         * n is the number being converted to an asymptopic form.
         * start is the place where the output should start to curve.
         * rate is the reciprical of the value it should approach minus the start.
         *
         * @param n
         * @param start
         * @param rate
         * @return
         */
        public static double Asymptote(double n, double start, double rate) {
            if(n > start) {
                double output = (n - start) / rate;
                output = 1 - (1 / (output + 1));
                output = (output * rate) + start;
                return output;
            }
            return n;
        }

    }

}