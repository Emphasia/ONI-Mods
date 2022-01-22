using System;
using TUNING;
using UnityEngine;

namespace LiquidBottler
{
    public class LiquidBottlerConfig : IBuildingConfig
    {
        public const string ID = "LIQUIDBOTTLER";

        public override BuildingDef CreateBuildingDef()
        {
            int width = 3;
            int height = 2;
            string anim = "gas_bottler_kanim";
            //string anim = "liquidbottler_kanim";
            int hitpoints = BUILDINGS.HITPOINTS.TIER1;
            float construction_time = BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER2;
            float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
            string[] raw_METALS = MATERIALS.ALL_METALS;
            float melting_point = BUILDINGS.MELTING_POINT_KELVIN.TIER0;
            BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
            EffectorValues decorPenalty = BUILDINGS.DECOR.PENALTY.TIER1;
            EffectorValues noisePollution = NOISE_POLLUTION.NOISY.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, width, height, anim, hitpoints, construction_time, tier, raw_METALS, melting_point, buildLocationRule, decorPenalty, noisePollution, 0.2f);
            buildingDef.InputConduitType = ConduitType.Liquid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
            buildingDef.Floodable = false;
            buildingDef.Entombable = true;
            buildingDef.AudioCategory = "HollowMetal";
            buildingDef.DefaultAnimState = "on";
            GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS;
            storage.capacityKg = 350f;
            storage.allowItemRemoval = true;
            storage.showInUI = true;
            go.AddOrGet<DropAllWorkable>();
            LiquidBottler liquidBottler = go.AddOrGet<LiquidBottler>();
            liquidBottler.storage = storage;
            liquidBottler.workTime = 9f;
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.storage = storage;
            conduitConsumer.conduitType = ConduitType.Liquid;
            conduitConsumer.ignoreMinMassCheck = true;
            //conduitConsumer.forceAlwaysSatisfied = true;
            //conduitConsumer.alwaysConsume = true;
            conduitConsumer.capacityKG = storage.capacityKg;
            conduitConsumer.keepZeroMassObject = false;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            BuildingTemplates.DoPostConfigure(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
        }
    }
}
