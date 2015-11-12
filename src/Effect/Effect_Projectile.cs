using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Effect_Projectile
    //#==============================================================
    public class Effect_Projectile : Effect_Missile
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _range;
        private readonly bool _splash;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Effect_Projectile(Game_BattleUnit unit, Point2DEx srcPt, Point2DEx startPt, int range) : base(unit, srcPt, startPt, range * 5)
        {
            _splash = unit.HasSplashSkill();
            _range = Math.Abs(range);
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Get Current Target Pts
        //#----------------------------------------------------------
        public override List<Point2DEx> GetCurrentTargetPts()
        {
            List<Point2DEx> list = new List<Point2DEx>();
            if (_tick == _maxTick)
            {
                Point2DEx targetPt = new Point2DEx(_srcGridPt.X + _range * (_playerSide ? 1 : -1), _srcGridPt.Y);
                list.Add(targetPt);
                if (IsSplash())
                {
                    foreach (Point2DEx adjPt in GetAdjacentPts(targetPt.X, targetPt.Y)) list.Add(adjPt);
                }
            }
            return list;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is Splash
        //#----------------------------------------------------------
        public override bool IsSplash()
        {
            return _splash;
        }
        //#======================================================================================
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
            get
            {
                int distance = Math.Abs(_range * 40);
                int maxHeight = 30;
                // simple quadratic trajectory
                return _startPt.Y + (int)((maxHeight / Math.Pow(distance / 2, 2)) * (Math.Pow(_tick * 8 - distance / 2, 2)) - maxHeight);
            }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Get Adjacent Points
        //#----------------------------------------------------------
        private List<Point2DEx> GetAdjacentPts(int x, int y)
        {
            List<Point2DEx> list = new List<Point2DEx>();
            list.Add(new Point2DEx(x, y - 1));
            list.Add(new Point2DEx(x, y + 1));
            list.Add(new Point2DEx(x - 1, y));
            list.Add(new Point2DEx(x + 1, y));
            return list;
        }
    }
}