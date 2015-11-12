using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_UnitAction
    //#==============================================================
    public enum Game_UnitAction
    {
        Null = 0,
        Move = 1,
        Jump = 2,
        Attack = 3,
        Projectile = 4,
        Magic = 5
    }
    //#==============================================================
    //# * Game_BattleUnit
    //#==============================================================
    public class Game_BattleUnit : Game_Unit
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public bool[] Connections = new bool[4] { false, false, false, false };
        protected Color _color = Color.Magenta;
        private int _tickHealing = 30;
        private int _tickAction = 60; // reduces by 1 each frame, when 0 the unit is ready to act
        private int _tickDespawn = 90; // reduces by 1 each frame when unit is dead, when it reaches 0, unit disappears
        private int _aniBitmap = 60;
        private int _aniAction = -1;
        public Game_UnitAction _currentAction = Game_UnitAction.Null;
        public int AuraMoraleBoost = 0;
        public int AuraSpot = 0;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_BattleUnit(Data_Unit unit, Game_Group group, bool playerOwned, bool isVeteran = false) : base(unit, playerOwned, isVeteran)
        {
            _color = group.Color;
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            if (!IsInAction()) _tickAction--;
            _tickHealing = (_tickHealing + 1) % 30;
            if (_aniAction == 0) _currentAction = Game_UnitAction.Null;
            if (_aniAction > -1) _aniAction--;
            if (IsDead()) _tickDespawn--;
            _aniBitmap = (_aniBitmap + 1) % 60;
            base.Update();
        }
        //#----------------------------------------------------------
        //# * Clear Turn Flags
        //#     morale boost from kanbi etc.
        //#----------------------------------------------------------
        public void ClearTurnFlags()
        {
            AuraMoraleBoost = 0;
            AuraSpot = 0;
        }
        //#======================================================================================
        public Color Color
        {
            get
            {
                if (IsDead()) return Color.Gray;
                return _color;
            }
        }
        public Color BorderColor
        {
            get { return (IsPlayerOwned ? Color.Blue : Color.Red); }
        }
        public override int BitmapCell
        {
            get
            {
                if (IsDead()) return Unit.BitmapCell + 66;
                if ((_currentAction == Game_UnitAction.Projectile || _currentAction == Game_UnitAction.Magic))
                {
                    if (_aniAction > 10) return Unit.BitmapCell + 44;
                    if (_aniAction > -1) return Unit.BitmapCell + 55;
                }
                if (_currentAction == Game_UnitAction.Attack)
                {
                    if (_aniAction > 10) return Unit.BitmapCell + 22;
                    if (_aniAction > -1) return Unit.BitmapCell + 33;
                }
                if (_aniBitmap < 30) return Unit.BitmapCell + 11;
                return Unit.BitmapCell;
            }
        }
        public string SoundLaunch
        {
            get
            {
                if (HasProjectileSkill() && _currentAction != Game_UnitAction.Projectile) return "null";
                else return Unit.SoundLaunch;
            }
        }
        public string SoundHit
        {
            get
            {
                if (HasProjectileSkill() && _currentAction != Game_UnitAction.Projectile) return "hit";
                else return Unit.SoundHit;
            }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Move Vector
        //#----------------------------------------------------------
        public Point2DEx MoveVector()
        {
            int x = (IsPlayerOwned ? 1 : -1);
            return new Point2DEx(x, 0);
        }
        //#----------------------------------------------------------
        //# * Jump Vector
        //#----------------------------------------------------------
        public Point2DEx JumpVector()
        {
            int x = (IsPlayerOwned ? 2 : -2);
            return new Point2DEx(x, 0);
        }
        //#----------------------------------------------------------
        //# * Attack Vector
        //#----------------------------------------------------------
        public Point2DEx AttackVector()
        {
            int x = (IsPlayerOwned ? 1 : -1);
            return new Point2DEx(x, 0);
        }
        //#----------------------------------------------------------
        //# * Missile Vector
        //#----------------------------------------------------------
        public List<Point2DEx> MissileVectors()
        {
            List<Point2DEx> pts = new List<Point2DEx>();
            int xMult = (IsPlayerOwned ? 1 : -1);
            for (int range = Unit.ProjectileSkill; range <= ProjectileRange(); range++) pts.Add(new Point2DEx(range * xMult, 0));
            return pts;
        }
        //#----------------------------------------------------------
        //# * Magic Vector
        //#----------------------------------------------------------
        public List<Point2DEx> MagicVectors()
        {
            List<Point2DEx> pts = new List<Point2DEx>();
            int xMult = (IsPlayerOwned ? 1 : -1);
            pts.Add(new Point2DEx(1 * xMult, 0));
            pts.Add(new Point2DEx(2 * xMult, 0));
            return pts;
        }
        //#======================================================================================
        public bool UpConnected
        {
            get { return Connections[0]; }
        }
        public bool DownConnected
        {
            get { return Connections[1]; }
        }
        public bool LeftConnected
        {
            get { return Connections[2]; }
        }
        public bool RightConnected
        {
            get { return Connections[3]; }
        }
        //#======================================================================================
        public bool HasBlockSkill()
        {
            return Unit.BlockSkill;
        }
        public int MoraleBoostAmount()
        {
            if (!Unit.MoraleBoostSkill) return 0;
            if (_isVeteran) return 2;
            else return 1;
        }
        public bool HasJumpSkill()
        {
            return Unit.JumpSkill;
        }
        public Point2DEx MovePosition()
        {
            Point2DEx pos = new Point2DEx();
            if (_currentAction == Game_UnitAction.Move) pos.X += _aniAction * 5;
            else if (_currentAction == Game_UnitAction.Jump) pos.X += _aniAction * 10;
            pos.X *= (IsPlayerOwned ? -1 : 1);
            return pos;
        }
        public bool HasProjectileSkill()
        {
            return Unit.ProjectileSkill > 0;
        }
        public int ProjectileRange()
        {
            return (Unit.ProjectileSkill + AuraSpot);
        }
        public bool HasSplashSkill()
        {
            return Unit.SplashSkill;
        }
        public int SpotAmount()
        {
            if (!Unit.SpotSkill) return 0;
            if (_isVeteran) return 2;
            else return 1;
        }
        public bool HasMagicSkill()
        {
            return Unit.MagicSkill;
        }
        public int HealAmount()
        {
            if (!Unit.HealSkill) return 0;
            if (_isVeteran) return 6;
            else return 3;
        }
        public bool IsReadyToCaseHeal()
        {
            return (_tickHealing == 0);
        }
        public string MissileBitmapName()
        {
            return Unit.MissileBitmapName;
        }
        public string MissileHitBitmapName()
        {
            return Unit.MissileHitBitmapName;
        }
        //#======================================================================================
        public int Attack
        {
            get
            {
                int atk = Unit.Attack;
                if (HasProjectileSkill() && _currentAction != Game_UnitAction.Projectile) atk /= 2;
                if (_isVeteran) atk *= 2;
                atk += Morale * 3;
                return atk;
            }
        }
        public int ExtraDamage
        {
            get
            {
                int extraDamage = Unit.ExtraDamage;
                if (_isVeteran) extraDamage *= 2;
                return extraDamage;
            }
        }
        public int Morale
        {
            get
            {
                int morale = AuraMoraleBoost;
                foreach (bool connection in Connections) if (connection) morale++;
                return morale;
            }
        }
        //#======================================================================================
        public bool IsInAction()
        {
            return (_aniAction > -1);
        }
        public override bool IsInMotion()
        {
            return (_currentAction == Game_UnitAction.Move || _currentAction == Game_UnitAction.Jump);
        }
        public bool IsReadyToStartAct(Game_UnitAction action)
        {
            if (IsDead() || IsInAction()) return false;
            if (action == Game_UnitAction.Jump)
            {
                foreach (bool connection in Connections) if (connection) return false; // can only jump when solo unit
            }
            return (_tickAction <= 0);
        }
        public bool IsReadyToAct(Game_UnitAction action)
        {
            if (action != _currentAction) return false;
            if (IsDead()) return false;
            return (_aniAction == 10);
        }
        public bool IsReadyToDespawn()
        {
            return (_tickDespawn <= 0);
        }
        //#======================================================================================
        public Game_DamageAgent GetDamageAgent()
        {
            return new Game_DamageAgent(this);
        }
        //#----------------------------------------------------------
        //# * Start Act
        //#----------------------------------------------------------
        public void StartAct(Game_UnitAction action)
        {
            _tickAction = 60;
            _currentAction = action;
            if (action == Game_UnitAction.Move) _aniAction = 8;
            else if (action == Game_UnitAction.Jump) _aniAction = 8;
            else _aniAction = 20;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Draw
        //#     used by Game_Field to draw units on the field
        //#----------------------------------------------------------
        public override void Draw(int x, int y)
        {
            // draw base
            Graphics.FillRectangle(Color, x, y, 40, 40);
            // draw border
            if (!UpConnected) Graphics.DrawLine(BorderColor, x, y, x + 39, y);
            if (!DownConnected) Graphics.DrawLine(BorderColor, x, y + 39, x + 39, y + 39);
            if (!LeftConnected) Graphics.DrawLine(BorderColor, x, y, x, y + 39);
            if (!RightConnected) Graphics.DrawLine(BorderColor, x + 39, y, x + 39, y + 39);
            // draw unit
            Images.BitmapNamed(IsPlayerOwned ? "units" : "units2").DrawCell(BitmapCell, x, y - 8);
            // draw glow
            if (_isVeteran) Images.BitmapNamed("veteran_glow").DrawCell(_aniBitmap / 15, x, y);
            // draw hp bar
            if (HP < MaxHP) DrawHPBar(x, y);
        }
    }
}
