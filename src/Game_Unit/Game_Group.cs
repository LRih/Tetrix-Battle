using System;
using System.Collections.Generic;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Game_Group
    //#==============================================================
    public class Game_Group
    {
        //#----------------------------------------------------------
        //# * Constants
        //#----------------------------------------------------------
        public const int MAX_WIDTH = 5;
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _index;
        private int[,] _unitIDs = new int[5, 5]; // stores unit ids in a 5x5 grid
        public readonly bool IsPlayerOwned;
        public bool IsVeteran = false;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Game_Group(int index, bool isPlayerOwned)
        {
            _index = index;
            IsPlayerOwned = isPlayerOwned;
            // initialize group to empty
            foreach (Point2DEx pt in GetUnitPts()) _unitIDs[pt.X, pt.Y] = -1;
        }
        public int[,] UnitIDs
        {
            get { return _unitIDs; }
        }
        public Color Color
        {
            get
            {
                if (IsVeteran)
                {
                    if (IsPlayerOwned) return Color.White;
                    else return Color.Black;
                }
                else
                {
                    if (IsPlayerOwned) return Color.FromArgb(155, 255 - _index * 30, 255);
                    else return Color.FromArgb(255 - _index * 30, 155 - _index * 30, 0);
                }
            }
        }
        public int PointValue
        {
            get
            {
                int value = 0;
                foreach (Point2DEx pt in GetUnitPts()) value += Global.Units[_unitIDs[pt.X, pt.Y]].PointValue;
                if (IsVeteran) value *= 8;
                return value;
            }
        }
        public int NumberOfUnits
        {
            get
            {
                int number = 0;
                foreach (Point2DEx pt in GetUnitPts()) if (_unitIDs[pt.X, pt.Y] != -1) number++;
                return number;
            }
        }
        public int[] RowsWithUnits
        {
            get
            {
                List<int> rows = new List<int>();
                foreach (Point2DEx pt in GetUnitPts()) if (_unitIDs[pt.X, pt.Y] != -1 && !rows.Contains(pt.Y)) rows.Add(pt.Y);
                return rows.ToArray();
            }
        }
        public int TopMostUnit
        {
            get
            {
                // iterate top to bottom to find topmost unit
                for (int y = 0; y < MAX_WIDTH; y++)
                {
                    for (int x = 0; x < MAX_WIDTH; x++) if (UnitIDs[x, y] != -1) return y;
                }
                return 0;
            }
        }
        public int BottomMostUnit
        {
            get
            {
                // iterate top to bottom to find topmost unit
                for (int y = MAX_WIDTH - 1; y >= 0; y--)
                {
                    for (int x = 0; x < MAX_WIDTH; x++) if (UnitIDs[x, y] != -1) return y;
                }
                return 0;
            }
        }
        public int LeftMostUnit
        {
            get
            {
                // iterate left to right to find leftmost unit
                for (int x = 0; x < MAX_WIDTH; x++)
                {
                    for (int y = 0; y < MAX_WIDTH; y++) if (UnitIDs[x, y] != -1) return x;
                }
                return 0;
            }
        }
        public int RightMostUnit
        {
            get
            {
                // iterate right to left to find rightmost unit
                for (int x = MAX_WIDTH - 1; x >= 0; x--)
                {
                    for (int y = 0; y < MAX_WIDTH; y++) if (UnitIDs[x, y] != -1) return x;
                }
                return 0;
            }
        }
        //#======================================================================================
        public void Rotate()
        {
            int[,] rotated = new int[MAX_WIDTH, MAX_WIDTH];
            foreach (Point2DEx pt in GetUnitPts()) rotated[pt.X, pt.Y] = _unitIDs[MAX_WIDTH - pt.Y - 1, pt.X];
            _unitIDs = rotated;
        }
        public void Clear()
        {
            foreach (Point2DEx pt in GetUnitPts()) UnitIDs[pt.X, pt.Y] = -1;
        }
        public IEnumerable<Point2DEx> GetUnitPts()
        {
            for (int y = 0; y < MAX_WIDTH; y++)
            {
                for (int x = 0; x < MAX_WIDTH; x++)
                {
                    yield return new Point2DEx(x, y);
                }
            }
        }
        private List<Point2DEx> GetAdjacentPts(int x, int y)
        {
            List<Point2DEx> list = new List<Point2DEx>();
            list.Add(new Point2DEx(x, y - 1));
            list.Add(new Point2DEx(x, y + 1));
            list.Add(new Point2DEx(x - 1, y));
            list.Add(new Point2DEx(x + 1, y));
            return list;
        }
        private List<Point2DEx> GetValidAdjacentPts(int x, int y)
        {
            List<Point2DEx> list = new List<Point2DEx>();
            if (IsValidPt(x, y - 1)) list.Add(new Point2DEx(x, y - 1));
            if (IsValidPt(x, y + 1)) list.Add(new Point2DEx(x, y + 1));
            if (IsValidPt(x - 1, y)) list.Add(new Point2DEx(x - 1, y));
            if (IsValidPt(x + 1, y)) list.Add(new Point2DEx(x + 1, y));
            return list;
        }
        public Game_Group Clone()
        {
            Game_Group clone = new Game_Group(_index, IsPlayerOwned);
            foreach (Point2DEx pt in GetUnitPts()) clone.UnitIDs[pt.X, pt.Y] = UnitIDs[pt.X, pt.Y];
            return clone;
        }
        public Game_BattleUnit CreateBattleUnit(int x, int y)
        {
            Game_BattleUnit unit = new Game_BattleUnit(Global.Units[UnitIDs[x, y]], this, IsPlayerOwned, IsVeteran);
            List<Point2DEx> AdjPts = GetAdjacentPts(x, y);
            for (int i = 0; i < 4; i++) if (IsOccupiedPt(AdjPts[i].X, AdjPts[i].Y)) unit.Connections[i] = true;
            return unit;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Is Empty
        //#     when group contains no units
        //#----------------------------------------------------------
        public bool IsEmpty()
        {
            return NumberOfUnits == 0;
        }
        //#----------------------------------------------------------
        //# * Is Attachable Pt
        //#     check if pt is a valid point to place unit
        //#----------------------------------------------------------
        public bool IsAttachablePt(int x, int y)
        {
            if (IsEmpty()) return true;
            foreach (Point2DEx pt in GetValidAdjacentPts(x, y))
            {
                if (IsOccupiedPt(pt.X, pt.Y)) return true;
            }
            return false;
        }
        //#----------------------------------------------------------
        //# * Is Valid Pt
        //#----------------------------------------------------------
        public bool IsValidPt(int x, int y)
        {
            if (x < 0 || x >= MAX_WIDTH) return false;
            if (y < 0 || y >= MAX_WIDTH) return false;
            return true;
        }
        //#----------------------------------------------------------
        //# * Is Empty Point
        //#     includes invalid points
        //#----------------------------------------------------------
        public bool IsEmptyPt(int x, int y)
        {
            return (!IsValidPt(x, y) || _unitIDs[x, y] == -1);
        }
        //#----------------------------------------------------------
        //# * Is Occupied Pt
        //#----------------------------------------------------------
        public bool IsOccupiedPt(int x, int y)
        {
            return (IsValidPt(x, y) && _unitIDs[x, y] != -1);
        }
        //#----------------------------------------------------------
        //# * Is Valid
        //#     checking if group is valid (all units connected) as
        //#     one group
        //#----------------------------------------------------------
        public bool IsValid()
        {
            if (IsEmpty()) return true;
            Point2DEx startPt = new Point2DEx(-1, -1);
            List<Point2DEx> checkedPts = new List<Point2DEx>();
            // find starting pt
            foreach (Point2DEx pt in GetUnitPts())
            {
                if (_unitIDs[pt.X, pt.Y] != -1)
                {
                    startPt = pt;
                    break;
                }
            }
            GetConnectedUnits(startPt, ref checkedPts);
            return (checkedPts.Count == NumberOfUnits);
        }
        private void GetConnectedUnits(Point2DEx srcPt, ref List<Point2DEx> checkedPts)
        {
            checkedPts.Add(srcPt);
            foreach (Point2DEx adjPt in GetValidAdjacentPts(srcPt.X, srcPt.Y))
            {
                if (!checkedPts.Contains(adjPt) && _unitIDs[adjPt.X, adjPt.Y] != -1) GetConnectedUnits(adjPt, ref checkedPts);
            }
        }
        //#======================================================================================
        public void Draw(int x, int y, bool big = false)
        {
            string bitmapName = (big ? "units_big" : "units");
            int offsetY = (big ? 16 : 8);
            int unitThickness = (big ? 80 : 40);
            // iterate through units and draw each one
            foreach (Point2DEx pt in GetUnitPts())
            {
                if (_unitIDs[pt.X, pt.Y] != -1) // if unit exists
                {
                    int drawingX = x + pt.X * unitThickness;
                    int drawingY = y + pt.Y * unitThickness;
                    Graphics.FillRectangle(Color, drawingX, drawingY, unitThickness, unitThickness); // draw base
                    //draw border
                    if (IsEmptyPt(pt.X, pt.Y - 1)) Graphics.DrawLine(Color.Blue, drawingX, drawingY, drawingX + unitThickness - 1, drawingY);
                    if (IsEmptyPt(pt.X, pt.Y + 1)) Graphics.DrawLine(Color.Blue, drawingX, drawingY + unitThickness - 1, drawingX + unitThickness - 1, drawingY + unitThickness - 1);
                    if (IsEmptyPt(pt.X - 1, pt.Y)) Graphics.DrawLine(Color.Blue, drawingX, drawingY, drawingX, drawingY + unitThickness - 1);
                    if (IsEmptyPt(pt.X + 1, pt.Y)) Graphics.DrawLine(Color.Blue, drawingX + unitThickness - 1, drawingY, drawingX + unitThickness - 1, drawingY + unitThickness - 1);
                    Images.DrawCell(Images.BitmapNamed(bitmapName), _unitIDs[pt.X, pt.Y], drawingX, drawingY - offsetY); // draw unit
                }
            }
        }
        public Bitmap GetBitmap()
        {
            Bitmap bitmap = Images.CreateBitmap(200, 200); // stores bitmap of group
            if (IsEmpty())
            {
                Graphics.FillRectangle(bitmap, Color.White, 0, 0, 200, 200);
                Images.BitmapNamed("empty").DrawOnto(bitmap, 0, 0);
            }
            else
            {
                // iterate through units and draw each one
                foreach (Point2DEx pt in GetUnitPts())
                {
                    if (_unitIDs[pt.X, pt.Y] != -1)
                    {
                        int drawingX = pt.X * 40;
                        int drawingY = pt.Y * 40;
                        Graphics.FillRectangle(bitmap, Color, drawingX, drawingY, 40, 40); // draw base
                        //draw border
                        if (IsEmptyPt(pt.X, pt.Y - 1)) Graphics.DrawLine(bitmap, Color.Blue, drawingX, drawingY, drawingX + 39, drawingY);
                        if (IsEmptyPt(pt.X, pt.Y + 1)) Graphics.DrawLine(bitmap, Color.Blue, drawingX, drawingY + 39, drawingX + 39, drawingY + 39);
                        if (IsEmptyPt(pt.X - 1, pt.Y)) Graphics.DrawLine(bitmap, Color.Blue, drawingX, drawingY, drawingX, drawingY + 39);
                        if (IsEmptyPt(pt.X + 1, pt.Y)) Graphics.DrawLine(bitmap, Color.Blue, drawingX + 39, drawingY, drawingX + 39, drawingY + 39);
                        Images.DrawCell(bitmap, Images.BitmapNamed("units"), _unitIDs[pt.X, pt.Y], drawingX, drawingY); // draw unit
                    }
                }
            }
            bitmap.OptimiseBitmap();
            return bitmap;
        }
    }
}
