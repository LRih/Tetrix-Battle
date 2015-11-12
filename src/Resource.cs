using System;
using SwinGame;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Resource
    //#==============================================================
    public static class Resource
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public static readonly Font MENU_FONT = new Font("maven_pro_regular", 20);
        public static readonly Font POPUP_FONT = new Font("maven_pro_regular",10);
        //#----------------------------------------------------------
        //# * Load Resources
        //#----------------------------------------------------------
        public static void LoadResources()
        {
            // load graphics
            Images.LoadBitmapNamed("window_confirm", "window_confirm.png");
            Images.LoadBitmapNamed("empty", "empty.png");

            Images.LoadBitmapNamed("units", "units.png");
            Images.BitmapNamed("units").SetCellDetails(40, 40, 11, 7, 77);
            Images.LoadBitmapNamed("units2", "units2.png");
            Images.BitmapNamed("units2").SetCellDetails(40, 40, 11, 7, 77);
            Images.LoadBitmapNamed("units_big", "units_big.png");
            Images.BitmapNamed("units_big").SetCellDetails(80, 80, 11, 5, 55);
            Images.LoadBitmapNamed("battlegroup_arrow", "battlegroup_arrow.png");
            Images.LoadBitmapNamed("text_victory", "text_victory.png");
            Images.LoadBitmapNamed("text_defeat", "text_defeat.png");
            Images.LoadBitmapNamed("veteran_glow", "veteran_glow.png");
            Images.BitmapNamed("veteran_glow").SetCellDetails(40, 40, 4, 1, 4);
            Images.LoadBitmapNamed("effect_arrow", "effect_arrow.png");
            Images.LoadBitmapNamed("effect_rock", "effect_rock.png");
            Images.LoadBitmapNamed("effect_magic", "effect_magic.png");
            Images.LoadBitmapNamed("hit_arrow", "hit_arrow.png");
            Images.BitmapNamed("hit_arrow").SetCellDetails(40, 40, 4, 1, 4);
            Images.LoadBitmapNamed("hit_magic", "hit_magic.png");
            Images.BitmapNamed("hit_magic").SetCellDetails(40, 40, 4, 1, 4);
            Images.LoadBitmapNamed("hit_rock", "hit_rock.png");
            Images.BitmapNamed("hit_rock").SetCellDetails(40, 40, 4, 1, 4);
        }
        //#----------------------------------------------------------
        //# * Free Resources
        //#----------------------------------------------------------
        public static void FreeResources()
        {
            // free font
            MENU_FONT.Dispose();
            POPUP_FONT.Dispose();
            // free graphics
            Images.FreeBitmap(Images.BitmapNamed("window_confirm"));
            Images.FreeBitmap(Images.BitmapNamed("empty"));

            Images.FreeBitmap(Images.BitmapNamed("units"));
            Images.FreeBitmap(Images.BitmapNamed("units2"));
            Images.FreeBitmap(Images.BitmapNamed("units_big"));
            Images.FreeBitmap(Images.BitmapNamed("battlegroup_arrow"));
            Images.FreeBitmap(Images.BitmapNamed("text_victory"));
            Images.FreeBitmap(Images.BitmapNamed("text_defeat"));
            Images.FreeBitmap(Images.BitmapNamed("veteran_glow"));
            Images.FreeBitmap(Images.BitmapNamed("effect_arrow"));
            Images.FreeBitmap(Images.BitmapNamed("effect_rock"));
            Images.FreeBitmap(Images.BitmapNamed("effect_magic"));
            Images.FreeBitmap(Images.BitmapNamed("hit_arrow"));
            Images.FreeBitmap(Images.BitmapNamed("hit_magic"));
            Images.FreeBitmap(Images.BitmapNamed("hit_rock"));

        }
        //#----------------------------------------------------------
        //# * Play Music
        //#----------------------------------------------------------
        public static void PlayMusic(string name, bool onceOnly = false)
        {
            //if (Global.CurrentMusic != name)
            //{
            //    if (onceOnly)
            //    {
            //        Global.CurrentMusic = string.Empty;
            //        Audio.MusicNamed(name).Play(0);
            //    }
            //    else
            //    {
            //        Global.CurrentMusic = name;
            //        Audio.MusicNamed(name).Play();
            //    }
            //}
        }
        //#----------------------------------------------------------
        //# * Stop Music
        //#----------------------------------------------------------
        public static void StopMusic()
        {
            //Global.CurrentMusic = string.Empty;
            //Audio.StopMusic();
        }
        //#----------------------------------------------------------
        //# * Play Sound
        //#----------------------------------------------------------
        public static void PlaySound(string name)
        {
            //Audio.SoundEffectNamed(name).Play();
        }
    }
}
