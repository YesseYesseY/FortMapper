using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE4Parse.GameTypes.FF7.Assets.Exports;
using CUE4Parse.UE4.AssetRegistry;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.UObject;
using FortMapper;

namespace FortMapper
{
    public static class GameFeatureStuff
    {
        public static Dictionary<string, List<(FSoftObjectPath ltd, FSoftObjectPath lp)>> TagLoots = new();
        public static void Init()
        {
            foreach (var file in GlobalProvider._provider.Files)
            {
                if (file.Key == "FortniteGame/AssetRegistry.bin")
                {
                    Console.WriteLine(file.Key);
                    var ar = new FAssetRegistryState(file.Value.CreateReader());
                    foreach (var thing in ar.PreallocatedAssetDataBuffers)
                    {
                        if (thing.AssetClass.Text == "FortGameFeatureData")
                        {
                            if (GlobalProvider._provider.TryLoadPackageObject(thing.ObjectPath, out var gfd) &&
                                gfd.TryGet<FStructFallback>("DefaultLootTableData", out var loot_tables) &&
                                gfd.TryGet<UObject[]>("Actions", out var actions) && 
                                loot_tables.TryGet<FSoftObjectPath>("LootTierData", out var feature_ltd) &&
                                loot_tables.TryGet<FSoftObjectPath>("LootPackageData", out var feature_lp))
                            {
                                foreach (var action in actions)
                                {
                                    if (action is not null && action.Class!.Name == "FortGameFeatureAction_AddToPlaylists")
                                    {
                                        if (action.TryGet<FStructFallback>("PlaylistQuery", out var pq) &&
                                            pq.TryGet<FStructFallback[]>("TagDictionary", out var td))
                                        {
                                            List<string> added_tags = new();

                                            if (gfd.TryGet<UScriptMap>("PlaylistOverrideLootTableData", out var poltd))
                                            {
                                                foreach (var thingy in poltd.Properties)
                                                {
                                                    var key = thingy.Key.GetValue<FStructFallback>().Get<FName>("TagName").Text;
                                                    if (!TagLoots.ContainsKey(key))
                                                        TagLoots[key] = new();
                                                    var value = thingy.Value.GetValue<FStructFallback>();
                                                    var cur_ltd = value.Get<FSoftObjectPath>("LootTierData");
                                                    var cur_lp = value.Get<FSoftObjectPath>("LootPackageData");
                                                    TagLoots[key].Add((cur_ltd, cur_lp));
                                                    added_tags.Add(key);
                                                }
                                            }

                                            foreach (var tag in td)
                                            {
                                                var tag_name = tag.Get<FName>("TagName");
                                                if (!added_tags.Contains(tag_name.Text))
                                                {
                                                    if (!TagLoots.ContainsKey(tag_name.Text))
                                                        TagLoots[tag_name.Text] = new();
                                                    TagLoots[tag_name.Text].Add((feature_ltd, feature_lp));
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
            }
        }
    }
}
