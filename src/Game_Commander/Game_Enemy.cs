using System;
using System.IO;
using System.Collections.Generic;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Enemy
    //#==============================================================
    public class Game_Enemy : Game_Commander
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _currentScript = 0;
        private int _tick;
        private List<int> _groupSendScript = new List<int>();
        private List<int> _rotationScript = new List<int>();
        private List<int> _rowScript = new List<int>();
        private List<int> _tickSendScript = new List<int>();
        private List<int> _considerVeteranScript = new List<int>();
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public void Update()
        {
            Game_Group group = Deck.Groups[CurrentGroupID].Clone();
            for (int i = 0; i < CurrentGroupRotation; i++) group.Rotate();
            if (CanSendGroup(group, CurrentGroupRow)) // if ready to send next wave
            {
                if (_considerVeteranScript[_currentScript] == 1 && ReinforcementPts >= group.PointValue * 8) group.IsVeteran = true;
                SendGroup(group, CurrentGroupRow);
                _currentScript = (_currentScript + 1) % _groupSendScript.Count;
                _tick = 0;
            }
            else _tick++;
        }
        //#----------------------------------------------------------
        //# * Battle Setup
        //#----------------------------------------------------------
        public override void BattleSetup(Game_Field field)
        {
            base.BattleSetup(field);
            ReinforcementPts = 10;
            _wall = new Game_Unit(Global.Units[10], false);
            _currentScript = 0;
            _tick = 0;
        }
        //#======================================================================================
        private int CurrentGroupID
        {
            get { return _groupSendScript[_currentScript]; }
        }
        private int CurrentGroupRotation
        {
            get { return _rotationScript[_currentScript]; }
        }
        private int CurrentGroupRow
        {
            get { return _rowScript[_currentScript]; }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is Player
        //#----------------------------------------------------------
        public override bool IsPlayer()
        {
            return false;
        }
        //#----------------------------------------------------------
        //# * Can Send Group
        //#----------------------------------------------------------
        public override bool CanSendGroup(Game_Group group, int row)
        {
            if (_tick < _tickSendScript[_currentScript]) return false; // if not time yet
            if (!base.CanSendGroup(group, row)) return false;
            return true;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Set Level
        //#----------------------------------------------------------
        public void SetLevel(int level)
        {
            // load level data
            int lineStart = (level - 1) * 6; 
            string[] rawSave = File.ReadAllText(Saving.LEVELS_DATA_PATH).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            List<string> filteredSave = new List<string>();
            // remove commented lines
            foreach (string line in rawSave) if (!line.StartsWith("#")) filteredSave.Add(line);
            // load groups
            Saving.LoadIntoDeck(Deck, filteredSave[lineStart]);
            // load group send script
            _groupSendScript = ParseScript(filteredSave[lineStart + 1]);
            _rotationScript = ParseScript(filteredSave[lineStart + 2]);
            _rowScript = ParseScript(filteredSave[lineStart + 3]);
            _tickSendScript = ParseScript(filteredSave[lineStart + 4]);
            _considerVeteranScript = ParseScript(filteredSave[lineStart + 5]);
        }
        //#----------------------------------------------------------
        //# * Parse Script
        //#     from 0,1,1,1...
        //#----------------------------------------------------------
        private List<int> ParseScript(string line)
        {
            List<int> list = new List<int>();
            string[] script = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string value in script) list.Add(Convert.ToInt32(value));
            return list;
        }
    }
}
