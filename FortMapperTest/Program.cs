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

+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList_Market_ShockGrenade.01;Weight;1.000000
+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList.AthenaHighConsumables.31;Weight;0.330000
+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList.AthenaHighConsumablesRare.09;Weight;0.400000
+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList.AthenaSupplyDropConsumables.28;Weight;0.400000
+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList.AthenaConsumables.40;Weight;0.300000
+DataTable=/LootCurrentSeason/DataTables/LootCurrentSeasonLootPackages_Client;RowUpdate;WorldList.Consumable.Mobility.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.01;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.02;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.03;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.04;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.05;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.06;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.07;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.08;Weight;0.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.09;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.10;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.11;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.01;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.02;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.03;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.04;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.05;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.06;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.07;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.08;Weight;0.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.09;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.10;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.11;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Rocket.02;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighRocket.02;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighRocket.03;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighRocket.04;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.Rocket.01;Weight;0.600000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.Rocket.02;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Rare.14;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.14;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.14;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.54;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.55;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.56;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.MMO.TrainCache.WepSetOne.09;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Heavy.03;Weight;0.160000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Crossbow.Jethro.GuineaPig.R.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Crossbow.Jethro.GuineaPig.VR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Crossbow.Jethro.GuineaPig.SR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Shotgun.04;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Shotgun.05;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Shotgun.06;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighShotgun.05;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighShotgun.06;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighShotgun.07;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighShotgun.08;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.HighShotgun.03;Weight;0.600000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.HighShotgun.04;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Rare.12;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.12;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.12;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.45;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.46;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.47;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.48;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.MMO.TrainCache.WepSetOne.07;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Shotgun.05;Weight;0.750000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Shotgun.06;Weight;0.250000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Shell.05;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Shell.06;Weight;0.160000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Guard.Heavy.Elite.Weap.01;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;JobBoardItemList.Armory.Athena.Shotgun.03;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.UC.03;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.R.03;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.VR.03;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.SR.03;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.Jethro.ChillyHammer.Athena.C.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.Jethro.ChillyHammer.Athena.UC.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.Jethro.ChillyHammer.Athena.VR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Shotgun.Jethro.ChillyHammer.Athena.SR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Sniper.02;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSniper.02;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSniper.03;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighSniper.04;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.Sniper.01;Weight;0.600000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.Sniper.02;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.08;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.08;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.30;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.31;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.32;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.MMO.TrainCache.WepSetOne.13;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Sniper.01;Weight;0.750000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Sniper.02;Weight;0.250000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Heavy.01;Weight;0.160000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Sniper.R.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Sniper.VR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Sniper.SR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Sniper.Cosmos.Athena.R.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Sniper.Cosmos.Athena.SR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.04;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.05;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.Handgun.06;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.05;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.06;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.07;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaLoot.Weapon.HighHandgun.08;Weight;0.004000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.HighHandgun.03;Weight;0.600000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.ApolloLoot.Weapon.HighHandgun.04;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Rare.13;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Epic.13;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.Weapons.Rarity.Legendary.13;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.49;Weight;0.400000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.50;Weight;0.070000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.51;Weight;0.016000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.GoldenPOI.DroneLoot.Wep.52;Weight;0.00400
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.MMO.TrainCache.WepSetOne.08;Weight;0.500000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Handgun.03;Weight;0.750000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDrop.Weapon.Handgun.04;Weight;0.250000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Light.03;Weight;0.40000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.FlopperHigh.Light.04;Weight;0.160000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;JobBoardItemList.Armory.Athena.Pistol.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Pistol.UC.02;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Pistol.R.02;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Pistol.VR.02;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Pistol.SR.02;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Jethro.ClearShell.Athena.UC.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Jethro.ClearShell.Athena.R.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;NPCItemList.Jethro.ClearShell.Athena.SR.01;Weight;1.000000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaHighConsumables.01;Weight;0.200000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaHighConsumablesRare.01;Weight;0.250000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaSupplyDropConsumables.01;Weight;0.250000
+DataTable=/JethroLoot/DataTables/JethroLootPackages;RowUpdate;WorldList.AthenaConsumables.01;Weight;0.150000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaTreasure_02;Weight;0.190000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaFloorLoot_04;Weight;0.65000000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaTreasure_07;Weight;0.022000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_ApolloTreasure_Rare_05;Weight;0.150000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaSupplyDrop_02;Weight;0.400000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaTreasure_04;Weight;0.044000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_ApolloTreasure_Rare_04;Weight;0.150000
+DataTable=/Game/Items/Datatables/AthenaLootTierData_Client;RowUpdate;Loot_AthenaSupplyDrop_02;Weight;0.400000
";

LootExport.Yes(
#if false // OG ZB
    ("FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LTD_Figment.NoBuild_Composite_LTD_Figment",
     "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/NoBuild/NoBuild_Composite_LP_Figment.NoBuild_Composite_LP_Figment")
#endif
    //"FortniteGame/Plugins/GameFeatures/Figment/FigmentPlaylists/Content/Playlists/Playlist_FigmentNoBuildSolo.Playlist_FigmentNoBuildSolo"
    //"FortniteGame/Plugins/GameFeatures/BRPlaylists/Content/Athena/Playlists/Playlist_DefaultSolo.Playlist_DefaultSolo"

#if true // BR
    ("FortniteGame/Content/Athena/Playlists/AthenaCompositeLTD.AthenaCompositeLTD",
     "FortniteGame/Plugins/GameFeatures/BRPlaylists/Content/Athena/Playlists/AthenaCompositeLP.AthenaCompositeLP"),
    ("FortniteGame/Plugins/GameFeatures/LootCurrentSeason/Content/DataTables/LootCurrentSeasonLootTierData_Client.LootCurrentSeasonLootTierData_Client",
     "FortniteGame/Plugins/GameFeatures/LootCurrentSeason/Content/DataTables/LootCurrentSeasonLootPackages_Client.LootCurrentSeasonLootPackages_Client"),
    ("FortniteGame/Plugins/GameFeatures/JethroLoot/Content/DataTables/JethroLootTierData.JethroLootTierData",
     "FortniteGame/Plugins/GameFeatures/JethroLoot/Content/DataTables/JethroLootPackages.JethroLootPackages")
#endif

).Export(true);
#endif

#if true
List<Task> tasks = new();
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
    tasks.Add(Task.Run(() =>
    {
        var wexport = new WorldExport()
        {
            JsonFormatting = Formatting.Indented,
            OutputActorClasses = true
        };
        var world = GlobalProvider.LoadPackageObject<UWorld>(thing.Item1);
        var minimap = GlobalProvider.LoadPackageObject<UTexture2D>(thing.Item2);
        wexport.Parse(world, minimap);
        wexport.Export(thing.Item3);
    }));
}
Task.WaitAll(tasks);
#endif