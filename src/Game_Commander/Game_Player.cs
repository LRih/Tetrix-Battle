using System;
using System.Collections.Generic;
using System.Text;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Player
    //#==============================================================
    public class Game_Player : Game_Commander
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int[] _levelScores = new int[Global.NUMBER_OF_LEVELS];
        private List<int> _unlockedUnits = new List<int>();
        //#----------------------------------------------------------
        //# * Battle Setup
        //#----------------------------------------------------------
        public override void BattleSetup(Game_Field field)
        {
            base.BattleSetup(field);
            ReinforcementPts = 10;
            _wall = new Game_Unit(Global.Units[10], true);
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is Player
        //#----------------------------------------------------------
        public override bool IsPlayer()
        {
            return true;
        }
        //#----------------------------------------------------------
        //# * Is Unlocked
        //#----------------------------------------------------------
        public bool IsUnitUnlocked(int unitID)
        {
            return _unlockedUnits.Contains(unitID);
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Unlocked Levels
        //#----------------------------------------------------------
        public int UnlockedLevels()
        {
            int unlocked = 0;

            while (true)
            {
                if (unlocked == _levelScores.GetUpperBound(0)) break;
                if (_levelScores[unlocked] == 0 && _levelScores[unlocked + 1] == 0) break;
                unlocked++;
            }

            return unlocked + 1;
        }
        //#----------------------------------------------------------
        //# * Total Score
        //#----------------------------------------------------------
        public int TotalScore
        {
            get
            {
                int totalScore = 0;
                foreach (int score in _levelScores) totalScore += score;
                return totalScore;
            }
        }
        //#----------------------------------------------------------
        //# * Level Score
        //#----------------------------------------------------------
        public int GetLevelScore(int level)
        {
            return _levelScores[level - 1];
        }
        //#----------------------------------------------------------
        //# * Set Level Score
        //#----------------------------------------------------------
        public void SetLevelScore(int level, int score)
        {
            _levelScores[level - 1] = score;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Unlock Unit
        //#----------------------------------------------------------
        public void UnlockUnit(int unitID)
        {
            _unlockedUnits.Add(unitID);
        }
    }
}
