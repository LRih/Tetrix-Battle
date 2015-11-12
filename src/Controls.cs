using System;
using SwinGame;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Controls
    //#==============================================================
    public static class Controls
    {
        //#----------------------------------------------------------
        //# * Up Typed
        //#----------------------------------------------------------
        public static bool UpTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_UP));
        }
        //#----------------------------------------------------------
        //# * Down Typed
        //#----------------------------------------------------------
        public static bool DownTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_DOWN));
        }
        //#----------------------------------------------------------
        //# * Left Typed
        //#----------------------------------------------------------
        public static bool LeftTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_LEFT));
        }
        //#----------------------------------------------------------
        //# * Right Typed
        //#----------------------------------------------------------
        public static bool RightTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_RIGHT));
        }
        //#----------------------------------------------------------
        //# * Accept Typed
        //#----------------------------------------------------------
        public static bool AcceptTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_SPACE) || Input.KeyTyped(KeyCode.vk_RETURN) || Input.KeyTyped(KeyCode.vk_z));
        }
        //#----------------------------------------------------------
        //# * Secondary Typed
        //#----------------------------------------------------------
        public static bool SecondaryTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_LSHIFT) || Input.KeyTyped(KeyCode.vk_RSHIFT) || Input.KeyTyped(KeyCode.vk_x));
        }
        //#----------------------------------------------------------
        //# * Tertiary Typed
        //#----------------------------------------------------------
        public static bool TertiaryTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_c));
        }
        //#----------------------------------------------------------
        //# * Cancel Typed
        //#----------------------------------------------------------
        public static bool CancelTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_ESCAPE));
        }
        //#----------------------------------------------------------
        //# * FPS Typed
        //#----------------------------------------------------------
        public static bool FPSTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_F10));
        }
        //#----------------------------------------------------------
        //# * Fullscreen Typed
        //#----------------------------------------------------------
        public static bool FullscreenTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_F11));
        }
        //#----------------------------------------------------------
        //# * Limit FPS Typed
        //#----------------------------------------------------------
        public static bool LimitFPSTyped()
        {
            return (Input.KeyTyped(KeyCode.vk_F12));
        }
    }
}
