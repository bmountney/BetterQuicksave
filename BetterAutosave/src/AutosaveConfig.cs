using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem;
using TaleWorlds.ObjectSystem;

namespace AutoSaveMod
{
    public class AutosaveConfig
    {
        [XmlIgnore]
        public string AutosaveNamePattern
        {
            get
            {
                string playerCharacterName = string.Empty;
                if (Main.Config.PerCharacterSaves && CurrentPlayerName.Length > 0)
                {
                    playerCharacterName = $"{Regex.Escape(CurrentPlayerName)} ";
                }
                string prefix = Regex.Escape(Main.Config.AutosavePrefix);
                string id;
                string suffix = Regex.Escape(Main.Config.AutosaveSuffix);
                if (UseDateTimeSuffix)
                    id = @"(\d{8}-\d{6})";
                else
                    id = @"(\d{3})";
                return $"^{playerCharacterName}^{prefix}{id}{suffix}$";
            }
        }

        public static string CurrentPlayerName
        {
            get
            {
                Hero player = Campaign.Current?.MainParty?.LeaderHero;
                return player != null ? $"{player.Name} {player.Clan.Name}" : "No Name";
            }
        }

        public string AutosavePrefix { get; set; } = "autosave ";
        public string AutosaveSuffix { get; set; } = "";
        public int MaxAutosaves { get; set; } = 5;
        public int AutoSaveTimeInMinutes { get; set; } = 15;
        public bool PerCharacterSaves { get; set; } = false;
        public bool UseDateTimeSuffix { get; set; } = false;

        private static string ConfigFilename
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../ModuleData/AutoSaveConfig.xml";
            }
        }

        public static AutosaveConfig LoadConfig()
        {
            AutosaveConfig autosaveConfig = AutosaveConfig.DeserializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename);
            if (autosaveConfig == null)
            {
                AutosaveConfig.SerializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename, new AutosaveConfig());
            }
            return AutosaveConfig.DeserializeConfig<AutosaveConfig>(AutosaveConfig.ConfigFilename);
        }

        private static void SerializeConfig<T>(string fileName, T config)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(config.GetType());
            using (StreamWriter streamWriter = File.CreateText(fileName))
            {
                xmlSerializer.Serialize(streamWriter, config);
            }
        }

        private static T DeserializeConfig<T>(string fileName) where T : class
        {
            T result;
            if (!File.Exists(fileName))
            {
                result = default(T);
            }
            else
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                try
                {
                    using (StreamReader streamReader = File.OpenText(fileName))
                    {
                        result = (T)((object)xmlSerializer.Deserialize(streamReader));
                    }
                }
                catch (Exception)
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    result = default(T);
                }
            }
            return result;
        }
    }
}
