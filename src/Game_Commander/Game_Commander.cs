using System;
using System.Collections.Generic;
using System.Text;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Commander
    //#==============================================================
    public abstract class Game_Commander
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private Game_Deck _deck;
        protected Game_Field _field;
        protected Game_Unit _wall;
        private int _reinforcementPts;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_Commander()
        {
            _deck = new Game_Deck(IsPlayer());
        }
        //#----------------------------------------------------------
        //# * Battle Setup
        //#----------------------------------------------------------
        public virtual void BattleSetup(Game_Field field)
        {
            _field = field;
        }
        //#----------------------------------------------------------
        //# * Process Post Battle
        //#----------------------------------------------------------
        public virtual void ProcessPostBattle()
        {
            _field = null;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Get Deck
        //#----------------------------------------------------------
        public Game_Deck Deck
        {
            get { return _deck; }
        }
        //#----------------------------------------------------------
        //# * Get Wall
        //#----------------------------------------------------------
        public Game_Unit Wall
        {
            get { return _wall; }
        }
        //#----------------------------------------------------------
        //# * Reinforcement Pts
        //#----------------------------------------------------------
        public int ReinforcementPts
        {
            get { return _reinforcementPts; }
            set { _reinforcementPts = Math.Min(Math.Max(value, 0), 255); }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is Player
        //#----------------------------------------------------------
        public abstract bool IsPlayer();
        //#----------------------------------------------------------
        //# * Can Send Group
        //#----------------------------------------------------------
        public virtual bool CanSendGroup(Game_Group group, int row)
        {
            if (group.PointValue > ReinforcementPts) return false; // enough points
            if (!_field.CanAddReinforcements(group, row)) return false; // can place on field
            return true;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Provide Reinforcement Pts
        //#----------------------------------------------------------
        public void ProvideReinforcementPts()
        {
            // reinforcements increase by 6 every update unless wall hp < 75%
            int maxAdd = 6;
            int toAdd = (int)Math.Min((maxAdd * (_wall.HP + _wall.MaxHP * 0.25)) / _wall.MaxHP, maxAdd);
            ReinforcementPts = _reinforcementPts + toAdd;
        }
        //#----------------------------------------------------------
        //# * Send Group
        //#----------------------------------------------------------
        public void SendGroup(Game_Group group, int row)
        {
            _field.AddReinforcements(group, row);
            ReinforcementPts -= group.PointValue;
        }
        //#----------------------------------------------------------
        //# * Get Wall HP Percentage
        //#----------------------------------------------------------
        public int GetWallHPPercentage()
        {
            return (Wall.HP * 100) / Wall.MaxHP;
        }
    }
}
