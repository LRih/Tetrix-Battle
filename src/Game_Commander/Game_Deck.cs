using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Deck
    //#==============================================================
    public class Game_Deck
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public const int MAX_GROUPS = 6;
        public const int MAX_UNITS = 25;
        private List<Game_Group> _groups = new List<Game_Group>();
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_Deck(bool player)
        {
            // initialize default groups
            for (int i = 0; i < MAX_GROUPS; i++) _groups.Add(new Game_Group(i, player));
        }
        //#----------------------------------------------------------
        //# * Get Groups
        //#----------------------------------------------------------
        public List<Game_Group> Groups
        {
            get { return _groups; }
        }
        //#----------------------------------------------------------
        //# * Number Of Units
        //#----------------------------------------------------------
        public int NumberOfUnits
        {
            get
            {
                int value = 0;
                foreach (Game_Group group in Groups) value += group.NumberOfUnits;
                return value;
            }
        }
        //#----------------------------------------------------------
        //# * Point Value
        //#----------------------------------------------------------
        public int PointValue
        {
            get
            {
                int value = 0;
                foreach (Game_Group group in Groups) value += group.PointValue;
                return value;
            }
        }
        //#----------------------------------------------------------
        //# * Get Bitmaps
        //#----------------------------------------------------------
        public Bitmap[] GetBitmaps()
        {
            Bitmap[] bitmaps = new Bitmap[MAX_GROUPS];
            for (int i = 0; i < bitmaps.Length; i++) bitmaps[i] = Groups[i].GetBitmap();
            return bitmaps;
        }
    }
}
