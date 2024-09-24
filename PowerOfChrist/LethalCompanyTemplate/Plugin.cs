﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
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
        public const string pluginVersion = "0.0.2";

        internal static ManualLogSource Logger { get; private set; }
        internal static Harmony HarmonyInstance { get; } = new Harmony(pluginGuid);
        internal static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

        internal static AudioClip dangerSound;
        internal static AudioSource audioSource;
        internal static HudFlasher hudFlasher;
        internal static Texture2D[] hudImages;

        public void Awake()
        {
            Plugin.Logger = base.Logger;

            HarmonyInstance.PatchAll(ExecutingAssembly);

            dangerSound = SoundTool.GetAudioClip("LemoCoffee-PowerOfChrist","sound.wav");

            Plugin.Logger.LogInfo("Power of Christ is compelling with V" + pluginVersion);
        }        
    }
}
