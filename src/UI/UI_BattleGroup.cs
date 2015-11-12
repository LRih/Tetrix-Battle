using System;
using SwinGame;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_BattleGroup
    //#==============================================================
    public class UI_BattleGroup
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private const int BATTLEFIELD_GRID_HEIGHT = 7;
        private readonly int FIELD_X;
        private readonly int FIELD_Y;
        private Game_Group _group;
        private int _index;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_BattleGroup(Game_Group group, int fieldX, int fieldY)
        {
            _group = group;
            FIELD_X = fieldX;
            FIELD_Y = fieldY;
            Index = MinIndex;
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public void UpdateInput()
        {
            // moving up/down
            int lastIndex = Index;
            if (Controls.DownTyped()) Index++;
            else if (Controls.UpTyped()) Index--;
            if (lastIndex != Index) Resource.PlaySound("cursor");
            // rotate
            if (Controls.SecondaryTyped())
            {
                int oldTopMostUnit = _group.TopMostUnit;
                _group.Rotate();
                Index = _index - (_group.TopMostUnit - oldTopMostUnit);
                Resource.PlaySound("cursor");
            }
            else if (Controls.TertiaryTyped())
            {
                _group.IsVeteran = (_group.IsVeteran != true);
                Resource.PlaySound("cursor");
            }
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public void UpdateDraw()
        {
            int[] rowsWithUnits = _group.RowsWithUnits;
            foreach (int rowWithUnit in rowsWithUnits)
                Images.BitmapNamed("battlegroup_arrow").Draw(FIELD_X + Game_Group.MAX_WIDTH * 40, FIELD_Y + (Index + rowWithUnit) * 40);
            // draw group
            int gridOffsetX = (Game_Group.MAX_WIDTH - 1) - _group.RightMostUnit;
            _group.Draw(FIELD_X + gridOffsetX * 40 - 5, FIELD_Y + Index * 40 - 5);
        }
        //#----------------------------------------------------------
        //# * Group
        //#----------------------------------------------------------
        public Game_Group Group
        {
            get { return _group; }
        }
        //#----------------------------------------------------------
        //# * Index
        //#----------------------------------------------------------
        public int Index
        {
            get { return _index; }
            set { _index = Math.Min(Math.Max(value, MinIndex), MaxIndex); }
        }
        //#----------------------------------------------------------
        //# * Min Index
        //#----------------------------------------------------------
        private int MinIndex
        {
            get { return -_group.TopMostUnit; }
        }
        //#----------------------------------------------------------
        //# * Max Index
        //#----------------------------------------------------------
        private int MaxIndex
        {
            get { return (BATTLEFIELD_GRID_HEIGHT - 1) - _group.BottomMostUnit; }
        }
    }
}
