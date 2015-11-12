using System;
using SwinGame;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_Confirm
    //#==============================================================
    public class UI_Confirm : UI_TextMenu
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private string[] _message = new string[2];
        private int _messageX;
        private int _messageY;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_Confirm(string[] message, int x = 200, int y = 220) : base(new string[] { "Yes", "No" }, x + 125, y + 80, 150, 80, 1)
        {
            _message = message;
            _messageX = x;
            _messageY = y;
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            if (Visible)
            {
                Images.BitmapNamed("window_confirm").Draw(_messageX, _messageY);
                TextEx.DrawText(_message[0], Resource.MENU_FONT, _messageX + 10, _messageY + 12, 400);
                TextEx.DrawText(_message[1], Resource.MENU_FONT, _messageX + 10, _messageY + 30 + 12, 400);
            }
            base.UpdateDraw();
        }
    }
}
