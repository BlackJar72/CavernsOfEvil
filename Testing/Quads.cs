using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DLD
{
    public struct Quadi
    {
        public Vector3Int ul, ur, ll, lr;

        public Quadi(Vector3Int ul, Vector3Int ur, Vector3Int ll, Vector3Int lr)
        {
            this.ul = ul;
            this.ur = ur;
            this.ll = ll;
            this.lr = lr;
        }
        public override string ToString()
        {
            return "[ll = " + ll + ", ul = " + ul + ", ur = " + ur + ", lr = " + lr + "]";
        }
    }


    public struct Quad
    {
        public Vector3 ul, ur, ll, lr;

        public Quad(Vector3 ul, Vector3 ur, Vector3 ll, Vector3 lr)
        {
            this.ul = ul;
            this.ur = ur;
            this.ll = ll;
            this.lr = lr;
        }
    }


    public struct Coords
    {
        public int x, z;
        public Coords(int x, int z)
        {
            this.x = x;
            this.z = z;
        }
    }
}
