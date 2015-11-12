using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Unit
    //#==============================================================
    public class Game_Unit
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public readonly Data_Unit Unit;
        public readonly bool IsPlayerOwned;
        protected bool _isVeteran;
        protected int _hp;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_Unit(Data_Unit unit, bool playerOwned = true, bool isVeteran = false)
        {
            Unit = unit;
            _isVeteran = isVeteran;
            IsPlayerOwned = playerOwned;
            _hp = MaxHP;
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public virtual void Update()
        {
        }
        //#----------------------------------------------------------
        //# * Name
        //#----------------------------------------------------------
        public string Name
        {
            get { return Unit.Name; }
        }
        //#----------------------------------------------------------
        //# * Bitmap Cell
        //#----------------------------------------------------------
        public virtual int BitmapCell
        {
            get
            {
                if (IsDead()) return Unit.BitmapCell + 66;
                return Unit.BitmapCell;
            }
        }
        //#----------------------------------------------------------
        //# * Class
        //#----------------------------------------------------------
        public Data_UnitClass Class
        {
            get { return Unit.Class; }
        }
        //#----------------------------------------------------------
        //# * Weakness
        //#----------------------------------------------------------
        public Data_UnitClass Weakness
        {
            get { return Unit.Weakness; }
        }
        //#----------------------------------------------------------
        //# * MaxHP
        //#----------------------------------------------------------
        public int MaxHP
        {
            get
            {
                int maxHP = Unit.MaxHP;
                if (_isVeteran) maxHP *= 2;
                return maxHP;
            }
        }
        //#----------------------------------------------------------
        //# * HP
        //#----------------------------------------------------------
        public int HP
        {
            get { return _hp; }
            set { _hp = Math.Min(Math.Max(value, 0), MaxHP); }
        }
        //#----------------------------------------------------------
        //# * Is Dead?
        //#----------------------------------------------------------
        public bool IsDead()
        {
            return (HP == 0);
        }
        //#----------------------------------------------------------
        //# * Is Currently Moving
        //#----------------------------------------------------------
        public virtual bool IsInMotion()
        {
            return false;
        }
        //#----------------------------------------------------------
        //# * Is Ally
        //#----------------------------------------------------------
        public bool IsAlly(Game_Unit unit)
        {
            return (this.IsPlayerOwned == unit.IsPlayerOwned);
        }
        //#----------------------------------------------------------
        //# * Is Legal Attack Target
        //#----------------------------------------------------------
        public bool IsLegalAttackTarget(Game_Unit target)
        {
            if (target == null) return false;
            if (target.IsAlly(this)) return false;
            if (target.IsInMotion()) return false;
            if (target.IsDead()) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Execute Damage
        //#----------------------------------------------------------
        public int ExecuteDamage(Game_DamageAgent damageAgent)
        {
            int damage = damageAgent.Damage;
            if (Weakness == damageAgent.UnitClass) damage += damageAgent.ExtraDamage;
            this.HP -= damage;
            return damage;
        }
        //#----------------------------------------------------------
        //# * Draw
        //#     used by Game_Field to draw units on the field
        //#----------------------------------------------------------
        public virtual void Draw(int x, int y)
        {
            Images.BitmapNamed("units").DrawCell(BitmapCell, x, y);
        }
        //#----------------------------------------------------------
        //# * Draw HP Bar
        //#----------------------------------------------------------
        public void DrawHPBar(int x, int y)
        {
            DrawHPBar(x + 2, y + 34, 40 - 4, 4);
        }
        public void DrawHPBar(int x, int y, int width, int height, bool reverse = false)
        {
            float hpPercent = HP / (float)MaxHP;
            int hpWidth = (int)(width * hpPercent);
            Graphics.FillRectangle(Color.Red, x, y, width, height);
            if (HP > 0) Graphics.FillRectangle(Color.Lime, x + (reverse ? width - hpWidth : 0), y, hpWidth, height);
            Graphics.DrawRectangle(Color.Black, x, y, width, height);
        }
    }
}
