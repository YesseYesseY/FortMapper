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
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using CUE4Parse_Conversion.Textures;
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

        Dictionary<string, int> missedActors = new();

        List<FVector> Chests = new();
        foreach (var file in provider.Files)
        {
            if (!file.Key.StartsWith("fortnitegame/plugins/gamefeatures/brmapch6/content/maps/hermes_terrain/_generated_/") || !file.Key.EndsWith(".umap"))
                continue;

            var lvl = provider.LoadObject<ULevel>(file.Key.Replace(".umap", ".PersistentLevel"));

            var objs = lvl.Actors;
            foreach (var packageindex in objs)
            {
                if (packageindex.IsNull || !packageindex.Name.Contains("Chest") || !packageindex.TryLoad(out AActor actor) || actor is null)
                {
                    var parsedName = packageindex.Name.Split("_UAID_")[0];
                    if (missedActors.ContainsKey(parsedName))
                        missedActors[parsedName]++;
                    else
                        missedActors[parsedName] = 1;
                    continue;
                }

                Chests.Add(actor.GetActorLocation());
            }
        }

        var bmp = TextureDecoder.Decode(provider.LoadObject<UTexture2D>("FortniteGame/Content/Athena/Apollo/Maps/UI/Apollo_Terrain_Minimap.Apollo_Terrain_Minimap"));
        using (var canvas = new SKCanvas(bmp))
        {
            var paint = new SKPaint
            {
                Color = SKColors.Red,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                StrokeWidth = 8
            };

            foreach (var pos in Chests)
            {
                var mappos = GetMapPos(pos);
                canvas.DrawCircle(mappos.X, mappos.Y, 2.5f, paint);
            }

            using (var image = SKImage.FromBitmap(bmp))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite("output.png"))
            {
                data.SaveTo(stream);
            }
        }
        
        // black magic from https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
        var sorted = (from entry in missedActors orderby entry.Value descending select entry).ToDictionary();
        File.WriteAllText("missed.json", JsonConvert.SerializeObject(sorted, Formatting.Indented));
    }

    static Vector2 GetMapPos(FVector WorldPos)
    {
        Vector3 CamPos = new Vector3(8500.0f, 508.0f, 100000.0f);
        Vector3 CamRot = new Vector3(-180.0f, 0.0f, 0.0f);
        //Vector3 CamRot = new Vector3(0.0f, 0.0f, -90.0f);

        float OrthoFarClipPlane = 200000.0f;
        float OrthoWidth = 299926.0f;

        int ImageSize = 2048;
        Matrix4x4 ViewMatrix =
            Matrix4x4.CreateRotationZ(CamRot.Z.ToRadians()) *
            Matrix4x4.CreateRotationY(CamRot.Y.ToRadians()) *
            Matrix4x4.CreateRotationX(CamRot.X.ToRadians()) *
            Matrix4x4.CreateTranslation(-CamPos);

        Matrix4x4 ProjectionMatrix = new Matrix4x4(
            2 / OrthoWidth, 0, 0, 0,
            0, 2 / OrthoWidth, 0, 0,
            0, 0, -2 / OrthoFarClipPlane, -OrthoFarClipPlane / OrthoFarClipPlane,
            0, 0, 0, 1
        );

        Vector4 ClipSpacePoint = Vector4.Transform(Vector4.Transform(new Vector4(WorldPos, 1.0f), ViewMatrix), ProjectionMatrix);

        return new Vector2
        (
            (ClipSpacePoint.X + 1) * 0.5f * ImageSize,
            ((1 - (ClipSpacePoint.Y + 1) * 0.5f) * ImageSize) - 6f
        );
    }
}