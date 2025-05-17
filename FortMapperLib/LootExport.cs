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
        public string ItemName
        {
            get {
                if (ItemDef.TryGet("ItemName", out FText? text) && text is not null)
                {
                    return text.Text;
                }
                return "No name :)";
            }
        }
        [JsonIgnore]
        private UTexture2D? _ItemIcon;
        [JsonIgnore]
        public UTexture2D? ItemIcon
        {
            get
            {
                if (_ItemIcon is not null)
                    return _ItemIcon;

                if (ItemDef.TryGet("DataList", out FInstancedStruct[]? dl))
                {
                    foreach (var thing in dl!)
                    {
                        if (thing.NonConstStruct!.TryGetValue(out FSoftObjectPath iconpath, "LargeIcon", "Icon") &&
                            iconpath.TryLoad<UTexture2D>(out UTexture2D? icon))
                        {
                            _ItemIcon = icon;
                            return icon;
                        }
                    }
                }

                return null;
            }
        }
        [JsonProperty("item_rarity")]
        public string ItemRarity => ItemDef.GetOrDefault("Rarity", EFortRarity.Uncommon).ToString();
        [JsonProperty("item_count")]
        public int ItemCount;
        [JsonProperty("item_icon")]
        public string ItemIconPath
        {
            get
            {
                if (ItemIcon is null) return "";
                var ret = $"{ItemIcon.Name}.png";
                foreach (var c in Path.GetInvalidFileNameChars())
                    ret.Replace(c, '_');
                return Path.Join("Images", ret).Replace('\\', '/');
            }
        }
    }

    // NOTE: Only OG loot is seriously tested
    // TODO: (not in order)
    // * Automatically get all the correct datatables from gamefeatures
    public class LootExport
    {
        // Options
        public static string OutPath = "./Loot/";
        public static bool OutputItemIcons = true;
        public static Formatting JsonFormatting = Formatting.None;

        public Dictionary<string, List<LootTierData>> LTD = new();
        public Dictionary<string, List<LootPackage>> LP = new();
        public Dictionary<string, List<LootPackageCall>> LPC = new();
        
        private static void SaveItemIcon(LootPackageCall lpc)
        {
            var iconoutpath = Path.Join(OutPath, lpc.ItemIconPath);
            if (File.Exists(iconoutpath) || lpc.ItemIcon is null)
                return;

            ExportUtils.ExportTexture2D(lpc.ItemIcon, iconoutpath);
        }

        public void Export(bool export_icons = false)
        {
            Directory.CreateDirectory(Path.Join(OutPath, "Images"));
            File.WriteAllText(Path.Join(OutPath, "LTD.json"), JsonConvert.SerializeObject(LTD, JsonFormatting));
            File.WriteAllText(Path.Join(OutPath, "LP.json"), JsonConvert.SerializeObject(LP, JsonFormatting));
            File.WriteAllText(Path.Join(OutPath, "LPC.json"), JsonConvert.SerializeObject(LPC, JsonFormatting));
            if (export_icons)
                foreach (var thing in LPC.Values)
                    foreach (var thingy in thing)
                        SaveItemIcon(thingy);
        }

        public static LootExport Yes(params (string ltd, string lp)[] paths)
        {
            var ret = new LootExport();

            foreach (var lootpath in paths)
            {
                var ltd_package = GlobalProvider.LoadPackageObject<UDataTable>(lootpath.ltd);
                var lp_package = GlobalProvider.LoadPackageObject<UDataTable>(lootpath.lp);


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
                            itemdefpath.TryLoad(out UObject? itemdef))
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
            }

            return ret;
        }
    }
} 
