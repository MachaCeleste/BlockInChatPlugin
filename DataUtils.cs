using BepInEx;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BlockInChatPlugin
{
    internal class DataUtils
    {
        public static List<string> blockedPlayers;

        internal static string dataFilePath = Path.Combine(Paths.ConfigPath, "Blocklist.json");

        public static void LoadBlocklist()
        {
            if (!File.Exists(dataFilePath))
            {
                Plugin.Logger.LogWarning("No blocklist found");
                blockedPlayers = new List<string>();
                SaveBlocklist();
                return;
            }
            string json = File.ReadAllText(dataFilePath);
            blockedPlayers = JsonConvert.DeserializeObject<List<string>>(json);
            Plugin.Logger.LogInfo("Blocklist loaded");
        }

        public static void SaveBlocklist()
        {
            string json = JsonConvert.SerializeObject(blockedPlayers, Formatting.Indented);
            File.WriteAllText(dataFilePath, json);
            Plugin.Logger.LogInfo("Blocklist saved");
        }
    }
}