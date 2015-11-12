using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Data_Unit
    //#==============================================================
    public struct Data_Unit
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public readonly string Name;
        public readonly int BitmapCell;
        public readonly Data_UnitClass Class;
        public readonly int MaxHP;
        public readonly int Attack;
        public readonly int PointValue;
        public readonly bool BlockSkill;
        public readonly bool MoraleBoostSkill;
        public readonly bool JumpSkill;
        public readonly int ProjectileSkill;
        public readonly bool SplashSkill;
        public readonly bool SpotSkill;
        public readonly bool MagicSkill;
        public readonly bool HealSkill;
        public readonly string MissileBitmapName;
        public readonly string MissileHitBitmapName;
        public readonly Data_UnitClass Weakness;
        public readonly int ExtraDamage;
        public readonly string SoundLaunch;
        public readonly string SoundHit;
        public readonly string[] Information;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Data_Unit(string rawData)
        {
            string[] data = rawData.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            Name = data[0];
            BitmapCell = Convert.ToInt32(data[1]);
            Class = (Data_UnitClass)Convert.ToInt32(data[2]);
            MaxHP = Convert.ToInt32(data[3]);
            Attack = Convert.ToInt32(data[4]);
            PointValue = Convert.ToInt32(data[5]);
            BlockSkill = (Convert.ToInt32(data[6]) == 1);
            MoraleBoostSkill = (Convert.ToInt32(data[7]) == 1);
            JumpSkill = (Convert.ToInt32(data[8]) == 1);
            ProjectileSkill = Convert.ToInt32(data[9]);
            SplashSkill = (Convert.ToInt32(data[10]) == 1);
            SpotSkill = (Convert.ToInt32(data[11]) == 1);
            MagicSkill = (Convert.ToInt32(data[12]) == 1);
            HealSkill = (Convert.ToInt32(data[13]) == 1);
            MissileBitmapName = data[14];
            MissileHitBitmapName = data[15];
            Weakness = (Data_UnitClass)Convert.ToInt32(data[16]);
            ExtraDamage = Convert.ToInt32(data[17]);
            SoundLaunch = data[18];
            SoundHit = data[19];
            Information = data[20].Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < Information.Length; i++)  Information[i] = Information[i].Replace('_', ' ');
        }
    }
    //#==============================================================
    //# * Data_UnitClass
    //#==============================================================
    public enum Data_UnitClass
    {
        Generic = 0,
        Sword = 1,
        Spear = 2,
        Mount = 3,
        Bow = 4,
        Magic = 5,
        Siege = 6,
        Structure = 7
    }
}
