using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_Editor
    //#==============================================================
    public class UI_Editor
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private readonly int ITEM_WIDTH = 80;
        private Point2DEx _start; // co-ord of menu
        private Point2DEx _index = new Point2DEx();
        private int _selectionAni = 0;
        private int _selectionAniDir = -2;
        public Game_Group Group;
        private int _pickedUpUnit = -1;
        public bool Active = true;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_Editor(int x, int y)
        {
            Images.LoadBitmapNamed("editor_selection", "editor_selection.png");
            _start = new Point2DEx(x, y);
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public void Update()
        {
            // update selection flash
            _selectionAni += _selectionAniDir;
            if (_selectionAni == -6) _selectionAniDir = 1;
            else if (_selectionAni == 0) _selectionAniDir = -1;
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public void UpdateInput()
        {
            if (Active)
            {
                int lastIndexX = IndexX;
                int lastIndexY = IndexY;
                if (Controls.DownTyped()) IndexY++;
                else if (Controls.UpTyped()) IndexY--;
                else if (Controls.RightTyped()) IndexX++;
                else if (Controls.LeftTyped()) IndexX--;
                if (lastIndexX != IndexX || lastIndexY != IndexY) Resource.PlaySound("cursor");
            }
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public void UpdateDraw()
        {
            if (Group != null)
            {
                DrawInvalidPts();
                Group.Draw(_start.X, _start.Y, true);
            }
            if (Active)
            {
                DrawPickedUpUnit();
                DrawSelection();
            }
        }
        private void DrawInvalidPts()
        {
            for (int y = 0; y < Global.MAX_GROUP_WIDTH; y++)
            {
                for (int x = 0; x < Global.MAX_GROUP_WIDTH; x++)
                    if (!Group.IsAttachablePt(x, y)) Graphics.FillRectangle(Color.FromArgb(175, Color.Red), _start.X + x * ITEM_WIDTH, _start.Y + y * ITEM_WIDTH, ITEM_WIDTH, ITEM_WIDTH);
            }
        }
        private void DrawPickedUpUnit()
        {
            if (IsUnitPickedUp())
            {
                int x = _start.X + IndexX * ITEM_WIDTH - 10;
                int y = _start.Y + IndexY * ITEM_WIDTH - 10;
                Graphics.FillRectangle(Color.LightGray, x, y, ITEM_WIDTH, ITEM_WIDTH); // draw base
                Graphics.DrawRectangle(Color.Gray, x, y, ITEM_WIDTH, ITEM_WIDTH); //draw border
                Images.DrawCell(Images.BitmapNamed("units_big"), Global.Units[_pickedUpUnit].BitmapCell, x, y - 16); // draw unit
            }
        }
        private void DrawSelection()
        {
            Images.BitmapNamed("editor_selection").Draw(_start.X + IndexX * ITEM_WIDTH + _selectionAni - 20, _start.Y + IndexY * ITEM_WIDTH + _selectionAni - 20);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public void Dispose()
        {
            Images.FreeBitmap(Images.BitmapNamed("editor_selection"));
        }
        //#======================================================================================
        public bool IsUnitPickedUp()
        {
            return (_pickedUpUnit != -1);
        }
        public bool IsSelectedUnitEmpty()
        {
            return (SelectedUnit == -1);
        }
        public bool CanPickUpSelectedUnit()
        {
            return (!IsUnitPickedUp() && !IsSelectedUnitEmpty());
        }
        public bool CanSwapUnit()
        {
            return (IsUnitPickedUp() && !IsSelectedUnitEmpty());
        }
        public bool CanDropPickedUpUnit()
        {
            return (IsUnitPickedUp() && IsSelectedUnitEmpty());
        }
        public bool IsUnitPlaceable()
        {
            return (!IsUnitPickedUp() && IsSelectedUnitEmpty());
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Get Unit
        //#----------------------------------------------------------
        private int GetUnitID(int x, int y)
        {
            return Group.UnitIDs[x, y];
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * Pick Up Unit
        //#----------------------------------------------------------
        public void PickUpUnit()
        {
            _pickedUpUnit = SelectedUnit;
            Group.UnitIDs[IndexX, IndexY] = -1;
        }
        //#----------------------------------------------------------
        //# * Swap Unit
        //#----------------------------------------------------------
        public void SwapUnit()
        {
            int tempUnit = _pickedUpUnit;
            _pickedUpUnit = SelectedUnit;
            Group.UnitIDs[IndexX, IndexY] = tempUnit;
        }
        //#----------------------------------------------------------
        //# * Drop Unit
        //#----------------------------------------------------------
        public void DropUnit()
        {
            Group.UnitIDs[IndexX, IndexY] = _pickedUpUnit;
            DeletePickedUpUnit();
        }
        //#----------------------------------------------------------
        //# * Delete Selected Unit
        //#----------------------------------------------------------
        public void DeleteSelectedUnit()
        {
            Group.UnitIDs[IndexX, IndexY] = -1;
        }
        //#----------------------------------------------------------
        //# * Delete Picked Up Unit
        //#----------------------------------------------------------
        public void DeletePickedUpUnit()
        {
            _pickedUpUnit = -1;
        }
        //#======================================================================================
        //#----------------------------------------------------------
        //# * X
        //#----------------------------------------------------------
        public int X
        {
            get { return _start.X; }
            set { _start.X = value; }
        }
        //#----------------------------------------------------------
        //# * Y
        //#----------------------------------------------------------
        public int Y
        {
            get { return _start.Y; }
            set { _start.Y = value; }
        }
        //#----------------------------------------------------------
        //# * Index
        //#----------------------------------------------------------
        public int IndexX
        {
            get { return _index.X; }
            set { _index.X = Math.Min(Math.Max(value, 0), Global.MAX_GROUP_WIDTH - 1); }
        }
        //#----------------------------------------------------------
        //# * Index Y
        //#----------------------------------------------------------
        public int IndexY
        {
            get { return _index.Y; }
            set { _index.Y = Math.Min(Math.Max(value, 0), Global.MAX_GROUP_WIDTH - 1); }
        }
        //#----------------------------------------------------------
        //# * Selected Unit
        //#----------------------------------------------------------
        public int SelectedUnit
        {
            get { return GetUnitID(IndexX, IndexY); }
        }
        //#----------------------------------------------------------
        //# * Picked Up Unit
        //#----------------------------------------------------------
        public int PickedUpUnit
        {
            get { return _pickedUpUnit; }
            set { _pickedUpUnit = value; }
        }
    }
}
