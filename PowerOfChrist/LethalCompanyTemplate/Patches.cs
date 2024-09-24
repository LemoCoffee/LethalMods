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
        /// <summary>
        /// Prepares the Hud Flasher for this game upon the creation of the player
        /// </summary>
        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        internal static void PlayerControllerB_Awake_Postfix(ref GameNetcodeStuff.PlayerControllerB __instance)
        {
            try
            {
                // If we haven't assigned the hud flasher yet, create and reference it
                // NOTE: The mod doesn't account for the lobby restarting, and will attempt to access a deleted GameObject due to that
                if (Plugin.hudFlasher == null)
                {
                    Plugin.hudFlasher = GetOrAddHUDObject().GetComponent<HudFlasher>();
                    Plugin.hudFlasher.textures = CreateTexturesFromFolder(Path.Combine(Path.GetDirectoryName(Plugin.ExecutingAssembly.Location), "images"));
                    Plugin.Logger.LogInfo("Created flasher with " + Plugin.hudFlasher.textures.Length + " textures");
                }

                // Attempt a flash sequence to fully initialize the flasher object
                BeginFlasher();
            }
            catch (Exception e) 
            {
                Plugin.Logger.LogError(e);
            }
        }

        // Trigger the flash check upon taking damage
        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "DamagePlayer")]
        [HarmonyPostfix]
        internal static void PlayerControllerB_DamagePlayer_Postfix(ref GameNetcodeStuff.PlayerControllerB __instance, int damageNumber, bool hasDamageSFX, bool callRPC, CauseOfDeath causeOfDeath, int deathAnimation, bool fallDamage, Vector3 force)
        {
            BeginFlasher();
        }

        // Stop the flash sequence upon death, the player is in the hands of the lord now :)
        [HarmonyPatch(typeof(GameNetcodeStuff.PlayerControllerB), "KillPlayer")]
        [HarmonyPostfix]
        internal static void PlayerControllerB_KillPlayer_Postfix(ref GameNetcodeStuff.PlayerControllerB __instance, Vector3 bodyVelocity, bool spawnBody = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, Vector3 positionOffset = default(Vector3))
        {
            StopFlasher();
        }

        // Stop the flash sequence from looping now that the player is fully restored. May be redundant
        [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
        [HarmonyPostfix]
        internal static void StartOfRound_ReviveDeadPlayers_Postfix()
        {
            StopFlasher();
        }

        /// <summary>
        /// Finds the ImageFlasher for this mod and, if it doesn't exist, creates one
        /// </summary>
        /// <returns>The GameObject with the component HudFlasher</returns>
        static GameObject GetOrAddHUDObject()
        {
            GameObject canvas = GameObject.Find("Canvas");
            GameObject rawImageObj = canvas.transform.find("ImageFlasher");

            if (rawImageObj != null)
            {
                rawImageObj = new GameObject("ImageFlasher", typeof(UnityEngine.UI.RawImage), typeof(HudFlasher));
                rawImageObj.transform.SetParent(canvas.transform);
            }

            return rawImageObj;
        }

        /// <summary>
        /// Creates a Texture2D from the image at the given path
        /// </summary>
        /// <param name="path">File name including file extension</param>
        /// <returns>Texture2D of the given image, returns null if image failed to load</returns>
        static Texture2D LoadImageInFolder(string path)
        {
            Texture2D texture = new Texture2D(2, 2);
            byte[] bytes = File.ReadAllBytes(path);
            if (!ImageConversion.LoadImage(texture, bytes))
            {
                Plugin.Logger.LogWarning(path + " failed to be loaded")
                return null;
            }

            Plugin.Logger.LogInfo(path + " successfully loaded");
            return texture;
        }

        /// <summary>
        /// Constructs a list of Texture2D out of image files in the given directory.
        /// Supports png and jpg.
        /// </summary>
        /// <param name="path">The absolute path of the folder containing the images</param>
        /// <returns>List of Texture2D's, images that failed to load will be null.</returns>
        static Texture2D[] CreateTexturesFromFolder(string path)
        {
            string[] imagePaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".png") || s.EndsWith(".jpg"));
            Texture2D[] textures = new Texture2D[imagePaths.Length];

            for (int i = 0; i < imagePaths.Length; i++)
            {
                textures[i] = LoadImageInFolder(imagePaths[i]);
            }

            return textures;
        }

        /// <summary>
        /// Attempts to stop the Hud Flashing sequence
        /// </summary>
        static bool StopFlasher ()
        {
            if (Plugin.hudFlasher != null)
            {
                Plugin.hudFlasher.StopFlash();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to begin Hud Flashing sequence
        /// </summary>
        static bool BeginFlasher ()
        {
            Plugin.hudFlasher.BeginFlash();
        }
    }    
}
