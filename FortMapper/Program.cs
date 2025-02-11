using System.Globalization;
using System.Numerics;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Component;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Versions;
using CUE4Parse.UE4.VirtualFileSystem;
using CUE4Parse.Utils;
using CUE4Parse_Conversion.Textures;
using FortMapper;
using Newtonsoft.Json;
using SkiaSharp;

public static class ActorExtension
{
    public static FVector GetActorLocation(this AActor actor)
    {
        if (!actor.TryGetValue(out UObject SMC, "RootComponent") ||
            !SMC.TryGetValue(out FVector Pos, "RelativeLocation"))
        {
            Console.WriteLine($"Couldn't find location for actor {actor.Name}");
            return FVector.ZeroVector;
        }
        return Pos;
    }
}

static class MainClass
{
    // I want to make this whole program interactive later on, but for now its just console program cus i suck at GUI
    static void Main(string[] args)
    {
        if (!File.Exists("oo2core_9_win64.dll"))
        {
            Console.WriteLine("Failed to find oo2core_9_win64.dll");
            return;
        }
        if (!File.Exists("mappings.usmap"))
        {
            Console.WriteLine("Failed to find mappings.usmap");
            return;
        }
        OodleHelper.Initialize("oo2core_9_win64.dll");
        
        var provider = new DefaultFileProvider(@"C:\Program Files\Epic Games\Fortnite\FortniteGame\Content\Paks", SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_UE5_LATEST));

        provider.MappingsContainer = new FileUsmapTypeMappingsProvider("./mappings.usmap");
        provider.Initialize();
        provider.SubmitKey(new FGuid(), new FAesKey("0x4940113FFF51E90CA7C9633AA84BC8075ADC90C71EFC0D1E8FCBD1A9CAADFC91"));

        var mapman = new MapManager(provider);
        mapman.Dump();
        mapman.Output();
    }

}