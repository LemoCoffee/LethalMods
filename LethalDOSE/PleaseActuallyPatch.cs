using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
using HarmonyLib;

namespace LethalInjection
{
    [HarmonyPatch]
    internal class PleaseActuallyPatch
    {
        [HarmonyPatch(typeof(StartOfRound),"Awake")]
        [HarmonyPrefix]
        static void StartOfRound_Awake_Prefix(ref StartOfRound __instance)
        {
            // Thank you to LethalLevelLoader for being a reference here
            RoundManager rm = UnityEngine.Object.FindFirstObjectByType<RoundManager>();

            DebugMethod(rm);
            
            string interiorsString = "";
            foreach (IndoorMapType interior in rm.dungeonFlowTypes)
            {
                interiorsString += "\n" + FormatFlowToString(interior);
            }

            Plugin.Logger.LogInfo("========= Dungeon Flows from StartOfRound ========" + interiorsString);
        }

        static string FormatFlowToString(IndoorMapType interior)
        {
            string output = interior.ToString();

            DunGen.DungeonArchetype[] discoveredArchetypes = interior.dungeonFlow.GetUsedArchetypes();
            foreach (DunGen.DungeonArchetype i in discoveredArchetypes)
            {
                output += "\n\t- " + i.name;

                foreach (DunGen.TileSet j in i.TileSets)
                {
                    output += "\n\t\t > " + j.name;

                    foreach (DunGen.GameObjectChance k in j.TileWeights.Weights)
                    {
                        output += $"\n\t\t\t > {k.Value.name} ({k.MainPathWeight})({k.BranchPathWeight})";
                    }
                }
            }

            return output;
        }

        static void DebugMethod(RoundManager rm)
        {
            List<DunGen.GameObjectChance> tiles = rm.dungeonFlowTypes[0].dungeonFlow.GetUsedArchetypes()[0].TileSets[0].TileWeights.Weights;
            //tiles.RemoveRange(1, 7);

            DunGen.GameObjectChance newTile = rm.dungeonFlowTypes[1].dungeonFlow.GetUsedArchetypes()[1].TileSets[1].TileWeights.Weights[3];
            /*DunGen.GameObjectChance newTile = CloneGOC(tiles[1]);
            string AssetLocation = Path.Combine(Path.GetDirectoryName(Plugin.ExecutingAssembly.Location), "Assets");
            AssetBundle Assets = AssetBundle.LoadFromFile(Path.Combine(AssetLocation, "catwalklighttile2x1"));
            if (Assets == null)
            {
                Plugin.Logger.LogError("Failed to load custom assets.");
                return;
            }

            GameObject[] loadedAssets = Assets.LoadAllAssets<GameObject>();
            Plugin.Logger.LogInfo("Successfully loaded " + loadedAssets.Length + " assets");
            
            newTile.Value = loadedAssets[0];
            
            /*foreach (UnityEngine.Object i in loadedAssets)
            {
                Plugin.Logger.LogInfo(i);
            }*/

            //Plugin.Logger.LogInfo(Assets.LoadAsset<GameObject>("Assets/LethalCompany/Game/Prefabs/Tile/CatwalkLightTile2x1"));

            tiles.Add(newTile);
        }

        static DunGen.GameObjectChance CloneGOC (DunGen.GameObjectChance x)
        {
            return x;
        }
    }
}
