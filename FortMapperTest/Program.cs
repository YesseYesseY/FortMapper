using FortMapper;
using Newtonsoft.Json;

GlobalProvider.Init();

#if true
LootExport.Yes(
     // "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/FigmentLootTierData.FigmentLootTierData",
     // "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/FigmentLootPackages.FigmentLootPackages"
     "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LTD_Figment.NoBuild_Composite_LTD_Figment",
     "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LP_Figment.NoBuild_Composite_LP_Figment"
).Export(true);
#endif

#if true

WorldExport.JsonFormatting = Formatting.Indented;
WorldExport.OutputActorClasses = true;
WorldExport.ActorsToExport.AddRange("Tiered_Chest_6_Figment_C", "Tiered_Ammo_Figment_C");

var yes = WorldExport.Yes("FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_Map/Content/Athena_Terrain_S03.Athena_Terrain_S03",
    "FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_MapUI/Content/MiniMapAthena_S03.MiniMapAthena_S03");

if (yes is null || yes.MinimapTexture is null)
{
    Console.WriteLine(":(");
    return;
}
yes.Export(true);
#endif