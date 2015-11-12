using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * UI_IconMenu
    //#==============================================================
    public class UI_IconMenu : UI_Menu
    {
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public UI_IconMenu(Bitmap[] items, int x, int y, int width, int height, bool isVertical = true, int index = 0) : base(x, y, width, height, isVertical)
        {
            MARGIN = 20;
            ITEM_SEPARATION = 20;
            Count = items.Length;
            _itemWidth = items[0].Width;
            _itemHeight = items[0].Height;
            InitializeBitmap(items);
            Index = index;
            _currentXY = (isVertical ? MenuBitmapDrawY() : MenuBitmapDrawX());
        }
        //#----------------------------------------------------------
        //# * Initialize Bitmap
        //#----------------------------------------------------------
        public override void InitializeBitmap(object[] items)
        {
            if (_menuBitmap != null) _menuBitmap.Dispose();
            _menuBitmap = (_isVertical ? Images.CreateBitmap(EffectiveWidth, items.Length * (_itemHeight + ITEM_SEPARATION)) :
                Images.CreateBitmap(items.Length * (_itemWidth + ITEM_SEPARATION), EffectiveHeight));
            _menuBitmap.ClearSurface(Color.Magenta);
            int xy = ITEM_SEPARATION / 2;
            foreach (Bitmap item in items)
            {
                if (_isVertical) Images.DrawBitmap(_menuBitmap, item, 0, xy);
                else Images.DrawBitmap(_menuBitmap, item, xy, 0);
                xy += (_isVertical ? _itemHeight : _itemWidth) + ITEM_SEPARATION;
            }
            _menuBitmap.SetTransparentColor(Color.Magenta);
            _menuBitmap.OptimiseBitmap();
        }
    }
}
