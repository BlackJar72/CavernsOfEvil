using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CevarnsOfEvil
{
    public struct SizeData
    {
        public SizeData(int d, int mr, int mrs, int mbrs, int maxn, int minn)
        {
            width = d * 16;
            area = width * width;
            maxRooms = mr;
            maxRoomSize = mrs;
            maxBigSize = mbrs;
            maxNodes = maxn;
            minNodes = minn;
        }
        public readonly int width;
        public readonly int area;
        public readonly int maxRooms;
        public readonly int maxRoomSize;
        public readonly int maxBigSize;
        public readonly int maxNodes;
        public readonly int minNodes;
    }


    [System.Serializable]
    public enum Size
    {
        tiny = 0,
        small = 1,
        medium = 2,
        large = 3,
        xlarge = 4,
        huge = 5
    }


    public static class SizeTable
    {
        public static readonly SizeData tiny = new SizeData(5, 60, 16, 24, 2, 2);
        public static readonly SizeData small = new SizeData(6, 65, 18, 32, 3, 2);
        public static readonly SizeData medium = new SizeData(7, 72, 20, 40, 4, 2);
        public static readonly SizeData large = new SizeData(8, 100, 22, 44, 5, 3);
        public static readonly SizeData xlarge = new SizeData(9, 128, 24, 48, 6, 4);
        public static readonly SizeData huge = new SizeData(10, 170, 24, 64, 8, 6);

        public static readonly SizeData[] Sizes = { tiny, small, medium, large, xlarge, huge };

        public static SizeData GetData(Size size) => Sizes[(int)size];
    }
}
