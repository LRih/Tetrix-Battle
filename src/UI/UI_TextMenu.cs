using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_TextMenu
    //#==============================================================
    public class UI_TextMenu : UI_Menu
    {
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_TextMenu(string[] items, int x, int y, int width, int height, int index = 0, Bitmap window = null) : base(x, y, width, height, true, window)
        {
            Count = items.Length;
            _itemWidth = EffectiveWidth;
            _itemHeight = Resource.MENU_FONT.TextHeight("fp") + 10;
            InitializeBitmap(items);
            Index = index;
            _currentXY = MenuBitmapDrawY();
        }
        //#----------------------------------------------------------
        //# * Initialize Bitmap
        //#----------------------------------------------------------
        public override void InitializeBitmap(object[] items)
        {
            if (_menuBitmap != null) _menuBitmap.Dispose();
            _menuBitmap = Images.CreateBitmap(EffectiveWidth, items.Length * (_itemHeight + ITEM_SEPARATION));
            int y = ITEM_SEPARATION / 2;
            foreach (string item in items)
            {
                TextEx.DrawText(_menuBitmap, item, Resource.MENU_FONT, 0, y, EffectiveWidth);
                y += _itemHeight + ITEM_SEPARATION;
            }
            _menuBitmap.OptimiseBitmap();
        }
    }
}
