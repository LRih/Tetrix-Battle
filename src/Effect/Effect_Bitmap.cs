using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Effect_Bitmap
    //#==============================================================
    public class Effect_Bitmap : Effect
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private string _bitmapName;
        private Point2DEx _startPt;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Effect_Bitmap(string bitmapName, int x, int y) : base (16)
        {
            _bitmapName = bitmapName;
            _startPt = new Point2DEx(x, y);
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            Images.BitmapNamed(_bitmapName).DrawCell(_tick / 4, _startPt.X, _startPt.Y);
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