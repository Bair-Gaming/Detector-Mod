using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using LethalLib.Modules;
using DunGen;
using System;
using Detector.Behaviours;

namespace Detector
{
    [BepInPlugin(GUID, NAME, PLUGIN)]
    
    public class Plugin : BaseUnityPlugin
    {
        const string GUID = "Detector";
        const string NAME = "Detector";
        const string PLUGIN = "1.0.2";

        public static Plugin instance;
        public void Awake()
        { 
            if(instance != null)
                instance = this;

            string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "detector-mod");
            Logger.LogInfo(bundlePath);

            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);

            Item detectorItem = bundle.LoadAsset<Item>("assets/detector/detector-item.asset");
            detectorItem.creditsWorth = 10;

            NetworkPrefabs.RegisterNetworkPrefab(detectorItem.spawnPrefab);
            Utilities.FixMixerGroups(detectorItem.spawnPrefab);

            DetectorItem script = detectorItem.spawnPrefab.AddComponent<DetectorItem>();
            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = detectorItem;
            
            Items.RegisterScrap(detectorItem, 25, Levels.LevelTypes.All);
            
            TerminalNode node = ScriptableObject.CreateInstance<TerminalNode>();
            node.clearPreviousText = true;
            node.displayText = "info here\n\n";

            Items.RegisterShopItem(detectorItem, null, null, node, 35);


            // Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginInfo.PLUGIN_GUID);
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
    }
}
