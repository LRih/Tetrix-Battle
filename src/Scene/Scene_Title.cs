using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Title
    //#==============================================================
    public class Scene_Title : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _tickAnimation = 0;
        private UI_TextMenu _menu;
        private UI_Confirm _menuConfirm;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Title(int index = 0) : base(new Bitmap("background_title.png"), "menu")
        {
            int width = 200;
            int x = (Graphics.ScreenWidth() - width) / 2;
            if (Saving.SaveFileExists()) _menu = new UI_TextMenu(new string[] { "New Game", "Continue" }, x, 250, width, 100, 1);
            else _menu = new UI_TextMenu(new string[] { "New Game" }, x, 250, width, 100);
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            _tickAnimation = (_tickAnimation + 1) % 1700;
            _menu.Update();
            if (_menuConfirm != null) _menuConfirm.Update();
            base.Update();
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            if (_menuConfirm != null) UpdateInputMenuConfirm();
            else UpdateInputMenu();
            base.UpdateInput();
        }
        private void UpdateInputMenu()
        {
            _menu.UpdateInput();
            if (Controls.AcceptTyped())
            {
                Resource.PlaySound("decision");
                switch (_menu.Index)
                {
                    case 0: // new game
                        {
                            _menuConfirm = new UI_Confirm(new string[] { "Are you sure you want to start a new", "game? (any saved data will be lost)" });
                            _menu.Active = false;
                            break;
                        }
                    case 1: // continue
                        {
                            this.Dispose();
                            Global.Player = Saving.LoadPlayerData();
                            GameMain.Scene = new Scene_Menu();
                            break;
                        }
                }
            }
        }
        private void UpdateInputMenuConfirm()
        {
            _menuConfirm.UpdateInput();
            if (Controls.AcceptTyped() && _menuConfirm.Index == 0)
            {
                Resource.PlaySound("decision");
                this.Dispose();
                Saving.CreateNewSave();
                Global.Player = Saving.LoadPlayerData();
                GameMain.Scene = new Scene_Menu();
            }
            else if (Controls.CancelTyped() || (Controls.AcceptTyped() && _menuConfirm.Index == 1))
            {
                Resource.PlaySound("cancel");
                _menuConfirm.Dispose();
                _menuConfirm = null;
                _menu.Active = true;
            }
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            DrawAnimation();
            TextEx.DrawText("Richard Liu", Resource.MENU_FONT, 0, 560, 780, FontAlignment.AlignRight);
            _menu.UpdateDraw();
            if (_menuConfirm != null) _menuConfirm.UpdateDraw();
            base.UpdateDraw();
        }
        private void DrawAnimation()
        {
            for (int unitID = 0; unitID < 9; unitID++)
            {
                Images.BitmapNamed("units").DrawCell((_tickAnimation % 30 < 15 ? unitID : unitID + 11), _tickAnimation - (unitID + 1) * 80, 500);
            }
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _menu.Dispose();
            if (_menuConfirm != null) _menuConfirm.Dispose();
            base.Dispose();
        }
    }
}
