using System.Text.RegularExpressions;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.UObject;
using FortMapper;
using Newtonsoft.Json;

GlobalProvider.Init();

#if true

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

#if false // BR
    ("FortniteGame/Content/Athena/Playlists/AthenaCompositeLTD.AthenaCompositeLTD",
     "FortniteGame/Plugins/GameFeatures/BRPlaylists/Content/Athena/Playlists/AthenaCompositeLP.AthenaCompositeLP"),
    ("FortniteGame/Plugins/GameFeatures/LootCurrentSeason/Content/DataTables/LootCurrentSeasonLootTierData_Client.LootCurrentSeasonLootTierData_Client",
     "FortniteGame/Plugins/GameFeatures/LootCurrentSeason/Content/DataTables/LootCurrentSeasonLootPackages_Client.LootCurrentSeasonLootPackages_Client"),
    ("FortniteGame/Plugins/GameFeatures/JethroLoot/Content/DataTables/JethroLootTierData.JethroLootTierData",
     "FortniteGame/Plugins/GameFeatures/JethroLoot/Content/DataTables/JethroLootPackages.JethroLootPackages")
#endif

).Export(true);
#endif

#if false

WorldExport.JsonFormatting = Formatting.Indented;
WorldExport.OutputActorClasses = true;
WorldExport.ActorsToExport.AddRange(
    "Tiered_Chest_6_Figment_C", "Tiered_Ammo_Figment_C",
    "B_BGA_Athena_EnvCampFire_C",
    "B_Athena_VendingMachine_Figment_C",
    "BGA_Athena_SCMachine_Figment_C"
    );

var yes = WorldExport.Yes("FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_Map/Content/Athena_Terrain_S03.Athena_Terrain_S03",
    "FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_MapUI/Content/MiniMapAthena_S03.MiniMapAthena_S03");

if (yes is null || yes.MinimapTexture is null)
{
    Console.WriteLine(":(");
    return;
}
yes.Export(true);

ExportUtils.ExportTexture2D("FortniteGame/Plugins/GameFeatures/Creative/Devices/CRD_RebootVan/Content/SetupAssets/ID/T_Icon_PS_CP_Device_RebootVan.T_Icon_PS_CP_Device_RebootVan", "./World/BGA_Athena_SCMachine_Figment_C.png");

#endif