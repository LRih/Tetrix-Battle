using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Victory
    //#==============================================================
    public class Scene_Victory : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Constants
        //#----------------------------------------------------------
        private const int WIN_BONUS = 10000;
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _tick = 0; // current frame
        private int _tickMenuEnable = 135; // number of frames until menu is available
        private List<int> _tickPlaySound = new List<int>(new int[] { 60, 90, 120 });
        private readonly int _level;
        private readonly int _unitsLost;
        private readonly int _wallHPPercentage;
        private int _unlockedUnitID = -1; // holds unlocked unit
        private bool _newRecord = false;
        private UI_TextMenu _menu;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Victory(int level, int unitsLost, int wallHPPercentage) : base(new Bitmap("background_victory.png"))
        {
            _level = level;
            _unitsLost = unitsLost;
            _wallHPPercentage = wallHPPercentage;
            InitializeMenu();
            UnlockUnit();
            SetNewScore();
            Saving.SavePlayerData(Global.Player);
        }
        private void InitializeMenu()
        {
            string[] items;
            if (_level == Global.NUMBER_OF_LEVELS) items = new string[] { "Level Select" }; // if last level
            else items = new string[] { "Level Select", "Next Level" };
            _menu = new UI_TextMenu(items, 300, 480, 200, 80, 1, new Bitmap("window_victory.png"));
            _menu.Active = false;
            _menu.Visible = false;
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            UpdateTick();
            _menu.Update();
            base.Update();
        }
        private void UpdateTick()
        {
            _tick++;
            if (_tickPlaySound.Contains(_tick)) Resource.PlaySound("text");
            else if (_tick >= _tickMenuEnable)
            {
                _menu.Active = true;
                _menu.Visible = true;
            }
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            if (_menu.Active)
            {
                if (Controls.AcceptTyped())
                {
                    Resource.PlaySound("decision");
                    this.Dispose();
                    switch (_menu.Index)
                    {
                        case 0: GameMain.Scene = new Scene_LevelSelect(_level - 1); break;
                        case 1: GameMain.Scene = new Scene_Battle(_level + 1); break;
                    }
                }
            }
            _menu.UpdateInput();
            base.UpdateInput();
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            DrawText();
            DrawScore(_tick);
            _menu.UpdateDraw();
            base.UpdateDraw();
        }
        private void DrawText()
        {
            TextEx.DrawText(Global.GetLevelName(_level), Resource.MENU_FONT, 210, 168, 400 - 20);
            TextEx.DrawText("Victory", Resource.MENU_FONT, 210, 168, 400 - 20, FontAlignment.AlignRight);
            if (_level == Global.NUMBER_OF_LEVELS)
            {
                Color color = (_tick % 10 < 5 ? Color.Gold : Color.Orange);
                TextEx.DrawText("Congratulations, you've completed the game!", Resource.MENU_FONT, color, 0, 70, 800, FontAlignment.AlignCenter);
            }
        }
        private void DrawScore(int tick)
        {
            int x = 210;
            int y = 200;
            int width = 400 - 20;
            TextEx.DrawText("Final Score:", Resource.MENU_FONT, Color.Gold, x, 400 + 8, width);
            TextEx.DrawText(GetFinalScore(tick).ToString("#,##0"), Resource.MENU_FONT, x, 400 + 8, width, FontAlignment.AlignRight);
            if (tick < 60) return;
            TextEx.DrawText("Win Bonus:", Resource.MENU_FONT, x, y + 8, width);
            TextEx.DrawText(WIN_BONUS.ToString("#,##0"), Resource.MENU_FONT, x, y + 8, width, FontAlignment.AlignRight);
            if (tick < 90) return;
            TextEx.DrawText("Units Lost: " + _unitsLost.ToString(), Resource.MENU_FONT, x, y + 8 + 24, width);
            TextEx.DrawText("- " + GetUnitsLostPenalty().ToString("#,##0"), Resource.MENU_FONT, x, y + 8 + 24, width, FontAlignment.AlignRight);
            if (tick < 120) return;
            TextEx.DrawText(string.Format("Wall HP: {0} %", _wallHPPercentage.ToString()), Resource.MENU_FONT, x, y + 8 + 48, width);
            TextEx.DrawText("x " + GetWallDamagePenalty().ToString(), Resource.MENU_FONT, x, y + 8 + 48, width, FontAlignment.AlignRight);
            // show new record
            if (_newRecord)
            {
                Color color = (_tick % 10 < 5 ? Color.Gold : Color.Orange);
                TextEx.DrawText("NEW RECORD", Resource.MENU_FONT, color, 610, 400 + 8, 190);
            }
            // draw unlocked unit
            if (IsUnitUnlocked() && _tick >= 150)
                TextEx.DrawText("NEW Unit Unlocked: " + Global.Units[_unlockedUnitID].Name, Resource.MENU_FONT, Color.Gold, x, y + 8 + 72, width);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _menu.Dispose();
            base.Dispose();
        }
        //#----------------------------------------------------------
        //# * Get Final Score
        //#----------------------------------------------------------
        private int GetFinalScore(int tick)
        {
            int score = 0;
            if (tick >= 60) score += WIN_BONUS;
            if (tick >= 90) score -= GetUnitsLostPenalty();
            if (tick >= 120) score = (int)(score * GetWallDamagePenalty());
            return Math.Max(score, 100);
        }
        private int GetUnitsLostPenalty()
        {
            return (int)(Math.Pow(_unitsLost, 2) / 2);
        }
        private float GetWallDamagePenalty()
        {
            return (_wallHPPercentage / 100f);
        }
        //#----------------------------------------------------------
        //# * Unlock Unit
        //#----------------------------------------------------------
        private void UnlockUnit()
        {
            int[] unlockData = new int[] { 0, 3, 5, 6, 0, 7, 1, 4, 8, 9 }; // unit IDs with the respective levels they are unlocked at
            for (int unitID = 0; unitID < 10; unitID++)
            {
                if (unlockData[unitID] == _level && !Global.Player.IsUnitUnlocked(unitID))
                {
                    Global.Player.UnlockUnit(unitID);
                    _unlockedUnitID = unitID;
                    _tickPlaySound.Add(150);
                    _tickMenuEnable += 30;
                }
            }
        }
        private bool IsUnitUnlocked()
        {
            return _unlockedUnitID > -1;
        }
        //#----------------------------------------------------------
        //# * Set New Score
        //#----------------------------------------------------------
        private void SetNewScore()
        {
            int score = GetFinalScore(120);
            if (score > Global.Player.GetLevelScore(_level))
            {
                Global.Player.SetLevelScore(_level, score);
                _newRecord = true;
            }
        }
    }
}