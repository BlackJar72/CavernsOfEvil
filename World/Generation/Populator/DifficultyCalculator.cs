using System;

namespace CevarnsOfEvil
{
    public static class DifficultyCalculator
    {
        // Number of levels in campaign mode
        public const int MAX_LEVEL = 32;
        public const float MAX_AS_FLOAT = (float)MAX_LEVEL;


        public static float CalcDifficulty(float level)
        {
            float diff = (float)Math.Sqrt(level / MAX_AS_FLOAT);
            if(diff > 1) diff = 2 - (1 / diff);
            return diff;
        }


        public static float CalcLevel(float difficulty)
        {
            return MAX_AS_FLOAT * difficulty * difficulty;
        }
    }

}