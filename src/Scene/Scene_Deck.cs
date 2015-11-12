using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Deck
    //#     where the deck is edited
    //#==============================================================
    public class Scene_Deck : Scene_Base
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        private int _screenOffsetX = 0;
        private int _tickWarning = 0;
        private string _warning;
        private UI_IconMenu _menuDeck;
        private UI_Editor _editor;
        private UI_IconMenu _menuUnits;
        private Game_Deck _deck = Global.Player.Deck;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Deck() : base(new Bitmap("background_edit.png"), "menu")
        {
            InitializeDeckMenu();
            InitializeUnitsMenu();
            _editor = new UI_Editor(320, 100);
            _editor.Group = _deck.Groups[_menuDeck.Index];
            _editor.Active = false;
        }
        private void InitializeDeckMenu(int index = 0)
        {
            Bitmap[] bitmaps = _deck.GetBitmaps();
            _menuDeck = new UI_IconMenu(bitmaps, 0, 40, 240, 600 - 40, true, index);
            foreach (Bitmap bitmap in bitmaps) bitmap.Dispose();
        }
        private void ReinitializeDeckMenu()
        {
            Bitmap[] bitmaps = _deck.GetBitmaps();
            _menuDeck.InitializeBitmap(bitmaps);
            foreach (Bitmap bitmap in bitmaps) bitmap.Dispose();
        }
        private void InitializeUnitsMenu()
        {
            Bitmap[] bitmaps = new Bitmap[10];
            for (int i = 0; i < bitmaps.Length; i++)
            {
                bitmaps[i] = new Bitmap(200, 80);
                Graphics.FillRectangle(bitmaps[i], Color.LightGray, 60, 0, 80, 80);
                Graphics.DrawRectangle(bitmaps[i], Color.Gray, 60, 0, 80, 80);
                Images.DrawCell(bitmaps[i], Images.BitmapNamed("units_big"), i, 60, 0);
                if (!Global.Player.IsUnitUnlocked(i)) Graphics.FillRectangle(bitmaps[i], Color.FromArgb(100, Color.Red), 60, 0, 80, 80);
            }
            _menuUnits = new UI_IconMenu(bitmaps, 800, 40, 240, 360);
            foreach (Bitmap bitmap in bitmaps) bitmap.Dispose();
            _menuUnits.Active = false;
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public override void Update()
        {
            UpdateScreenOffset();
            _menuDeck.Update();
            _editor.Update();
            _menuUnits.Update();
            if (_tickWarning > 0) _tickWarning--;
            base.Update();
        }
        private void UpdateScreenOffset()
        {
            if (_offsetX > _screenOffsetX) _offsetX -= 60;
            else if (_offsetX < _screenOffsetX) _offsetX += 60;
            _menuDeck.X = _offsetX;
            _editor.X = 320 + _offsetX;
            _menuUnits.X = 800 + _offsetX;
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public override void UpdateInput()
        {
            if (_menuDeck.Active) UpdateMenuDeckInput();
            else if (_editor.Active) UpdateEditorInput();
            else if (_menuUnits.Active) UpdateMenuUnitsInput();

            base.UpdateInput();
        }
        private void UpdateMenuDeckInput()
        {
            if (Controls.AcceptTyped())
            {
                _menuDeck.Active = false;
                _editor.Active = true;
                _screenOffsetX = -240;
                Resource.PlaySound("decision");
            }
            else if (Controls.CancelTyped())
            {
                string illegalMessage = GetIllegalDeckMessage();
                if (illegalMessage != string.Empty)
                {
                    Resource.PlaySound("buzzer");
                    SetWarning(illegalMessage);
                }
                else
                {
                    Resource.PlaySound("cancel");
                    Saving.SavePlayerData(Global.Player);
                    this.Dispose();
                    GameMain.Scene = new Scene_Menu(1);
                }
            }
            // update group display in editor
            int groupIndex = _menuDeck.Index;
            _menuDeck.UpdateInput();
            if (groupIndex != _menuDeck.Index) _editor.Group = _deck.Groups[_menuDeck.Index];
        }
        private void UpdateEditorInput()
        {
            if (Controls.AcceptTyped()) UpdateEditorInputAccept();
            else if (Controls.CancelTyped())
            {
                if (_editor.IsUnitPickedUp()) _editor.DeletePickedUpUnit();
                else
                {
                    ReinitializeDeckMenu(); // re-make deck menu to reflect changes in the editor
                    _menuDeck.Active = true;
                    _editor.Active = false;
                    _screenOffsetX = 0;
                }
                Resource.PlaySound("cancel");
            }
            else if (Controls.SecondaryTyped()) // rotate group
            {
                _editor.Group.Rotate();
                Resource.PlaySound("cursor");
            }
            else if (Controls.TertiaryTyped()) // delete selected unit
            {
                _editor.DeleteSelectedUnit();
                Resource.PlaySound("cancel");
            }
            _editor.UpdateInput();
        }
        private void UpdateEditorInputAccept()
        {
            if (_editor.CanPickUpSelectedUnit()) // picking up unit
            {
                _editor.PickUpUnit();
                Resource.PlaySound("decision");
            }
            else if (_editor.CanSwapUnit()) // swapping up unit
            {
                _editor.SwapUnit();
                Resource.PlaySound("decision");
            }
            else if (_editor.CanDropPickedUpUnit()) // drop unit
            {
                _editor.DropUnit();
                Resource.PlaySound("decision");
            }
            else if (_editor.IsUnitPlaceable()) // placing new unit
            {
                _menuUnits.Active = true;
                _editor.Active = false;
                Resource.PlaySound("decision");
            }
            else Resource.PlaySound("buzzer");
        }
        private void UpdateMenuUnitsInput()
        {
            if (Controls.AcceptTyped())
            {
                // modify group with selected unit
                if (!Global.Player.IsUnitUnlocked(_menuUnits.Index)) Resource.PlaySound("buzzer");
                else
                {
                    _menuUnits.Active = false;
                    _editor.Active = true;
                    _editor.Group.UnitIDs[_editor.IndexX, _editor.IndexY] = _menuUnits.Index;
                    Resource.PlaySound("decision");
                }
            }
            else if (Controls.CancelTyped())
            {
                _menuUnits.Active = false;
                _editor.Active = true;
                Resource.PlaySound("cancel");
            }
            _menuUnits.UpdateInput();
        }
        private string GetIllegalDeckMessage()
        {
            if (_deck.NumberOfUnits == 0) return "Deck must contain at least 1 unit";
            else if (GetDeckUnitNumber() > Game_Deck.MAX_UNITS) return "Deck contains too many units";
            for (int groupID = 0; groupID < Game_Deck.MAX_GROUPS; groupID++)
            {
                if (!_deck.Groups[groupID].IsValid()) return string.Format("Group {0} contains more than 1 group", groupID + 1);
            }
            return string.Empty;
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        public override void UpdateDraw()
        {
            DrawText();
            if (_menuUnits.Active) DrawUnitInformation();
            DrawUnitLimits();
            _menuDeck.UpdateDraw();
            _editor.UpdateDraw();
            _menuUnits.UpdateDraw();
            if (_tickWarning > 0) TextEx.DrawText(_warning, Resource.MENU_FONT, 0 + _screenOffsetX, 525, 1040, FontAlignment.AlignCenter);

            base.UpdateDraw();
        }
        private void DrawText()
        {
            TextEx.DrawText("Deck", Resource.MENU_FONT, 10 + _offsetX, 8, 240 - 20);
            TextEx.DrawText("Units", Resource.MENU_FONT, 810 + _offsetX, 8, 240 - 20);
            if (_editor.Active)
            {
                TextEx.DrawText("X: Rotate Group", Resource.MENU_FONT, 320 + _offsetX, 560 + 8, 210 - 20);
                TextEx.DrawText("C: Delete Unit", Resource.MENU_FONT, 530 + _offsetX, 560 + 8, 210 - 20);
            }
        }
        private void DrawUnitInformation()
        {
            int startY = 400;
            Data_Unit unit = Global.Units[_menuUnits.Index];
            TextEx.DrawText(unit.Name, Resource.MENU_FONT, Color.Gold, 810 + _offsetX, startY + 8, 240 - 20);
            if (!Global.Player.IsUnitUnlocked(_menuUnits.Index))
                TextEx.DrawText("Locked Unit", Resource.MENU_FONT, 810 + _offsetX, startY + 24 + 8, 240 - 20);
            else
            {
                int y = 0;
                foreach (string line in unit.Information)
                {
                    TextEx.DrawText(line, Resource.MENU_FONT, 810 + _offsetX, startY + 24 + y + 8, 240 - 20);
                    y += 24;
                }
            }
        }
        private void DrawUnitLimits()
        {
            Color deckColor = (GetDeckUnitNumber() > Game_Deck.MAX_UNITS ? Color.Red : Color.White);
            TextEx.DrawText("Group Value:", Resource.MENU_FONT, 320 + _offsetX, 25 + 8, 210 - 20);
            TextEx.DrawText("Deck No.:", Resource.MENU_FONT, 530 + _offsetX, 25 + 8, 210 - 20);
            TextEx.DrawText(GetGroupValue().ToString(), Resource.MENU_FONT, 320 + _offsetX, 25 + 8, 210 - 20, FontAlignment.AlignRight);
            TextEx.DrawText(GetDeckUnitNumber().ToString(), Resource.MENU_FONT, deckColor, 530 + _offsetX, 25 + 8, 163 - 20, FontAlignment.AlignRight);
            TextEx.DrawText(string.Format(" / {0}", Game_Deck.MAX_UNITS), Resource.MENU_FONT, 530 + _offsetX, 25 + 8, 210 - 20, FontAlignment.AlignRight);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public override void Dispose()
        {
            _editor.Dispose();
            _menuDeck.Dispose();
            _menuUnits.Dispose();
            base.Dispose();
        }
        //#----------------------------------------------------------
        //# * Get Group Value
        //#----------------------------------------------------------
        private int GetGroupValue()
        {
            int pointValue = _deck.Groups[_menuDeck.Index].PointValue;
            if (_menuUnits.Active) pointValue += Global.Units[_menuUnits.Index].PointValue;
            return pointValue;
        }
        //#----------------------------------------------------------
        //# * Get Deck Unit Number
        //#----------------------------------------------------------
        private int GetDeckUnitNumber()
        {
            int numberOfUnits = _deck.NumberOfUnits;
            if (_menuUnits.Active) numberOfUnits++;
            return numberOfUnits;
        }
        //#----------------------------------------------------------
        //# * Set Warning
        //#----------------------------------------------------------
        private void SetWarning(string message)
        {
            _tickWarning = 60;
            _warning = message;
        }
    }
}
