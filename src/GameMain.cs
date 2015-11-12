using System;
using SwinGame;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * GameMain
    //#==============================================================
    public class GameMain
    {
        //#----------------------------------------------------------
        //# * Variables
        //#----------------------------------------------------------
        public static Scene_Base Scene;
        //#----------------------------------------------------------
        //# * Main
        //#----------------------------------------------------------
        public static void Main()
        {
            Input.HideMouse();

            //Start the audio system so sound can be played
            Audio.OpenAudio();

            //Open the game window
            Graphics.OpenGraphicsWindow("Tetrix Battle", 800, 600);

            // load data and resources
            Resource.LoadResources();
            Saving.LoadUnitsData(ref Global.Units);

            //Run the game loop
            Scene = new Scene_Title();
            while (false == Input.WindowCloseRequested()) Scene.Update();

            //Close any resources we were using
            Audio.CloseAudio();
            Resource.FreeResources();
            Resources.ReleaseAllResources();
        }
    }
}