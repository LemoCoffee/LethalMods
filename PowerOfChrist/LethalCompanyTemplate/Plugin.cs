using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using HarmonyLib;
using LCSoundTool;
using UnityEngine;

namespace PowerOfChrist
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    [BepInDependency("LCSoundTool")]
    class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "LemoCoffee.LethalCompany.PowerOfChrist";
        public const string pluginName = "Power Of Christ";
        public const string pluginVersion = "1.0.4";

        internal static ManualLogSource Logger { get; private set; }
        internal static Harmony HarmonyInstance { get; } = new Harmony(pluginGuid);
        internal static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

        internal static AudioClip dangerSound;
        internal static AudioSource audioSource;
        internal static float volume;

        internal static HudFlasher hudFlasher;
        internal static Texture2D[] hudImages;
        internal static bool flashOnce;
        internal static bool allowOverlap;
        internal static int triggerValue;
        internal static float flashDelay;

        public void Awake()
        {
            Plugin.Logger = base.Logger;

            string soundName = "";
            Configure(ref soundName);

            HarmonyInstance.PatchAll(ExecutingAssembly);

            dangerSound = SoundTool.GetAudioClip("LemoCoffee-PowerOfChrist", soundName);

            Plugin.Logger.LogInfo("Power of Christ is compelling with V" + pluginVersion);
        }

        private void Configure(ref string soundName)
        {
            // Audio
            volume       = Config.Bind("Audio", "Volume", 1f).Value;
            soundName    = Config.Bind("Audio", "File Name", "sound.wav").Value;

            // General
            flashOnce    = Config.Bind("General", "Flash Once", false).Value;
            allowOverlap = Config.Bind("General", "Allow Overlap", false).Value;
            triggerValue = Config.Bind("General", "Trigger Point", 20).Value;
            flashDelay   = Config.Bind("General", "Flash Delay", 5f).Value;
        }
    }
}
