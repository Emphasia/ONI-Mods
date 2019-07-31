using System;
using System.Collections.Generic;
using Database;
using Harmony;

namespace LiquidBottlerPatches
{
    public class LiquidBottlerPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class LiquidBottlerBuildingsPatch
        {
            private static void Prefix()
            {
                Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.LIQUIDBOTTLER.NAME",
                    "Liquid Bottler"
                });
                Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.LIQUIDBOTTLER.DESC",
                    "Allow Duplicants to fetch bottled liquids for delivery to buildings." //"This bottler station has access to: {Liquids}"
				});
                Strings.Add(new string[] {
                    "STRINGS.BUILDINGS.PREFABS.LIQUIDBOTTLER.EFFECT",
                    "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport." //"Liquid Available: {Liquids}"
				});
                ModUtil.AddBuildingToPlanScreen("Plumbing", "LIQUIDBOTTLER");
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public class LiquidBottlerDbPatch
        {
            private static void Prefix()
            {
                List<string> list = new List<string>(Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
                list.Add("LIQUIDBOTTLER");
                Techs.TECH_GROUPING["ImprovedLiquidPiping"] = list.ToArray();
            }
        }
    }
}
