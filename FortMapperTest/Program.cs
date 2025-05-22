using System.Text.RegularExpressions;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.UObject;
using FortMapper;
using Newtonsoft.Json;

GlobalProvider.Init();

#if false

LootExport.Hotfixes = @"
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaFloorLoot_07;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaTreasure_05;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaTreasure_13;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaSupplyDrop_08;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.SMG.07;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.SMG.08;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSMG.09;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSMG.10;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSMG.11;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaFloorLoot_08;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaTreasure_07;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootTierData;RowUpdate;Loot_AthenaSupplyDrop_05;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.01;Weight;1.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.02;Weight;0.400000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.03;Weight;0.070000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.06;Weight;1.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.07;Weight;0.400000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.08;Weight;0.070000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.02;Weight;0.400000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.03;Weight;0.070000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.07;Weight;0.016000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.11;Weight;0.016000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.15;Weight;0.070000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HandCannon.01;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HandCannon.02;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandCannon.01;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandCannon.02;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Handgun.05;Weight;0.600000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Handgun.06;Weight;0.400000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaHighConsumables.06;Weight;0.000000
+DataTable=/Figment_LootTables/DataTables/FigmentLootPackages;RowUpdate;WorldList.AthenaConsumables.06;Weight;0.000000
";

LootExport.Yes(
#if true // OG ZB
    ("FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LTD_Figment.NoBuild_Composite_LTD_Figment",
     "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LP_Figment.NoBuild_Composite_LP_Figment")
#endif
    //"FortniteGame/Plugins/GameFeatures/Figment/FigmentPlaylists/Content/Playlists/Playlist_FigmentNoBuildSolo.Playlist_FigmentNoBuildSolo"
//"FortniteGame/Plugins/GameFeatures/BRPlaylists/Content/Athena/Playlists/Playlist_DefaultSolo.Playlist_DefaultSolo"

#if false // BR
    ("FortniteGame/Content/Athena/Playlists/AthenaCompositeLTD.AthenaCompositeLTD",
     "FortniteGame/Plugins/GameFeatures/BRPlaylists/Content/Athena/Playlists/AthenaCompositeLP.AthenaCompositeLP"),
    ("FortniteGame/Plugins/GameFeatures/LootCurrentSeason/Content/DataTables/LootCurrentSeasonLootTierData_Client.LootCurrentSeasonLootTierData_Client",
     "FortniteGame/Plugins/GameFeatures/LootCurrentSeaszhroLootPackages.JethroLootPackages")
#endif

).Export(true);
#endif

#if true
foreach (var thing in new (string, string, string)[]
{
    ("FortniteGame/Plugins/GameFeatures/BRMapCh6/Content/Maps/Hermes_Terrain.Hermes_Terrain",
     "FortniteGame/Content/Athena/Apollo/Maps/UI/Apollo_Terrain_Minimap.Apollo_Terrain_Minimap",
     "Hermes"),
    ("FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_Map/Content/Athena_Terrain_S03.Athena_Terrain_S03",
     "FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_MapUI/Content/MiniMapAthena_S03.MiniMapAthena_S03",
     "Figment"),
    ("FortniteGame/Plugins/GameFeatures/f4032749-42c4-7fe9-7fa2-c78076f34f54/Content/DashBerry.DashBerry",
     "FortniteGame/Plugins/GameFeatures/BlastBerryMapUI/Content/Minimap/Discovered_DashBerry.Discovered_DashBerry",
     "Slurp Rush"),
    ("FortniteGame/Plugins/GameFeatures/632de27e-4506-41f8-532f-93ac01dc10ca/Content/Maps/PunchBerry_Terrain.PunchBerry_Terrain",
     "FortniteGame/Plugins/GameFeatures/BlastBerryMapUI/Content/Minimap/Discovered_PunchBerry.Discovered_PunchBerry",
     "Oasis"),
    ("FortniteGame/Plugins/GameFeatures/fd242d06-46d5-d389-1a48-2fb3bb65c2a1/Content/Maps/BlastBerry_Terrain.BlastBerry_Terrain",
     "FortniteGame/Plugins/GameFeatures/BlastBerryMapUI/Content/Minimap/Capture_Iteration_Discovered_BlastBerry.Capture_Iteration_Discovered_BlastBerry",
     "Venture")
})
{
    var wexport = new WorldExport()
    {
        JsonFormatting = Formatting.Indented,
        OutputActorClasses = true,
        ActorsToExport =
    {
        "Tiered_Chest_6_Figment_C",
        "Tiered_Ammo_Figment_C",
        "B_BGA_Athena_EnvCampFire_C",
        "B_Athena_VendingMachine_Figment_C",
        "BGA_Athena_SCMachine_Figment_C",
        "Tiered_Chest_Athena_C",
        "Tiered_Ammo_Athena_C",
        "Tiered_Chest_Sunflower_C"
    }
    };
    var world = GlobalProvider.LoadPackageObject<UWorld>(thing.Item1);
    var minimap = GlobalProvider.LoadPackageObject<UTexture2D>(thing.Item2);
    wexport.Parse(world, minimap);
    wexport.Export(thing.Item3);
}
ExportUtils.ExportTexture2D("FortniteGame/Plugins/GameFeatures/Creative/Devices/CRD_RebootVan/Content/SetupAssets/ID/T_Icon_PS_CP_Device_RebootVan.T_Icon_PS_CP_Device_RebootVan", "./World/Images/BGA_Athena_SCMachine_Figment_C.png");
#endif