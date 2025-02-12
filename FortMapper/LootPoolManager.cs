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
        UDataTable LootTierData;
        UDataTable LootPackages;
        Dictionary<string, List<FStructFallback>> ParsedLootTierData;
        Dictionary<string, List<FStructFallback>> ParsedLootPackages;

        public LootPoolManager(UDataTable ltd, UDataTable lpd)
        {
            LootTierData = ltd;
            LootPackages = lpd;
            ParsedLootTierData = new();
            ParsedLootPackages = new();

            foreach (var row in LootTierData.RowMap)
            {
                if (!row.Value.TryGetValue(out FName TierGroup, "TierGroup"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootTierData.ContainsKey(TierGroup.Text))
                    ParsedLootTierData[TierGroup.Text] = new();
                ParsedLootTierData[TierGroup.Text].Add(row.Value);
            }

            foreach (var row in LootPackages.RowMap)
            {
                if (!row.Value.TryGetValue(out FName LootPackageID, "LootPackageID"))
                    throw new Exception("Failed to parse LootTierData");

                if (!ParsedLootPackages.ContainsKey(LootPackageID.Text))
                    ParsedLootPackages[LootPackageID.Text] = new();
                ParsedLootPackages[LootPackageID.Text].Add(row.Value);
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
                            case EFortRarity.Common:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                            case EFortRarity.Uncommon:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case EFortRarity.Rare:
                                Console.ForegroundColor = ConsoleColor.Blue;
                                break;
                            case EFortRarity.Epic:
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                break;
                            case EFortRarity.Legendary:
                                Console.ForegroundColor = ConsoleColor.DarkYellow;
                                break;
                            case EFortRarity.Mythic:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case EFortRarity.Exotic:
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                break;
                            default:
                                Console.WriteLine($"Unknown rarity: {Rarity}");
                                break;
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