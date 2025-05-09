using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.GameTypes.FN.Assets.Exports.Sound;
using CUE4Parse.GameTypes.FN.Enums;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using CUE4Parse_Conversion.Textures;
using Newtonsoft.Json;
using SkiaSharp;

namespace FortMapper
{
    public struct LootTierData
    {
        [JsonProperty("weight")]
        public float Weight;
        [JsonProperty("loot_package")]
        public string LootPackage;
    }

    public struct LootPackage
    {
        [JsonProperty("weight")]
        public float Weight;
        [JsonProperty("loot_package_call")]
        public string LootPackageCall;
    }
    
    public struct LootPackageCall
    {
        [JsonProperty("weight")]
        public float Weight;
        [JsonIgnore]
        public UObject ItemDef;
        [JsonProperty("item_name")]
        public string ItemName => ItemDef.Get<FText>("ItemName").Text;
        [JsonProperty("item_rarity")]
        public string ItemRarity => ItemDef.GetOrDefault("Rarity", EFortRarity.Uncommon).ToString();
        [JsonProperty("item_count")]
        public int ItemCount;
    }

    // TODO: (not in order)
    // * CompositeDatatable
    // * Move provider out
    public class LootExport
    {
        // Options
        public static string OutPath = "./Loot/";
        public static bool OutputItemIcons = true;

        public Dictionary<string, List<LootTierData>> LTD = new();
        public Dictionary<string, List<LootPackage>> LP = new();
        public Dictionary<string, List<LootPackageCall>> LPC = new();
        


        private static void SaveItemIcon(UObject itemdef)
        {
            if (itemdef.TryGet("DataList", out FInstancedStruct[]? dl) && dl is not null)
            {
                foreach (var thing in dl)
                {
                    if (thing.NonConstStruct is not null && thing.NonConstStruct.TryGet("LargeIcon", out FSoftObjectPath iconpath) &&
                        iconpath.TryLoad<UTexture2D>(out UTexture2D? icon) && icon is not null)
                    {
                        var icondecode = icon.Decode()?.ToSkBitmap();
                        if (icondecode is not null)
                        {
                            using (var image = SKImage.FromBitmap(icondecode))
                            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                            using (var stream = File.OpenWrite($"{OutPath}/{icon.Name}.png"))
                            {
                                data.SaveTo(stream);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public void Export(bool export_icons = false)
        {
            File.WriteAllText($"{OutPath}LTD.json", JsonConvert.SerializeObject(LTD, Formatting.Indented));
            File.WriteAllText($"{OutPath}LP.json", JsonConvert.SerializeObject(LP, Formatting.Indented));
            File.WriteAllText($"{OutPath}LPC.json", JsonConvert.SerializeObject(LPC, Formatting.Indented));
            if (export_icons)
                foreach (var thing in LPC.Values)
                    foreach (var thingy in thing)
                        SaveItemIcon(thingy.ItemDef);
        }

        public static LootExport Yes(string ltd_path, string lp_path)
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);

            var provider = new DefaultFileProvider(@"C:\Program Files\Epic Games\Fortnite\FortniteGame\Content\Paks", SearchOption.AllDirectories, new VersionContainer(EGame.GAME_UE5_LATEST), StringComparer.OrdinalIgnoreCase);
            
            provider.MappingsContainer = new FileUsmapTypeMappingsProvider("./mappings.usmap");
            provider.Initialize();
            provider.SubmitKey(new FGuid(), new FAesKey("0x17243B0E3E66DA90347F7C4787692505EC5E5285484633D71B09CD6ABB714E9B"));
            provider.PostMount();
            provider.LoadVirtualPaths();

            var ltd_package = provider.LoadPackageObject<UDataTable>(ltd_path);
            var lp_package = provider.LoadPackageObject<UDataTable>(lp_path);

            Directory.CreateDirectory(OutPath);
            var ret = new LootExport();

            foreach (var row in ltd_package.RowMap)
            {
                var tg = row.Value.Get<FName>("TierGroup").Text;
                var lp = row.Value.Get<FName>("LootPackage").Text;
                var weight = row.Value.Get<float>("Weight");
                if (weight <= 0.0f) continue;

                if (!ret.LTD.ContainsKey(tg))
                    ret.LTD[tg] = new();

                ret.LTD[tg].Add(new LootTierData { Weight = weight, LootPackage = lp });
            }

            foreach (var row in lp_package.RowMap)
            {
                var lpc = row.Value.Get<string>("LootPackageCall");
                var lpid = row.Value.Get<FName>("LootPackageID").Text;
                var weight = row.Value.Get<float>("Weight");
                if (weight <= 0.0f) continue;

                if (lpc == "")
                {
                    // If its WorldList and has no itemdef just ignore
                    if (row.Value.TryGet("ItemDefinition", out FSoftObjectPath itemdefpath) &&
                        itemdefpath.TryLoad(out UObject? itemdef) && itemdef is not null)
                    {
                        var count = row.Value.Get<TIntVector2<int>>("CountRange");
                        if (!ret.LPC.ContainsKey(lpid))
                            ret.LPC[lpid] = new();

                        ret.LPC[lpid].Add(new LootPackageCall
                        {
                            ItemDef = itemdef,
                            Weight = weight,
                            ItemCount = count.X
                        });
                    }
                }
                else
                {
                    if (!ret.LP.ContainsKey(lpid))
                        ret.LP[lpid] = new();

                    ret.LP[lpid].Add(new LootPackage
                    {
                        LootPackageCall = lpc,
                        Weight = weight
                    });
                }
            }

            return ret;
        }
    }
} 
