using System;
using System.Collections.Generic;
using KMod;
using static Localization;
using HarmonyLib;

namespace LiquidBottler
{
    public class LiquidBottlerMod : UserMod2
    {
        public static UserMod2 _mod;

        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            _mod = this;
        }

        [HarmonyPatch(typeof(GeneratedBuildings), "LoadGeneratedBuildings")]
        public class LiquidBottlerBuildingsPatch
        {
            private static void Prefix()
            {
                // Strings.Add()

                ModUtil.AddBuildingToPlanScreen("Plumbing", LiquidBottlerConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Db), "Initialize")]
        public class LiquidBottlerDbPatch
        {
            private static void Postfix()
            {
                Db.Get().Techs.Get("ImprovedLiquidPiping").unlockedItemIDs.Add(LiquidBottlerConfig.ID);
            }
        }

        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class LiquidBottlerLocalizationPatch
        {
            public static void Postfix()
            {
                Dictionary<string, string> translations;
                string file_po = System.IO.Path.Combine(_mod.path, "translations", GetLocale()?.Code + ".po");
                if (System.IO.File.Exists(file_po))
                {
                    translations = LoadStringsFile(file_po, false);
                }
                else
                {
                    file_po = System.IO.Path.Combine(_mod.path, "translations", "zh" + ".po");
                    translations = LoadStringsFile(file_po, true);
                }
                // OverloadStrings(translations);
                foreach (KeyValuePair<string, string> translation in translations)
                {
                    Strings.Add(translation.Key, translation.Value);
                }
            }
        }
    }
}
