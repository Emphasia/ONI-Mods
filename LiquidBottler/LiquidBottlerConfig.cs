using System;
using TUNING;
using UnityEngine;

public class LiquidBottlerConfig : IBuildingConfig
{
    public override BuildingDef CreateBuildingDef()
    {
        string id = "LIQUIDBOTTLER";
        int width = 3;
        int height = 2;
        string anim = "gas_bottler_kanim";
        int hitpoints = 30;
        float construction_time = 30f;
        float[] tier = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
        string[] raw_METALS = MATERIALS.ALL_METALS;
        float melting_point = 800f;
        BuildLocationRule buildLocationRule = BuildLocationRule.OnFloor;
        BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, width, height, anim, hitpoints, construction_time, tier, raw_METALS, melting_point, buildLocationRule, BUILDINGS.DECOR.PENALTY.TIER1, NOISE_POLLUTION.NOISY.TIER0, 0.2f);
        buildingDef.InputConduitType = ConduitType.Liquid;
        buildingDef.UtilityInputOffset = new CellOffset(0, 0);
        buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
        buildingDef.Floodable = false;
        buildingDef.AudioCategory = "HollowMetal";
        return buildingDef;
    }

    public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
    {
        Storage storage = BuildingTemplates.CreateDefaultStorage(go, true);
        storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
        storage.showDescriptor = true;
        storage.capacityKg = 350f;
        storage.allowItemRemoval = true;
        storage.showInUI = true;
        go.AddOrGet<DropAllWorkable>();
        ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
        conduitConsumer.conduitType = ConduitType.Liquid;
        conduitConsumer.ignoreMinMassCheck = true;
        conduitConsumer.forceAlwaysSatisfied = true;
        conduitConsumer.alwaysConsume = true;
        conduitConsumer.capacityKG = 1000f;
        conduitConsumer.keepZeroMassObject = false;
    }

    public override void DoPostConfigureComplete(GameObject go)
    {
        BuildingTemplates.DoPostConfigure(go);
    }

    public const string ID = "LIQUIDBOTTLER";
}
