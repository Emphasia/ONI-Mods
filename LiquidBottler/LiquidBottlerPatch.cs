using System;
using System.Collections.Generic;
using Database;
using HarmonyLib;

namespace LiquidBottler
{
    public class LiquidBottlerPatch
    {
        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class LiquidBottlerBuildingsPatch
        {
            private static void Prefix()
            {
                Strings.Add(new string[] {
                    string.Format("STRINGS.BUILDINGS.PREFABS.{0}.NAME", LiquidBottlerConfig.ID),
                    "Liquid Bottler"
                });
                Strings.Add(new string[] {
                    string.Format("STRINGS.BUILDINGS.PREFABS.{0}.DESC", LiquidBottlerConfig.ID),
                    "Allow Duplicants to fetch bottled liquids for delivery to buildings." //"This bottler station has access to: {Liquids}"
				});
                Strings.Add(new string[] {
                    string.Format("STRINGS.BUILDINGS.PREFABS.{0}.EFFECT", LiquidBottlerConfig.ID),
                    "Automatically stores piped <link=\"ELEMENTSLIQUID\">Liquid</link> into bottles for manual transport." //"Liquid Available: {Liquids}"
				});
                ModUtil.AddBuildingToPlanScreen("Plumbing", LiquidBottlerConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public class LiquidBottlerDbPatch
        {
            // private static void Prefix()
            // {
            //     List<string> list = new List<string>(Techs.TECH_GROUPING["ImprovedLiquidPiping"]);
            //     list.Add(LiquidBottlerConfig.ID);
            //     Techs.TECH_GROUPING["ImprovedLiquidPiping"] = list.ToArray();
            // }
            private static void Postfix()
            {
                Db.Get().Techs.Get("ImprovedLiquidPiping").unlockedItemIDs.Add(LiquidBottlerConfig.ID);
            }
        }
    }
}
