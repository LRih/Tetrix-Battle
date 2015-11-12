using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_Menu
    //#==============================================================
    public abstract class UI_Menu
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        protected int MARGIN = 10;
        protected int ITEM_SEPARATION = 4;
        protected Bitmap _windowBitmap; // bitmap of the optional window
        protected Bitmap _menuBitmap; // bitmap of the menu
        private int _startX; // x co-ord of menu
        private int _startY; // y co-ord of menu
        protected int _width;
        protected int _height;
        protected int _itemWidth;
        protected int _itemHeight;
        protected bool _isVertical;
        protected int _index = 0;
        protected int _selectionFlash = 255;
        protected int _selectionFlashDir = -10;
        protected int _currentXY;
        public int Count;
        public bool Active = true;
        public bool Visible = true;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_Menu(int x, int y, int width, int height, bool isVertical = true, Bitmap window = null)
        {
            _startX = x;
            _startY = y;
            _width = width;
            _height = height;
            _isVertical = isVertical;
            _windowBitmap = window;
        }
        //#----------------------------------------------------------
        //# * Initialize Bitmap
        //#----------------------------------------------------------
        public abstract void InitializeBitmap(object[] items);
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public virtual void Update()
        {
            // update selection flash
            _selectionFlash += _selectionFlashDir;
            if (_selectionFlash == 105) _selectionFlashDir = 10;
            else if (_selectionFlash == 255) _selectionFlashDir = -10;
            // update index change
            int menuBitmapDrawXY;
            int itemWidthHeight;
            if (_isVertical)
            {
                menuBitmapDrawXY = MenuBitmapDrawY();
                itemWidthHeight = _itemHeight;
            }
            else
            {
                menuBitmapDrawXY = MenuBitmapDrawX();
                itemWidthHeight = _itemWidth;
            }
            if (Math.Abs(_currentXY - menuBitmapDrawXY) < itemWidthHeight / 5) _currentXY = menuBitmapDrawXY;
            else if (_currentXY < menuBitmapDrawXY) _currentXY += itemWidthHeight / 5;
            else if (_currentXY > menuBitmapDrawXY) _currentXY -= itemWidthHeight / 5;
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public virtual void UpdateInput()
        {
            if (!Active) return;
            int lastIndex = Index;
            if (_isVertical)
            {
                if (Controls.DownTyped()) Index++;
                else if (Controls.UpTyped()) Index--;
            }
            else
            {
                if (Controls.RightTyped()) Index++;
                else if (Controls.LeftTyped()) Index--;
            }
            if (lastIndex != Index) Resource.PlaySound("cursor");
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public virtual void UpdateDraw()
        {
            //Graphics.DrawRectangle(Color.Blue, _startX, _startY, _width, _height);
            if (Visible)
            {
                DrawMenu();
                if (Active)
                {
                    DrawSelection();
                    DrawArrows();
                }
            }
        }
        protected virtual void DrawMenu()
        {
            if (_windowBitmap != null) _windowBitmap.Draw(_startX, _startY);
            if (_isVertical) _menuBitmap.DrawPart(0, _currentXY, EffectiveWidth, EffectiveHeight, Left + MARGIN, Top + MARGIN);
            else _menuBitmap.DrawPart(_currentXY, 0, EffectiveWidth, EffectiveHeight, Left + MARGIN, Top + MARGIN);
        }
        protected virtual void DrawSelection()
        {
            int x = (_isVertical ? Left + (MARGIN / 2) : SelectionX);
            int y = (_isVertical ? SelectionY : _startY + (MARGIN / 2));
            int width = (_isVertical ? _width - MARGIN : _itemWidth + ITEM_SEPARATION);
            int height = (_isVertical ? _itemHeight + ITEM_SEPARATION : _height - MARGIN);
            Graphics.DrawRectangle(Color.FromArgb(_selectionFlash, Color.Black), x + 1, y + 1, width, height);
            Graphics.DrawRectangle(Color.FromArgb(_selectionFlash, Color.White), x, y, width, height);
        }
        protected virtual void DrawArrows()
        {
            // if items exist above the top of menu
            if (MenuBitmapDrawY() > 0) Graphics.FillTriangle(Color.White, Left + _width / 2, Top + 2, Left + _width / 2 - 6, Top + 8, Left + _width / 2 + 6, Top + 8);
            // if items exist below bottom of menu
            if (MenuBitmapDrawY() + EffectiveHeight < _menuBitmap.Height) Graphics.FillTriangle(Color.White, Left + _width / 2, Bottom - 2, Left + _width / 2 - 6, Bottom - 8, Left + _width / 2 + 6, Bottom - 8);
            // if items exist left of menu
            if (MenuBitmapDrawX() > 0) Graphics.FillTriangle(Color.White, Left + 2, Top + _height / 2, Left + 8, Top + _height / 2 - 6, Left + 8, Top + _height / 2 + 6);
            // if items exist right of menu
            if (MenuBitmapDrawX() + EffectiveWidth < _menuBitmap.Width) Graphics.FillTriangle(Color.White, Right - 2, Top + _height / 2, Right - 8, Top + _height / 2 - 6, Right - 8, Top + _height / 2 + 6);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public virtual void Dispose()
        {
            if (_windowBitmap != null) _windowBitmap.Dispose();
            _menuBitmap.Dispose();
        }
        //#----------------------------------------------------------
        //# * X
        //#----------------------------------------------------------
        public int X
        {
            get { return _startX; }
            set { _startX = value; }
        }
        //#----------------------------------------------------------
        //# * Y
        //#----------------------------------------------------------
        public int Y
        {
            get { return _startY; }
            set { _startY = value; }
        }
        //#----------------------------------------------------------
        //# * Effective Width
        //#----------------------------------------------------------
        public int EffectiveWidth
        {
            get { return _width - MARGIN * 2; }
        }
        //#----------------------------------------------------------
        //# * Effective Height
        //#----------------------------------------------------------
        public int EffectiveHeight
        {
            get { return _height - MARGIN * 2; }
        }
        //#----------------------------------------------------------
        //# * Top
        //#----------------------------------------------------------
        public int Top
        {
            get { return _startY; }
        }
        //#----------------------------------------------------------
        //# * Bottom
        //#----------------------------------------------------------
        public int Bottom
        {
            get { return _startY +_height; }
        }
        //#----------------------------------------------------------
        //# * Left
        //#----------------------------------------------------------
        public int Left
        {
            get { return _startX; }
        }
        //#----------------------------------------------------------
        //# * Right
        //#----------------------------------------------------------
        public int Right
        {
            get { return _startX + _width; }
        }
        //#----------------------------------------------------------
        //# * Item X
        //#     the x co-ord of a specific item on the menu bitmap
        //#----------------------------------------------------------
        public int ItemX(int index)
        {
            if (_isVertical) return 0;
            else return Index * (_itemWidth + ITEM_SEPARATION);
        }
        //#----------------------------------------------------------
        //# * Item Y
        //#     the y co-ord of a specific item on the menu bitmap
        //#----------------------------------------------------------
        protected int ItemY(int index)
        {
            if (_isVertical) return Index * (_itemHeight + ITEM_SEPARATION);
            else return 0;
        }
        //#----------------------------------------------------------
        //# * Menu Bitmap Draw X
        //#     the x co-ord of the bitmap to start drawing at
        //#----------------------------------------------------------
        protected int MenuBitmapDrawX()
        {
            if (_isVertical) return 0;
            else return MARGIN - (_width - _itemWidth - ITEM_SEPARATION) / 2 + ItemX(Index);
        }
        //#----------------------------------------------------------
        //# * Menu Bitmap Draw Y
        //#     the y co-ord of the bitmap to start drawing at
        //#----------------------------------------------------------
        protected int MenuBitmapDrawY()
        {
            if (_isVertical) return MARGIN - (_height - _itemHeight - ITEM_SEPARATION) / 2 + ItemY(Index);
            else return 0;
        }
        //#----------------------------------------------------------
        //# * Selection Left X
        //#     x co-ord of the left of menu selection
        //#----------------------------------------------------------
        public int SelectionX
        {
            get { return _startX + (_width - _itemWidth - ITEM_SEPARATION) / 2; }
        }
        //#----------------------------------------------------------
        //# * Selection Top Y
        //#     y co-ord of the top of menu selection
        //#----------------------------------------------------------
        protected int SelectionY
        {
            get { return Top + (_height - _itemHeight - ITEM_SEPARATION) / 2; }
        }
        //#----------------------------------------------------------
        //# * Index
        //#----------------------------------------------------------
        public int Index
        {
            get { return _index; }
            set { _index = Math.Min(Math.Max(value, 0), Count - 1); }
        }
    }
}
