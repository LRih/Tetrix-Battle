using System;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_DamageAgent
    //#==============================================================
    public struct Game_DamageAgent
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public readonly bool IsPlayer;
        public readonly Data_UnitClass UnitClass;
        public readonly int Damage;
        public readonly int ExtraDamage;
        public readonly string BitmapHit;
        public readonly string SoundHit;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_DamageAgent(Game_BattleUnit unit)
        {
            IsPlayer = unit.IsPlayerOwned;
            UnitClass = unit.Class;
            Damage = unit.Attack;
            ExtraDamage = unit.ExtraDamage;
            BitmapHit = unit.MissileHitBitmapName();
            SoundHit = unit.SoundHit;
        }
        //#----------------------------------------------------------
        //# * Is Legal Attack Target
        //#----------------------------------------------------------
        public bool IsLegalAttackTarget(Game_Unit target)
        {
            if (target == null) return false;
            if (IsPlayer == target.IsPlayerOwned) return false;
            if (target.IsInMotion()) return false;
            if (target.IsDead()) return false;
            return true;
        }
    }
}
