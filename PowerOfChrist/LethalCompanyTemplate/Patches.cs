using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using System.IO;

namespace PowerOfChrist
{
    [HarmonyPatch]
    class Patches
    {
        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "Awake")]
        [HarmonyPrefix]
        internal static void PlayerControllerB_Awake_Prefix(ref GameNetcodeStuff.PlayerControllerB __instance)
        {
            try
            {
                if (Plugin.hudFlasher == null)
                {
                    Plugin.hudFlasher = GetOrAddHUDObject().GetComponent<HudFlasher>();
                    Plugin.hudFlasher.textures = CreateTexturesFromFolder(Path.Combine(Path.GetDirectoryName(Plugin.ExecutingAssembly.Location), "images"));
                    Plugin.Logger.LogInfo("Created flasher with " + Plugin.hudFlasher.textures.Length + " textures");
                }

                if (__instance == GameNetworkManager.Instance.localPlayerController)
                {
                    Plugin.hudFlasher.BeginFlash();
                }
            }
            catch (Exception e) 
            {
                Plugin.Logger.LogError(e);
            }
            
        }

        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "DamagePlayer")]
        [HarmonyPostfix]
        internal static void PlayerControllerB_DamagePlayer_Postfix(ref GameNetcodeStuff.PlayerControllerB __instance, int damageNumber, bool hasDamageSFX, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
        {
            if (__instance == GameNetworkManager.Instance.localPlayerController)
            {
                Plugin.hudFlasher.BeginFlash();
            }
        }

        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "KillPlayer")]
        [HarmonyPostfix]
        internal static void PlayerControllerB_KillPlayer_Postfix(ref GameNetcodeStuff.PlayerControllerB __instance, Vector3 bodyVelocity, bool spawnBody = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, Vector3 positionOffset = default(Vector3))
        {
            StopFlasher();
        }

        [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
        [HarmonyPostfix]
        internal static void StartOfRound_ReviveDeadPlayers_Postfix()
        {
            StopFlasher();
        }

        static GameObject GetOrAddHUDObject()
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject rawImageObj = new GameObject("ImageFlasher", typeof(UnityEngine.UI.RawImage), typeof(HudFlasher));

            rawImageObj.transform.SetParent(canvas.transform);

            return rawImageObj;
        }

        static Texture2D LoadImageInFolder(string path)
        {
            Texture2D texture = new Texture2D(2, 2);
            byte[] bytes = File.ReadAllBytes(path);
            if (!ImageConversion.LoadImage(texture, bytes))
            {
                Plugin.Logger.LogWarning(path + " failed to be loaded");
            }
            else
            {
                Plugin.Logger.LogInfo(path + " successfully loaded");
            }

            return texture;
        }

        static Texture2D[] CreateTexturesFromFolder(string path)
        {
            string[] imagePaths = Directory.GetFiles(path, "*.png");
            Texture2D[] textures = new Texture2D[imagePaths.Length];

            for (int i = 0; i < imagePaths.Length; i++)
            {
                textures[i] = LoadImageInFolder(imagePaths[i]);
            }

            return textures;
        }

        static void StopFlasher ()
        {
            if (Plugin.hudFlasher != null)
            {
                Plugin.hudFlasher.StopFlash();
            }
        }
    }    
}
