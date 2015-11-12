using System;
using System.Collections.Generic;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Global
    //#==============================================================
    public static class Global
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public const int NUMBER_OF_LEVELS = 10;
        public const int MAX_GROUP_WIDTH = 5;

        public static bool FPSShown = false;
        public static bool LimitFPS = true;
        public static string CurrentMusic = string.Empty;

        public static Dictionary<int, Data_Unit> Units = new Dictionary<int, Data_Unit>();

        public static Game_Player Player;
        public static Game_Enemy Enemy = new Game_Enemy();
        //#----------------------------------------------------------
        //# * Get Level Name
        //#----------------------------------------------------------
        public static string GetLevelName(int level)
        {
            if (level == NUMBER_OF_LEVELS) return "Final Level";
            else return string.Format("Level {0}", level);
        }
    }
}
