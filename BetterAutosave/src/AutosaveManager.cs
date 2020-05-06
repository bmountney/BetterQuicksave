using System;
using System.Text.RegularExpressions;
using TaleWorlds.Core;

namespace AutoSaveMod
{
    class AutosaveManager
    {
        public static string GetAutosaveName()
        {
            string characterName = Main.Config.PerCharacterSaves ? $"{AutosaveConfig.CurrentPlayerName} " : string.Empty;
            string id;
            if (Main.Config.UseDateTimeSuffix)
            {
                DateTime ts = DateTime.Now;
                id = $"{ts.Year:0000}" + $"{ts.Month:00}" + $"{ts.Day:00}" + $"-{ts.Hour:00}" + $"{ts.Minute:00}" + $"{ts.Second:00}"; // + $"-{ts.Millisecond:000}";
            }
            else
            {
                if (currentAutosaveNum >= Main.Config.MaxAutosaves)
                {
                    currentAutosaveNum = 0;
                }
                id = $"{++currentAutosaveNum:000}";
            }
            return $"{characterName}{Main.Config.AutosavePrefix}{id}{Main.Config.AutosaveSuffix}";
        }

        public static bool IsValidName(string name)
        {
            return Regex.IsMatch(name, Main.Config.AutosaveNamePattern);
        }

        private static int GetCurrentAutosaveNumber()
        {
            SaveGameFileInfo[] saveFiles = MBSaveLoad.GetSaveFiles();
            foreach (SaveGameFileInfo saveGameFileInfo in saveFiles)
            {
                Match match = Regex.Match(saveGameFileInfo.Name, Main.Config.AutosaveNamePattern);
                if (match.Success)
                {
                    int result;
                    int.TryParse(match.Groups[1].Value, out result);
                    return result;
                }
            }
            return 0;
        }

        private static int currentAutosaveNum = GetCurrentAutosaveNumber();
    }
}
