using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using SwinGame;
using Color = System.Drawing.Color;

namespace TetrixBattle.src
{
    //#==============================================================
    //# * Saving
    //#==============================================================
    public static class Saving
    {
        //#----------------------------------------------------------
        //# * Constants
        //#----------------------------------------------------------
        private static readonly string APP_PATH = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string SAVE_FOLDER_PATH = APP_PATH + string.Format("Data{0}", Path.DirectorySeparatorChar);
        private static readonly string PLAYER_SAVE_PATH = APP_PATH + string.Format("Data{0}player.txt", Path.DirectorySeparatorChar);
        private static readonly string UNITS_DATA_PATH = APP_PATH + string.Format("Data{0}units.txt", Path.DirectorySeparatorChar);
        public static readonly string LEVELS_DATA_PATH = APP_PATH + string.Format("Data{0}levels.txt", Path.DirectorySeparatorChar);
        //#----------------------------------------------------------
        //# * Save File Exists?
        //#----------------------------------------------------------
        public static bool SaveFileExists()
        {
            return (File.Exists(PLAYER_SAVE_PATH));
        }
        //#----------------------------------------------------------
        //# * Create New Save
        //#----------------------------------------------------------
        public static void CreateNewSave()
        {
            string save = string.Empty;
            // create default groups and units
            for (int groupID = 0; groupID < 6; groupID++)
            {
                for (int unitID = 0; unitID < 25; unitID++)
                {
                    if (groupID == 0 && (unitID == 7 || unitID == 12 || unitID == 17)) save += "0,";
                    else if (groupID == 1 && (unitID == 7 || unitID == 12 || unitID == 17)) save += "4,";
                    else if (unitID == 24) save += "-1;";
                    else save += "-1,";
                }
            }
            save += Environment.NewLine;
            // make scores
            for (int i = 0; i < Global.NUMBER_OF_LEVELS; i++) save += "0,";
            save += Environment.NewLine;
            // unlock default units
            save += "0,4,";
            Directory.CreateDirectory(SAVE_FOLDER_PATH);
            File.WriteAllText(PLAYER_SAVE_PATH, save);
        }
        //#----------------------------------------------------------
        //# * Load Player Data
        //#----------------------------------------------------------
        public static Game_Player LoadPlayerData()
        {
            Game_Player player = new Game_Player();
            string[] rawSave = File.ReadAllText(PLAYER_SAVE_PATH).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // load groups and units
            LoadIntoDeck(player.Deck, rawSave[0]);
            // load scores
            string[] scores = rawSave[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int level = 1; level <= Global.NUMBER_OF_LEVELS; level++)
            {
                player.SetLevelScore(level, Convert.ToInt32(scores[level - 1]));
            }
            // load unlocked units
            string[] unlockedUnits = rawSave[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string unitID in unlockedUnits) player.UnlockUnit(Convert.ToInt32(unitID));
            return player;
        }
        //#----------------------------------------------------------
        //# * Load Into Deck
        //#----------------------------------------------------------
        public static void LoadIntoDeck(Game_Deck deck, string deckLine)
        {
            string[] groups = deckLine.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int groupID = 0; groupID < 6; groupID++)
            {
                string[] units = groups[groupID].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int unitID = 0; unitID < 25; unitID++)
                {
                    deck.Groups[groupID].UnitIDs[(unitID % 5), (unitID / 5)] = Convert.ToInt32(units[unitID]);
                }
            }
        }
        //#----------------------------------------------------------
        //# * Save Player Data
        //#----------------------------------------------------------
        public static void SavePlayerData(Game_Player player)
        {
            string save = string.Empty;
            // save groups
            foreach (Game_Group group in player.Deck.Groups)
            {
                for (int y = 0; y <= group.UnitIDs.GetUpperBound(1); y++)
                {
                    for (int x = 0; x <= group.UnitIDs.GetUpperBound(0); x++)
                    {
                        string seperator = (x == 4 && y == 4 ? ";" : ",");
                        save += group.UnitIDs[x, y].ToString() + seperator;
                    }
                }
            }
            save += Environment.NewLine;
            // save scores
            for (int level = 1; level <= Global.NUMBER_OF_LEVELS; level++) save += player.GetLevelScore(level) + ",";
            save += Environment.NewLine;
            // save unlocked units
            for (int unitID = 0; unitID < 10; unitID++) if (player.IsUnitUnlocked(unitID)) save += unitID.ToString() + ",";
            Directory.CreateDirectory(SAVE_FOLDER_PATH);
            File.WriteAllText(PLAYER_SAVE_PATH, save);
        }
        //#----------------------------------------------------------
        //# * Load Units Data
        //#----------------------------------------------------------
        public static void LoadUnitsData(ref Dictionary<int, Data_Unit> list)
        {
            string rawSave = File.ReadAllText(UNITS_DATA_PATH);
            string[] units = rawSave.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            int id = -1;
            foreach (string unit in units)
            {
                if (!unit.StartsWith("#"))
                {
                    list.Add(id, new Data_Unit(unit));
                    id++;
                }
            }
        }
    }
}