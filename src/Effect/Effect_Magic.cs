using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Effect_Magic
    //#==============================================================
    public class Effect_Magic : Effect_Missile
    {
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Effect_Magic(Game_BattleUnit unit, Point2DEx srcPt, Point2DEx startPt) : base(unit, srcPt, startPt, 10)
        {
        }
        //#----------------------------------------------------------
        //# * Get Current Target Pts
        //#----------------------------------------------------------
        public override List<Point2DEx> GetCurrentTargetPts()
        {
            List<Point2DEx> list = new List<Point2DEx>();
            list.Add(new Point2DEx(_srcGridPt.X + (_tick * 8 * (_playerSide ? 1 : -1)) / 40, _srcGridPt.Y));
            return list;
        }
        //#----------------------------------------------------------
        //# * Current X
        //#----------------------------------------------------------
        protected override int CurrentX
        {
            get { return _startPt.X + _tick * 8 * (_playerSide ? 1 : -1); }
        }
        //#----------------------------------------------------------
        //# * Current Y
        //#----------------------------------------------------------
        protected override int CurrentY
        {
            get { return _startPt.Y; }
        }
    }
}