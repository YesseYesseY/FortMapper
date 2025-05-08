using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Engine;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Versions;
using CUE4Parse_Conversion.Textures;
using SkiaSharp;

namespace FortMapper
{
    // TODO: (not in order)
    // * Advanced output (Weight, count, itemdef, etc...)
    // * Hotfixes
    // * CompositeDatatable
    // * Move provider out
    public class LootExport
    {
        public struct LootTierData
        {
            public float Weight;
            public string LootPackage; 
            // TODO: Add direct reference to loot package?
        }

        public struct LootPackage
        {
            public float Weight;
            public string LootPackageCall;
            // TODO: Add direct reference to loot package call?
        }

        // Options
        public static string OutPath = "./Loot/";
        public static bool OutputItemIcons = true;

        public Dictionary<string, List<LootTierData>> LTD = new();
        public Dictionary<string, List<LootPackage>> LP = new();

        public static void Yes(string ltd_path, string lp_path)
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);


            var provider = new DefaultFileProvider(@"C:\Program Files\Epic Games\Fortnite\FortniteGame\Content\Paks", SearchOption.AllDirectories, new VersionContainer(EGame.GAME_UE5_LATEST), StringComparer.OrdinalIgnoreCase);
            
            provider.MappingsContainer = new FileUsmapTypeMappingsProvider("./mappings.usmap");
            provider.Initialize();
            provider.SubmitKey(new FGuid(), new FAesKey("0x17243B0E3E66DA90347F7C4787692505EC5E5285484633D71B09CD6ABB714E9B"));
            provider.PostMount();
            provider.LoadVirtualPaths();
            

            Console.WriteLine(provider.ProjectName);

            var ltd_package = provider.LoadPackageObject<UDataTable>(ltd_path);
            var lp_package = provider.LoadPackageObject<UDataTable>(lp_path);

            Directory.CreateDirectory(OutPath);
            var ret = new LootExport();

            foreach (var row in ltd_package.RowMap)
            {
                var tg = row.Value.Get<FName>("TierGroup").Text;
                var weight = row.Value.Get<float>("Weight");

                if (!ret.LTD.ContainsKey(tg))
                    ret.LTD[tg] = new();

                ret.LTD[tg].Add(new LootTierData { Weight = weight, LootPackage = tg });
            }

            foreach (var row in lp_package.RowMap)
            {
                var lpc = row.Value.Get<string>("LootPackageCall");
                var lpid = row.Value.Get<FName>("LootPackageID").Text;
                if (lpc == "")
                {
                    var itemdefpath = row.Value.Get<FSoftObjectPath>("ItemDefinition");
                    if (itemdefpath.TryLoad(out UObject? itemdef) && itemdef is not null 
                        && itemdef.TryGet("DataList", out FInstancedStruct[]? dl) && dl is not null
                        )
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
            }
        }
    }
} 
