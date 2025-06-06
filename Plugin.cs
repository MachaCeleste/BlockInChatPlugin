using AssetBundleTools;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace BlockInChatPlugin;

[BepInPlugin("com.machaceleste.blockinchatplugin", MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        BundleTool.LoadBundle();
        var harmony = new Harmony("com.machaceleste.blockinchatplugin");
        harmony.PatchAll();
    }
}