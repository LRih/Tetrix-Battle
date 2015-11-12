using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Effect_Missile
    //#==============================================================
    public abstract class Effect_Missile : Effect
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private string _bitmapName;
        protected bool _playerSide;
        protected Point2DEx _srcGridPt;
        protected Point2DEx _startPt;
        public Game_DamageAgent DamageAgent;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Effect_Missile(Game_BattleUnit unit, Point2DEx srcGridPt, Point2DEx startPt, int maxTick) : base(maxTick)
        {
            _bitmapName = unit.MissileBitmapName();
            _playerSide = unit.IsPlayerOwned;
            DamageAgent = unit.GetDamageAgent();
            _srcGridPt = srcGridPt;
            _startPt = startPt;
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            Images.BitmapNamed(_bitmapName).Draw(CurrentX, CurrentY);
        }
        //#----------------------------------------------------------
        //# * Is Legal Attack Target
        //#----------------------------------------------------------
        public bool IsLegalAttackTarget(Game_Unit target)
        {
            if (target == null) return false;
            if (target.IsDead()) return false;
            if (target.IsPlayerOwned == _playerSide) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is Splash
        //#----------------------------------------------------------
        public virtual bool IsSplash()
        {
            return false;
        }
        //#----------------------------------------------------------
        //# * Get Current Target Pts
        //#----------------------------------------------------------
        public abstract List<Point2DEx> GetCurrentTargetPts();
    }
}