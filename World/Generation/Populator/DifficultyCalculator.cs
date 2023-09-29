using System;
using kfutils;
using UnityEngine;

namespace CevarnsOfEvil
{
    public static class DifficultyCalculator
    {
        // Number of levels in campaign mode
        public const int MAX_LEVEL = 32;
        public const float MAX_AS_FLOAT = (float)MAX_LEVEL;


        public static float CalcDifficulty(float level)
        {
            return (float)Math.Sqrt(KFMath.Asymptote(level, 16, 16) / MAX_AS_FLOAT);
        }


        public static float CalcLevel(float difficulty)
        {
            return MAX_AS_FLOAT * difficulty * difficulty;
        }
    }

}