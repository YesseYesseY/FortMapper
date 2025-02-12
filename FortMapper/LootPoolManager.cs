using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.UObject;

namespace FortMapper
{
    public class LootPoolManager
    {
        Dictionary<string, List<FStructFallback>> ParsedLootTierData = new();
        Dictionary<string, List<FStructFallback>> ParsedLootPackages = new();

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
                    TempLPD[row.Key.Text] = row.Value;
                }
            }

            foreach (var row in TempLPD)
            {
                if (!row.Value.TryGetValue(out FName LootPackageID, "LootPackageID"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootPackages.ContainsKey(LootPackageID.Text))
                    ParsedLootPackages[LootPackageID.Text] = new();
                ParsedLootPackages[LootPackageID.Text].Add(row.Value);
            }
            foreach (var row in TempLTD)
            {
                if (!row.Value.TryGetValue(out FName TierGroup, "TierGroup"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootTierData.ContainsKey(TierGroup.Text))
                    ParsedLootTierData[TierGroup.Text] = new();
                ParsedLootTierData[TierGroup.Text].Add(row.Value);
            }
        }

        public void Test()
        {
            string TierGroupName = "Loot_AthenaTreasure";

            float TotalWeight = 0.0f;
            List<KeyValuePair<string, float>> Weights = new();
            foreach (var TierData in ParsedLootTierData[TierGroupName])
            {
                if (!TierData.TryGetValue(out FName LootPackage, "LootPackage") ||
                    !TierData.TryGetValue(out float Weight, "Weight") ||
                    Weight == 0.0f)
                    continue;

                Weights.Add(new(LootPackage.Text, Weight));
                TotalWeight += Weight;
            }

            foreach (var thing in Weights)
            {
                Console.WriteLine($"{thing.Key} ({(thing.Value / TotalWeight) * 100:0.00}%)");
                foreach (var thing2 in ParsedLootPackages[thing.Key])
                {
                    if (!thing2.TryGetValue(out float Weight, "Weight") ||
                        //Weight == 0.0f ||
                        Weight != 1.0f || // TODO: :)
                        !thing2.TryGetValue(out string LootPackageCall, "LootPackageCall")// ||
                        //!thing2.TryGetValue(out TIntVector2<float> Yes, "")
                        )
                        continue;
                    
                    Console.WriteLine($"    {LootPackageCall}:");
                    float TotalWeight2 = 0.0f;
                    foreach (var thing3 in ParsedLootPackages[LootPackageCall])
                    {
                        if (
                            !thing3.TryGetValue(out float Weight3, "Weight") ||
                            Weight3 == 0.0f)
                            continue;
                        TotalWeight2 += Weight3;
                    }
                    foreach (var thing3 in ParsedLootPackages[LootPackageCall])
                    {
                        if (
                            !thing3.TryGetValue(out float Weight3, "Weight") ||
                            Weight3 == 0.0f ||
                            !thing3.TryGetValue(out FSoftObjectPath _ItemDefinition, "ItemDefinition") ||
                            !_ItemDefinition.TryLoad(out UObject ItemDef) ||
                            !ItemDef.TryGetValue(out FText ItemName, "ItemName") ||
                            !thing3.TryGetValue(out TIntVector2<int> CountRange, "CountRange")
                            )
                            continue;

                        // TODO: CountRange thingy

                        var Rarity = ItemDef.GetOrDefault("Rarity", EFortRarity.Uncommon);

                        Console.Write($"        {CountRange.X}x ");
                        
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
                        Console.WriteLine($" ({(Weight3 / TotalWeight2) * 100:0.00}%)");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}