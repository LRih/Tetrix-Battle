using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Menu
    //#==============================================================
    public class Scene_Menu : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private UI_TextMenu _menu;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Menu(int index = 0) : base(new Bitmap("background_menu.png"), "menu")
        {
            _menu = new UI_TextMenu(new string[] { "Select Level", "Edit Deck" }, 90, 270, 200, 100, index);
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            _menu.Update();

            base.Update();
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            _menu.UpdateInput();
            // menu input
            if (Controls.AcceptTyped())
            {
                Resource.PlaySound("decision");
                this.Dispose();
                switch (_menu.Index)
                {
                    case 0: GameMain.Scene = new Scene_LevelSelect(); break;
                    case 1: GameMain.Scene = new Scene_Deck(); break;
                }
            }
            else if (Controls.CancelTyped())
            {
                Resource.PlaySound("cancel");
                this.Dispose();
                GameMain.Scene = new Scene_Title();
            }

            base.UpdateInput();
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            TextEx.DrawText("Menu", Resource.MENU_FONT, 90 + 10, 230 + 8, 200);
            TextEx.DrawText("Controls", Resource.MENU_FONT, 310 + 10, 170 + 8, 200);
            DrawControls();
            _menu.UpdateDraw();

            base.UpdateDraw();
        }
        private void DrawControls()
        {
            string[] controls1 = new string[] { "Arrow Keys:", "Z/Enter/Space:", "Escape:", "X/Shift:", "C:", "F10:", "F11:", "F12:" };
            string[] controls2 = new string[] { "Navigation", "Accept", "Cancel", "Rotate Group", "Toggle Veteran / Delete", "Toggle FPS", "Toggle Fullscreen", "Limit FPS" };
            int x = 310 + 10;
            int x2 = x + 150;
            int y = 218;
            for (int i = 0; i < controls1.Length; i++)
            {
                TextEx.DrawText(controls1[i], Resource.MENU_FONT, x, y + 24 * i + 8, 200);
                TextEx.DrawText(controls2[i], Resource.MENU_FONT, x2, y + 24 * i + 8, 300);
            }
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _menu.Dispose();
            base.Dispose();
        }
    }
}
