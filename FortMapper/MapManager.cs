using System.Numerics;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Core.i18N;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.Utils;
using CUE4Parse_Conversion.Textures;
using Newtonsoft.Json;
using SkiaSharp;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.UObject;

namespace FortMapper
{
    public class MapManager
    {
        public MapManager(AbstractFileProvider _provider)
        {
            Provider = _provider;
        }

        Dictionary<string, int> MissedActors = new();
        List<FVector> Actors = new();
        List<KeyValuePair<string, FVector>> POIs = new();
        AbstractFileProvider Provider;

        void AddMissing(string miss)
        {
            if (MissedActors.ContainsKey(miss))
                MissedActors[miss]++;
            else
                MissedActors[miss] = 1;
        }

        void DumpActor(FPackageIndex packageindex)
        {
            if (!packageindex.TryLoad(out AActor actor))
                return;

            Actors.Add(actor.GetActorLocation());
        }
        
        void DumpPOI(FPackageIndex packageindex)
        {
            if (!packageindex.TryLoad(out AActor actor))
                return;

            if (!actor.TryGetValue(out UObject[] components, "InstanceComponents") || components.Length <= 0) return;
            
            foreach (var component in components)
            {
                if (component.ExportType == "FortPoi_DiscoverableComponent")
                {
                    if (!component.TryGetValue(out FText PlayerFacingName, "PlayerFacingName") ||
                        !component.TryGetValue(out int DiscoverMinimapBitId, "DiscoverMinimapBitId"))
                        continue;

                    POIs.Add(new(PlayerFacingName.Text, actor.GetActorLocation()));
                }
            }
        }

        public void Dump()
        {
            foreach (var file in Provider.Files)
            {
                if (!(file.Key.Contains("/hermes_terrain/_generated_/") || file.Key.EndsWith("Hermes_Terrain.umap")) || !file.Key.EndsWith(".umap")) continue;

                var lvl = Provider.LoadObject<ULevel>(file.Key.Replace(".umap", ".PersistentLevel"));

                var objs = lvl.Actors;
                foreach (var packageindex in objs)
                {
                    if (packageindex.IsNull) continue;

                    var parsedName = packageindex.Name.Split("_UAID_")[0];

                    switch (parsedName)
                    {
                        case "Tiered_Chest_Athena_C":
                        case "AlwaysSpawn_NormalChest_C":
                        case "Tiered_Chest_6_Parent47":
                        case "AlwaysSpawn_RareChest_C":
                        case "Tiered_Chest_6_Parent_C": // Why is this even used lol
                        case "B_FirePetal_ThemedChest_Container_C":
                            DumpActor(packageindex);
                            break;
                        case "FortPoiVolume":
                            DumpPOI(packageindex);
                            break;
                        default:
                            AddMissing(parsedName);
                            break;
                    }

                }
            }
        }

        public void Output() => OutputTo("Out/");
        public void OutputTo(string path)
        {
            if (!path.EndsWith('\\') || !path.EndsWith('/'))
                path += '/';

            Directory.CreateDirectory(path);

            var bmp = TextureDecoder.Decode(Provider.LoadObject<UTexture2D>("FortniteGame/Content/Athena/Apollo/Maps/UI/Apollo_Terrain_Minimap.Apollo_Terrain_Minimap"));
            using (var canvas = new SKCanvas(bmp))
            {
                var paint = new SKPaint
                {
                    Color = SKColors.Red,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 8
                };

                var textpaint = new SKPaint
                {
                    Color = SKColors.Coral,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 18
                };

                var font = SKTypeface.FromFamilyName("Arial").ToFont();
                font.Embolden = true;

                foreach (var pos in Actors)
                {
                    var mappos = MapCamera.GetMapPos(pos);
                    canvas.DrawCircle(mappos.X, mappos.Y, 2.5f, paint);
                }
                foreach (var poi in POIs)
                {
                    var mappos = MapCamera.GetMapPos(poi.Value);
                    canvas.DrawText(poi.Key, mappos.X, mappos.Y, SKTextAlign.Center, font, textpaint);
                }

                using (var image = SKImage.FromBitmap(bmp))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite($"{path}map.png"))
                {
                    data.SaveTo(stream);
                }
            }

            // black magic from https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
            var sorted = (from entry in MissedActors orderby entry.Value descending select entry).ToDictionary();
            File.WriteAllText($"{path}missed.json", JsonConvert.SerializeObject(sorted, Formatting.Indented));
        }

        static class MapCamera
        {
            static Vector3 CamPos = new Vector3(8500.0f, 508.0f, 100000.0f);
            static Vector3 CamRot = new Vector3(-180.0f, 0.0f, 0.0f);
            // static Vector3 CamRot = new Vector3(0.0f, 0.0f, -90.0f); // this is the rotation of the minimap camera... Im just gonna say they're wrong and im right, im not a bad programmer (surely)
            static float OrthoFarClipPlane = 200000.0f;
            static float OrthoWidth = 299926.0f;
            static int ImageSize = 2048;
            static Matrix4x4 ProjectionMatrix = new Matrix4x4(
                    2 / OrthoWidth, 0, 0, 0,
                    0, 2 / OrthoWidth, 0, 0,
                    0, 0, -2 / OrthoFarClipPlane, -OrthoFarClipPlane / OrthoFarClipPlane,
                    0, 0, 0, 1);
            static Matrix4x4 ViewMatrix =
                    Matrix4x4.CreateRotationZ(CamRot.Z.ToRadians()) *
                    Matrix4x4.CreateRotationY(CamRot.Y.ToRadians()) *
                    Matrix4x4.CreateRotationX(CamRot.X.ToRadians()) *
                    Matrix4x4.CreateTranslation(-CamPos);

            public static Vector2 GetMapPos(FVector WorldPos)
            {
                Vector4 ClipSpacePoint = Vector4.Transform(Vector4.Transform(new Vector4(WorldPos, 1.0f), ViewMatrix), ProjectionMatrix);

                return new Vector2
                (
                    (ClipSpacePoint.X + 1) * 0.5f * ImageSize,
                    ((1 - (ClipSpacePoint.Y + 1) * 0.5f) * ImageSize) - 6f
                );
            }
        }

    }

}
