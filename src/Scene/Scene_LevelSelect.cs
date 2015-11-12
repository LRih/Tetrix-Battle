using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_LevelSelect
    //#==============================================================
    public class Scene_LevelSelect : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private UI_TextMenu _menuLevelSelect;
        private int _totalScore = Global.Player.TotalScore;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_LevelSelect(int index = 0) : base(new Bitmap("background_level.png"), "menu")
        {
            InitializeMenu(index);
        }
        private void InitializeMenu(int index)
        {
            int numLevels = Global.Player.UnlockedLevels();
            string[] items = new string[numLevels];
            for (int i = 0; i < numLevels; i++) items[i] = Global.GetLevelName(i + 1);
            _menuLevelSelect = new UI_TextMenu(items, 200, 220, 200, 200, index);
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            _menuLevelSelect.Update();
            base.Update();
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            if (Controls.AcceptTyped())
            {
                Resource.PlaySound("decision");
                this.Dispose();
                GameMain.Scene = new Scene_Battle(_menuLevelSelect.Index + 1);
            }
            else if (Controls.CancelTyped())
            {
                Resource.PlaySound("cancel");
                this.Dispose();
                GameMain.Scene = new Scene_Menu();
            }
            _menuLevelSelect.UpdateInput();

            base.UpdateInput();
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            DrawText();
            DrawScoreDisplay();
            _menuLevelSelect.UpdateDraw();

            base.UpdateDraw();
        }
        private void DrawText()
        {
            TextEx.DrawText("Select Level", Resource.MENU_FONT, 200 + 10, 180 + 8, 400);
        }
        private void DrawScoreDisplay()
        {
            TextEx.DrawText("Score:", Resource.MENU_FONT, Color.Gold, 400 + 10, 220 + 8, 180);
            TextEx.DrawText(Global.Player.GetLevelScore(_menuLevelSelect.Index + 1).ToString("#,##0"), Resource.MENU_FONT, 400 + 10, 244 + 8, 180, FontAlignment.AlignRight);
            TextEx.DrawText("Total Score:", Resource.MENU_FONT, Color.Gold, 400 + 10, 320 + 8, 180);
            TextEx.DrawText(_totalScore.ToString("#,##0"), Resource.MENU_FONT, 400 + 10, 344 + 8, 180, FontAlignment.AlignRight);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _menuLevelSelect.Dispose();
            base.Dispose();
        }
    }
}
