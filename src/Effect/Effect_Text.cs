using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Effect_Text
    //#==============================================================
    public class Effect_Text : Effect
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private string _text;
        private Color _color;
        private Point2DEx _startPt;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Effect_Text(string text, Color color, int x, int y, int maxTick) : base (maxTick)
        {
            _text = text;
            _color = color;
            _startPt = new Point2DEx(x, y);
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            TextEx.DrawText(_text, Resource.POPUP_FONT, _color, CurrentX, CurrentY, 40, FontAlignment.AlignCenter);
        }
        //#----------------------------------------------------------
        //# * Current X
        //#----------------------------------------------------------
        protected override int CurrentX
        {
            get { return _startPt.X; }
        }
        //#----------------------------------------------------------
        //# * Current Y
        //#----------------------------------------------------------
        protected override int CurrentY
        {
            get { return _startPt.Y - _tick; }
        }
    }
}