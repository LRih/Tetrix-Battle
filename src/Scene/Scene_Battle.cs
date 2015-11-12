using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Battle
    //#==============================================================
    public class Scene_Battle : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Constants
        //#----------------------------------------------------------
        private const int FIELD_X = -80;
        private const int FIELD_Y = 60;
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private readonly int _level;
        private Game_Player _player = Global.Player;
        private Game_Enemy _enemy = Global.Enemy;
        private Game_Field _field = new Game_Field(FIELD_X, FIELD_Y);
        private int _flashReinforcements = 0;
        private int _tickReinforcements = 0;
        private int _tickPostBattle = 0;
        private UI_TextMenu _menuMain;
        private UI_Confirm _menuConfirm;
        private UI_IconMenu _menuGroups;
        private UI_BattleGroup _menuBattleGroup;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Battle(int level) : base(new Bitmap("background_battlefield.png"), "battle")
        {
            _level = level;
            _enemy.SetLevel(_level);
            _player.BattleSetup(_field);
            _enemy.BattleSetup(_field);
            InitializeMenuGroups();
        }
        private void InitializeMenuGroups()
        {
            Bitmap[] bitmaps = _player.Deck.GetBitmaps();
            _menuGroups = new UI_IconMenu(bitmaps, 280, 360, 800 - 280, 600 - 360, false);
            foreach (Bitmap bitmap in bitmaps) bitmap.Dispose();
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            if (!IsBattleOver())
            {
                _menuGroups.Update();
                if (_menuConfirm != null)
                {
                    _menuConfirm.Update();
                    _menuMain.Update();
                }
                else if (_menuMain != null) _menuMain.Update();
                else
                {
                    _field.Update();
                    UpdateReinforcements();
                    UpdateEnemy();
                }
            }
            else UpdatePostBattle();
            if (_flashReinforcements > 0) _flashReinforcements--;
            base.Update();
        }
        private void UpdateReinforcements()
        {
            if (_tickReinforcements == 299)
            {
                _player.ProvideReinforcementPts();
                _enemy.ProvideReinforcementPts();
            }
            _tickReinforcements = (_tickReinforcements + 1) % 300;
        }
        private void UpdateEnemy()
        {
            _enemy.Update();
        }
        private void UpdatePostBattle()
        {
            _menuGroups.Active = false;
            _menuBattleGroup = null;
            if (_tickPostBattle < 30) _field.Update();
            if (_tickPostBattle == 30)
            {
                if (_field.HasPlayerWon())
                    Resource.PlayMusic("victory", true);
                else if (_field.HasEnemyWon())
                    Resource.PlayMusic("defeat", true);
            }
            _tickPostBattle++;
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            if (_menuGroups.Active) UpdateInputGroupSelect();
            else if (_menuConfirm != null) UpdateInputMenuConfirm();
            else if (_menuMain != null) UpdateInputMenuMain();
            else if (_menuBattleGroup != null) UpdateInputGroupPlacement();
            else if (IsBattleOver()) UpdateInputBattleOver();
            base.UpdateInput();
        }
        private void UpdateInputGroupSelect()
        {
            if (Controls.AcceptTyped())
            {
                Game_Group group = _player.Deck.Groups[_menuGroups.Index].Clone();
                if (group.IsEmpty()) Resource.PlaySound("buzzer");
                else
                {
                    _menuBattleGroup = new UI_BattleGroup(group, FIELD_X, FIELD_Y);
                    _menuGroups.Active = false;
                    Resource.PlaySound("decision");
                }
            }
            else if (Controls.CancelTyped())
            {
                _menuMain = new UI_TextMenu(new string[] { "Return to Game", "Return to Level Select" }, 280, 250, 240, 100, 0, new Bitmap("window_battlemenu.png"));
                _menuGroups.Active = false;
                Resource.PlaySound("decision");
            }
            _menuGroups.UpdateInput();
        }
        private void UpdateInputMenuMain()
        {
            _menuMain.UpdateInput();
            if (Controls.AcceptTyped() && _menuMain.Index == 1)
            {
                Resource.PlaySound("decision");
                _menuConfirm = new UI_Confirm(new string[] { "Are you sure you want to leave the", "battle?" });
                _menuMain.Active = false;
            }
            else if (Controls.CancelTyped() || (Controls.AcceptTyped() && _menuMain.Index == 0))
            {
                Resource.PlaySound("cancel");
                _menuMain.Dispose();
                _menuMain = null;
                _menuGroups.Active = true;
            }
        }
        private void UpdateInputMenuConfirm()
        {
            _menuConfirm.UpdateInput();
            if (Controls.AcceptTyped() && _menuConfirm.Index == 0)
            {
                Resource.PlaySound("decision");
                this.Dispose();
                GameMain.Scene = new Scene_LevelSelect(_level - 1);
            }
            else if (Controls.CancelTyped() || (Controls.AcceptTyped() && _menuConfirm.Index == 1))
            {
                Resource.PlaySound("cancel");
                _menuConfirm.Dispose();
                _menuConfirm = null;
                _menuMain.Active = true;
            }
        }
        private void UpdateInputGroupPlacement()
        {
            _menuBattleGroup.UpdateInput();
            if (Controls.AcceptTyped())
            {
                if (_player.CanSendGroup(_menuBattleGroup.Group, _menuBattleGroup.Index))
                {
                    _player.SendGroup(_menuBattleGroup.Group, _menuBattleGroup.Index);
                    _menuBattleGroup = null;
                    _menuGroups.Active = true;
                    Resource.PlaySound("decision");
                }
                else
                {
                    _flashReinforcements = 30;
                    Resource.PlaySound("buzzer");
                }
            }
            else if (Controls.CancelTyped())
            {
                _menuBattleGroup = null;
                _menuGroups.Active = true;
                Resource.PlaySound("cancel");
            }
        }
        private void UpdateInputBattleOver()
        {
            if (Controls.AcceptTyped())
            {
                if (_tickPostBattle > 120 && _field.HasPlayerWon())
                {
                    Resource.PlaySound("decision");
                    this.Dispose();
                    GameMain.Scene = new Scene_Victory(_level, _field.UnitsLost, _player.GetWallHPPercentage());
                }
                else if (_tickPostBattle > 200 && _field.HasEnemyWon())
                {
                    Resource.PlaySound("decision");
                    this.Dispose();
                    GameMain.Scene = new Scene_LevelSelect(_level - 1);
                }
            }
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            DrawWallHP();
            DrawText();
            DrawReinforcements();
            _field.UpdateDraw();
            _menuGroups.UpdateDraw();
            DrawBattlegroup();
            if (_menuMain != null) _menuMain.UpdateDraw();
            if (_menuConfirm != null) _menuConfirm.UpdateDraw();
            DrawPostBattle();
            base.UpdateDraw();
        }
        private void DrawWallHP()
        {
            int edgeOffset = 10;
            int width = 320 - edgeOffset * 2;
            _player.Wall.DrawHPBar(edgeOffset, 10, width, 20, true);
            _enemy.Wall.DrawHPBar(480 + edgeOffset, 10, width, 20);
            TextEx.DrawText("HP", Resource.MENU_FONT, edgeOffset, 8, width - 5, FontAlignment.AlignRight);
            TextEx.DrawText("HP", Resource.MENU_FONT, 480 + 15, 8, width - 5);
            TextEx.DrawText(_player.GetWallHPPercentage() + "%", Resource.MENU_FONT, edgeOffset, 8, width, FontAlignment.AlignCenter);
            TextEx.DrawText(_enemy.GetWallHPPercentage() + "%", Resource.MENU_FONT, 480 + edgeOffset, 8, width, FontAlignment.AlignCenter);
            
        }
        private void DrawText()
        {
            TextEx.DrawText(Global.GetLevelName(_level), Resource.MENU_FONT, 320 + 10, 8, 160 - 20, FontAlignment.AlignCenter);
            TextEx.DrawText("Select Group", Resource.MENU_FONT, 10, 360 + 8, 280 - 20, FontAlignment.AlignRight);
            if (_menuBattleGroup != null)
            {
                TextEx.DrawText("X: Rotate", Resource.MENU_FONT, 10, 530 + 8, 280 - 20);
                TextEx.DrawText("C: Toggle Veteran", Resource.MENU_FONT, 10, 530 + 24 + 8, 280 - 20);
            }
        }
        private void DrawReinforcements()
        {
            int width = 280 - 20;
            Color color = (_flashReinforcements % 10 > 4 ? Color.Orange : Color.White);
            Game_Group group;
            if (_menuBattleGroup == null) group = _player.Deck.Groups[_menuGroups.Index];
            else group = _menuBattleGroup.Group;
            TextEx.DrawText("Reinforcement Pts:", Resource.MENU_FONT, Color.Gold, 10, 400 + 8, width);
            TextEx.DrawText(_player.ReinforcementPts.ToString(), Resource.MENU_FONT, color, 10, 460 + 8, width, FontAlignment.AlignCenter);
            TextEx.DrawText("- " + group.PointValue.ToString(), Resource.MENU_FONT, Color.Red, 170, 460 + 8, width);
        }
        private void DrawBattlegroup()
        {
            if (_menuBattleGroup != null) _menuBattleGroup.UpdateDraw();
        }
        private void DrawPostBattle()
        {
            if (_field.HasPlayerWon()) Images.BitmapNamed("text_victory").Draw(Math.Min(-1100 + _tickPostBattle * 10, 0), 0);
            else if (_field.HasEnemyWon()) Images.BitmapNamed("text_defeat").Draw(0, Math.Min(-410 + _tickPostBattle * 2, 0));
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _player.ProcessPostBattle();
            _enemy.ProcessPostBattle();
            if (_menuMain != null) _menuMain.Dispose();
            if (_menuConfirm != null) _menuConfirm.Dispose();
            _menuGroups.Dispose();

            base.Dispose();
        }
        //#----------------------------------------------------------
        //# * Is Battle Over
        //#----------------------------------------------------------
        private bool IsBattleOver()
        {
            return (_field.HasPlayerWon() || _field.HasEnemyWon());
        }
    }
}
