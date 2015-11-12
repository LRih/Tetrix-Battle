using System;
using Color = System.Drawing.Color;

namespace SwinGame
{
    //#==============================================================
    //# * TextEx
    //#==============================================================
    public static class TextEx
    {
        //#----------------------------------------------------------
        //# * Draw Text
        //#----------------------------------------------------------
        public static void DrawText(string text, Font font, int x, int y, int width, FontAlignment align = FontAlignment.AlignLeft)
        {
            DrawText(text, font, Color.White, x, y, width, align);
        }
        public static void DrawText(string text, Font font, Color color, int x, int y, int width, FontAlignment align = FontAlignment.AlignLeft)
        {
            Text.DrawTextLines(text, Color.Black, Color.Transparent, font, align, x + 1, y + 1, width, font.TextHeight(text));
            Text.DrawTextLines(text, color, Color.Transparent, font, align, x, y, width, font.TextHeight(text));
        }
        public static void DrawText(Bitmap dest, string text, Font font, int x, int y, int width, FontAlignment align = FontAlignment.AlignLeft)
        {
            Text.DrawTextLines(dest, text, Color.Black, Color.Transparent, font, align, x + 1, y + 1, width, font.TextHeight(text));
            Text.DrawTextLines(dest, text, Color.White, Color.Transparent, font, align, x, y, width, font.TextHeight(text));
        }
    }
    //#==============================================================
    //# * Point2DEx
    //#==============================================================
    public struct Point2DEx
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _x;
        private int _y;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Point2DEx(int x, int y)
        {
            _x = x;
            _y = y;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Add
        //#----------------------------------------------------------
        public Point2DEx Add(Point2DEx pt)
        {
            return new Point2DEx(X + pt.X, Y + pt.Y);
        }
        //#----------------------------------------------------------
        //# * X
        //#----------------------------------------------------------
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }
        //#----------------------------------------------------------
        //# * Y
        //#----------------------------------------------------------
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * == !=
        //#----------------------------------------------------------
        public static bool operator ==(Point2DEx pt1, Point2DEx pt2)
        {
            return (pt1.X == pt2.X && pt1.Y == pt2.Y);
        }
        public static bool operator !=(Point2DEx pt1, Point2DEx pt2)
        {
            return (pt1.X != pt2.X || pt1.Y != pt2.Y);
        }
    }
}
