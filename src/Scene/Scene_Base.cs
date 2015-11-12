using System;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Scene_Base
    //#==============================================================
    public class Scene_Base : IDisposable
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        protected Bitmap _backgroundImage;
        protected int _offsetX = 0;
        private int _fadeInAlpha = 255;
        //#----------------------------------------------------------
        //# * Initialize
        //#----------------------------------------------------------
        public Scene_Base(Bitmap background, string musicName = "")
        {
            _backgroundImage = background;
            if (musicName != "") Resource.PlayMusic(musicName);
            else Resource.StopMusic();
        }
        //#----------------------------------------------------------
        //# * Update
        //#----------------------------------------------------------
        public virtual void Update()
        {
            // update draw
            PreDraw();
            UpdateDraw();
            // update input
            if (_fadeInAlpha <= 0) UpdateInput();
            //Fetch the next batch of UI interaction
            Input.ProcessEvents();
        }
        //#----------------------------------------------------------
        //# * Update Input
        //#----------------------------------------------------------
        public virtual void UpdateInput()
        {
            if (Controls.FPSTyped()) Global.FPSShown = (Global.FPSShown != true);
            else if (Controls.FullscreenTyped()) Graphics.ToggleFullScreen();
            else if (Controls.LimitFPSTyped()) Global.LimitFPS = (Global.LimitFPS != true);
        }
        //#----------------------------------------------------------
        //# * Update Draw
        //#----------------------------------------------------------
        private void PreDraw()
        {
            Graphics.ClearScreen(Color.White);
            DrawBackground();
        }
        public virtual void UpdateDraw()
        {
            //Draw the framerate
            if (Global.FPSShown) Text.DrawFramerate(550, 576, Resource.MENU_FONT);
            if (_fadeInAlpha > 0)
            {
                Graphics.FillRectangle(Color.FromArgb(_fadeInAlpha, Color.Black), 0, 0, 800, 600);
                _fadeInAlpha -= 30;
            }
            //Draw onto the screen
            if (Global.LimitFPS) Graphics.RefreshScreen(30);
            else Graphics.RefreshScreen();
        }
        private void DrawBackground()
        {
            _backgroundImage.Draw(_offsetX, 0);
        }
        //#----------------------------------------------------------
        //# * Dispose
        //#----------------------------------------------------------
        public virtual void Dispose()
        {
            for (int alpha = 0; alpha < 255; alpha+=30)
            {
                Graphics.FillRectangle(Color.FromArgb(alpha, Color.Black), 0, 0, 800, 600);
                if (Global.LimitFPS) Graphics.RefreshScreen(30);
                else Graphics.RefreshScreen();
            }
            _backgroundImage.Dispose();
        }
    }
}
