using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace LethalInjection
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid = "LemoCoffee.LethalCompany.LethalInjection";
        public const string pluginName = "Lethal Injection";
        public const string pluginVersion = "0.0.3";

        public static ManualLogSource Logger { get; private set; }
        public static Harmony HarmonyInstance { get; } = new Harmony(pluginGuid);
        public static Assembly ExecutingAssembly { get; } = Assembly.GetExecutingAssembly();

        public void Awake()
        {
            Plugin.Logger = base.Logger;

            HarmonyInstance.PatchAll(ExecutingAssembly);

            Plugin.Logger.LogInfo("Lethal Injection is primed with V"+pluginVersion);
        }
    }
}
