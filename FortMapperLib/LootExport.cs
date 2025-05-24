using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        [JsonProperty("loot_package_category_min_array")]
        public int[] LootPackageCategoryMinArray;
    }

    public struct LootPackage
    {
        [JsonProperty("weight")]
        public float Weight;
        [JsonProperty("loot_package_call")]
        public string LootPackageCall;
    }

    public class TempLootTierData
    {
        public float Weight;
        public string LootPackage;
        public string TierGroup;
        public int[] LootPackageCategoryMinArray;
    }

    public class TempLootPackage
    {
        public float Weight;
        public string LootPackageCall;
        public string LootPackageId;
        public FSoftObjectPath ItemDef;
        public TIntVector2<int> CountRange;
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
                    foreach (var thing in dl!.Reverse())
                    {
                        if (thing.NonConstStruct!.TryGetValue(out FSoftObjectPath iconpath, "LargeIcon", "Icon") && iconpath.TryLoad<UTexture2D>(out UTexture2D? icon))
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
                return Path.Join("Images", ExportUtils.ValidFileName($"{ItemIcon.Name}.png")).Replace('\\', '/');
            }
        }
    }

    // NOTE:
    // * Only OG loot is seriously tested
    // * This is SUPER messy and i dont feel like making it good right now
    // TODO: (not in order)
    // * Rest of values to make ltd and lp function correctly (im looking at you 4x shotgun shell llama)
    // * Automatically get all the correct datatables from gamefeatures
    public class LootExport
    {
        // Options
        [JsonIgnore]
        public static string OutPath = "./Loot/";
        [JsonIgnore]
        public static bool OutputItemIcons = true;
        [JsonIgnore]
        public static Formatting JsonFormatting = Formatting.None;
        [JsonIgnore]
        public static string Hotfixes = "";

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
            File.WriteAllText(Path.Join(OutPath, "Loot.json"), JsonConvert.SerializeObject(this, JsonFormatting));
            if (export_icons)
                foreach (var thing in LPC.Values)
                    foreach (var thingy in thing)
                        SaveItemIcon(thingy);
        }

        public static LootExport Yes(string playlist_path)
        {
            List<(string ltd, string lp)> paths = new();

            var playlist = GlobalProvider.LoadPackageObject<UObject>(playlist_path);
            paths.Add((playlist.Get<FSoftObjectPath>("LootTierData").AssetPathName.PlainText, playlist.Get<FSoftObjectPath>("LootPackages").AssetPathName.PlainText));

            return Yes(paths);
        }

        public static LootExport Yes(params IEnumerable<(string ltd, string lp)> paths)
        {
            var ret = new LootExport();

            Dictionary<string, TempLootTierData> TEMP_LTD = new();
            Dictionary<string, TempLootPackage> TEMP_LP = new();

            foreach (var lootpath in paths)
            {
                var ltd_package = GlobalProvider.LoadPackageObject<UDataTable>(lootpath.ltd);
                var lp_package = GlobalProvider.LoadPackageObject<UDataTable>(lootpath.lp);

                foreach (var row in ltd_package.RowMap)
                {
                    TEMP_LTD[row.Key.Text] = new TempLootTierData
                    {
                        LootPackage = row.Value.Get<FName>("LootPackage").Text,
                        Weight = row.Value.Get<float>("Weight"),
                        TierGroup = row.Value.Get<FName>("TierGroup").Text,
                        LootPackageCategoryMinArray = row.Value.Get<int[]>("LootPackageCategoryMinArray")
                    };
                }

                foreach (var row in lp_package.RowMap)
                {
                    TEMP_LP[row.Key.Text] = new TempLootPackage
                    {
                        ItemDef = row.Value.Get<FSoftObjectPath>("ItemDefinition"),
                        Weight = row.Value.Get<float>("Weight"),
                        LootPackageId = row.Value.Get<FName>("LootPackageID").Text,
                        LootPackageCall  = row.Value.Get<string>("LootPackageCall"),
                        CountRange = row.Value.Get<TIntVector2<int>>("CountRange")
                    };
                }

                List<string> parent_tables = new();

                {
                    parent_tables.Add(ltd_package.Outer!.GetPathName());
                    if (ltd_package.TryGet<UObject[]>("ParentTables", out var ptables))
                    {
                        foreach (var table in ptables)
                        {
                            parent_tables.Add(table.Outer!.GetPathName());
                        }
                    }

                    foreach (var line in Hotfixes.Split('\n'))
                    {
                        if (line.StartsWith("+DataTable="))
                        {
                            var split = line.Split(';');

                            var hotfixtable = split[0].Substring("+DataTable=".Length);

                            if (!parent_tables.Any(path => hotfixtable == path))
                                continue;

                            if (split[1] != "RowUpdate")
                                continue; // throw new NotImplementedException();

                            if (split[3] != "Weight")
                                continue; // throw new NotImplementedException();

                            TEMP_LTD[split[2]].Weight = float.Parse(split[4], CultureInfo.InvariantCulture);
                        }
                    }

                    parent_tables.Clear();
                }

                {
                    parent_tables.Add(lp_package.Outer!.GetPathName());
                    if (lp_package.TryGet<UObject[]>("ParentTables", out var ptables))
                    {
                        foreach (var table in ptables)
                        {
                            parent_tables.Add(table.Outer!.GetPathName());
                        }
                    }

                    foreach (var line in Hotfixes.Split('\n'))
                    {
                        if (line.StartsWith("+DataTable="))
                        {
                            var split = line.Split(';');

                            var hotfixtable = split[0].Substring("+DataTable=".Length);

                            if (!parent_tables.Any(path => hotfixtable == path))
                                continue;

                            if (split[1] != "RowUpdate")
                                continue; // throw new NotImplementedException();

                            if (split[3] != "Weight")
                                continue; // throw new NotImplementedException();



                            TEMP_LP[split[2]].Weight = float.Parse(split[4], CultureInfo.InvariantCulture);
                        }
                    }

                    parent_tables.Clear();
                }
            }

            foreach (var thingy in TEMP_LTD)
            {
                var tg = thingy.Value.TierGroup;
                var lp = thingy.Value.LootPackage;
                var weight = thingy.Value.Weight;
                if (weight <= 0.0f) continue;

                if (!ret.LTD.ContainsKey(tg))
                    ret.LTD[tg] = new();

                ret.LTD[tg].Add(new LootTierData { Weight = weight, LootPackage = lp, LootPackageCategoryMinArray = thingy.Value.LootPackageCategoryMinArray });
            }

            foreach (var thingy in TEMP_LP)
            {
                var lpc = thingy.Value.LootPackageCall;
                var lpid = thingy.Value.LootPackageId;
                var weight = thingy.Value.Weight;
                if (weight <= 0.0f) continue;

                if (lpc == "")
                {
                    // If its WorldList and has no itemdef just ignore
                    if (thingy.Value.ItemDef.TryLoad(out UObject? itemdef))
                    {
                        var count = thingy.Value.CountRange;
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
