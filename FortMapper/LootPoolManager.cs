using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Readers;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;

namespace FortMapper
{
    public class FFortLootTierData
    {
        FStructFallback sfb;
        public FFortLootTierData(FStructFallback _sfb)
        {
            sfb = _sfb;
        }

        public FName LootPackage => sfb.Get<FName>("LootPackage");
        public float Weight => sfb.Get<float>("Weight");
    }
    public class FFortLootPackageData
    {
        FStructFallback sfb;
        public FFortLootPackageData(FStructFallback _sfb)
        {
            sfb = _sfb;
        }

        public FName LootPackageID => sfb.Get<FName>("LootPackageID");
        public float Weight => sfb.Get<float>("Weight");
        public string LootPackageCall => sfb.Get<string>("LootPackageCall");
        public FSoftObjectPath ItemDefinition => sfb.Get<FSoftObjectPath>("ItemDefinition");
        public TIntVector2<int> CountRange => sfb.Get<TIntVector2<int>>("CountRange");
    }

    public class LootPoolManager
    {
        Dictionary<string, List<FFortLootTierData>> ParsedLootTierData = new();
        Dictionary<string, List<FFortLootPackageData>> ParsedLootPackages = new();

        public LootPoolManager(UCompositeDataTable ltd, UCompositeDataTable lpd) :
            this(ltd.Get<UDataTable[]>("ParentTables"), lpd.Get<UDataTable[]>("ParentTables"))
        {
        }
        
        public LootPoolManager(UDataTable[] ltds, UDataTable[] lpds)
        {
            Dictionary<string, FStructFallback> TempLTD = new();
            Dictionary<string, FStructFallback> TempLPD = new();

            foreach (var ltd in ltds)
            {
                foreach (var row in ltd.RowMap)
                {
                    if (TempLTD.ContainsKey(row.Key.Text))
                        TempLTD[row.Key.Text] = row.Value;
                    else
                        TempLTD.Add(row.Key.Text, row.Value);
                }
            }

            foreach (var lpd in lpds)
            {
                foreach (var row in lpd.RowMap)
                {
                    if (TempLPD.ContainsKey(row.Key.Text))
                        TempLPD[row.Key.Text] = row.Value;
                    else
                        TempLPD.Add(row.Key.Text, row.Value);
                }
            }

            foreach (var row in TempLPD)
            {
                if (!row.Value.TryGetValue(out FName LootPackageID, "LootPackageID"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootPackages.ContainsKey(LootPackageID.Text))
                    ParsedLootPackages[LootPackageID.Text] = new();
                ParsedLootPackages[LootPackageID.Text].Add(new FFortLootPackageData(row.Value));
            }
            foreach (var row in TempLTD)
            {
                if (!row.Value.TryGetValue(out FName TierGroup, "TierGroup"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootTierData.ContainsKey(TierGroup.Text))
                    ParsedLootTierData[TierGroup.Text] = new();
                ParsedLootTierData[TierGroup.Text].Add(new FFortLootTierData(row.Value));
            }
        }
        /*
         * Okay something weird about this whole looting system ive noticed is that the ammo makes no sense
         * There's stuff you think makes no sense in the beginning like the PackageDrop being a float and being able to be for example 1.5f
         * But for the ammo it's the same always, Like for example shells.
         * Shells in BR and shells in Reload are the same, they shouldn't
         * Shells drop in 2 stacks in reload and 1 stack in BR, aka reload = 4 shells + 4 shell + shotgun. BR = 4 shells + shotgun
         * Also the ammo index (mostly 2nd in the list) is always set to 0.
         * Like in Loot_Athena_Treasure the LootPackageCategoryMinArray is [1,0,1,1,1, ...] WHY????
         * Except for in RELOAD the ammo is set to 1 "[1,1,1,1,1,0...]" ?????
         * The ONLY reason i could see where this makes sense is:
         * every thing is called 1 time but for some reason the ammo drops 1 time when set to 0 and 2 times when set to 1 but WHY would it be that
         * Im actually starting to go insane looking at this
         */
        public void Test(string TierGroupName)
        {
            float TotalWeight = 0.0f;
            List<KeyValuePair<string, float>> Weights = new();
            foreach (var TierData in ParsedLootTierData[TierGroupName])
            {
                if (TierData.Weight == 0.0f)
                    continue;
                
                Weights.Add(new(TierData.LootPackage.Text, TierData.Weight));
                TotalWeight += TierData.Weight;
            }

            foreach (var thing in Weights)
            {
                Console.WriteLine($"{thing.Key} ({(thing.Value / TotalWeight) * 100:0.00}%)");
                foreach (var thing2 in ParsedLootPackages[thing.Key])
                {
                    if (thing2.Weight != 1.0f) // TODO: :)
                        continue;
                    
                    Console.WriteLine($"\t{thing2.LootPackageCall}:");
                    float TotalWeight2 = 0.0f;
                    foreach (var thing3 in ParsedLootPackages[thing2.LootPackageCall])
                    {
                        if (thing3.Weight == 0.0f)
                            continue;
                        TotalWeight2 += thing3.Weight;
                    }
                    foreach (var thing3 in ParsedLootPackages[thing2.LootPackageCall])
                    {
                        if (thing3.Weight == 0.0f)
                            continue;

                        if (!thing3.ItemDefinition.TryLoad(out UObject ItemDef) ||
                            !ItemDef.TryGetValue(out FText ItemName, "ItemName"))
                            continue;

                        // TODO: CountRange thingy

                        var Rarity = ItemDef.GetOrDefault("Rarity", EFortRarity.Uncommon);

                        Console.Write($"\t\t{thing3.CountRange.X}x ");
                        
                        switch (Rarity)
                        {
                            case EFortRarity.Common:    Console.ForegroundColor = ConsoleColor.Gray; break;
                            case EFortRarity.Uncommon:  Console.ForegroundColor = ConsoleColor.Green; break;
                            case EFortRarity.Rare:      Console.ForegroundColor = ConsoleColor.Blue; break;
                            case EFortRarity.Epic:      Console.ForegroundColor = ConsoleColor.Magenta; break;
                            case EFortRarity.Legendary: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                            case EFortRarity.Mythic:    Console.ForegroundColor = ConsoleColor.Yellow; break;
                            case EFortRarity.Exotic:    Console.ForegroundColor = ConsoleColor.Cyan; break;
                            default: Console.WriteLine($"Unknown rarity: {Rarity}"); break;
                        }

                        Console.Write(ItemName.Text);
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($" ({(thing3.Weight / TotalWeight2) * 100:0.00}%)");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}