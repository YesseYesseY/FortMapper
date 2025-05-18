using CUE4Parse.Compression;
using CUE4Parse.FileProvider;
using CUE4Parse.MappingsProvider;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse_Conversion.Textures;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Core.Math;
using System.Numerics;
using CUE4Parse.UE4.Assets.Exports.Component;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Assets.Exports.WorldPartition;
using CUE4Parse.UE4.Assets.Utils;
using CUE4Parse.UE4.Assets.Objects.Properties;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using Newtonsoft.Json;
using SkiaSharp;
using CUE4Parse.UE4.Objects.Core.i18N;

namespace FortMapper
{
    // NOTE: I doubt anything but pos and orthowidth is really needed... MAYBE rot
    public class CameraProperties
    {
        [JsonProperty("pos")]
        public Vector3 Pos;
        [JsonProperty("rot")]
        public FRotator Rot;
        [JsonProperty("relrot")]
        public FRotator RelRot;
        [JsonProperty("ortho_width")]
        public float OrthoWidth;
        // All values below are the same on hermes and figment so i see no reason to dynamically get them
        // Also you prob dont need them
        [JsonProperty("field_of_view")]
        public readonly float FieldOfView = 14.0f;
        [JsonProperty("ortho_near_clip_plane")]
        public readonly float OrthoNearClipPlane = 0.0f;
        [JsonProperty("ortho_far_clip_plane")]
        public readonly float OrthoFarClipPlane = 200000.0f;
        [JsonProperty("aspect_ratio")]
        public readonly float AspectRatio = 1.0f;
    }
    public class WorldExport
    {
        [JsonIgnore]
        public static string OutPath = "./World/";
        [JsonIgnore]
        public static List<string> ActorsToExport = new();
        [JsonIgnore]
        public static Formatting JsonFormatting = Formatting.None;
        [JsonIgnore]
        public static bool OutputActorClasses = false;
        [JsonIgnore]
        public static bool OutputActorIcons = true;

        [JsonIgnore]
        public static UObject? QuestIndicatorData = null;

        [JsonIgnore]
        public CTexture? MinimapTexture;
        [JsonIgnore]
        public Dictionary<string, int> ActorClasses = new();


        [JsonProperty("actors")]
        public Dictionary<string, List<Vector3>> Actors = new();
        [JsonProperty("pois")]
        public Dictionary<string, Vector3> POIs = new();
        [JsonProperty("camera")]
        public CameraProperties Camera = new();

        private void Parse(FPackageIndex? pi)
        {
            if (pi is null || pi.IsNull)
                return;

            if (pi.Name.Contains("CameraActor") &&
                pi.TryLoad(out UObject? camera) &&
                camera.TryGet("Tags", out FName[]? tags) &&
                tags!.Length > 0 && tags[0].PlainText == "MinimapCaptureCamera" &&
                camera.TryGet("SceneComponent", out USceneComponent? sc) &&
                camera.TryGet("CameraComponent", out UCameraComponent? cc)
            )
            {
                if (cc!.TryGet("RelativeRotation", out FRotator cc_relrot))
                    Camera.RelRot = cc_relrot;
                Camera.Pos = sc!.GetRelativeLocation();
                Camera.Rot = sc.GetRelativeRotation();
                Camera.OrthoWidth = cc!.Get<float>("OrthoWidth");
            }

            if (pi.ResolvedObject is not null && 
                pi.ResolvedObject.Class is not null)
            {
                if (!ActorClasses.ContainsKey(pi.ResolvedObject.Class.Name.Text))
                    ActorClasses[pi.ResolvedObject.Class.Name.Text] = 0;

                ActorClasses[pi.ResolvedObject.Class.Name.Text]++;

                foreach (var class_name in ActorsToExport)
                {
                    bool exported_icon = false;
                    if (pi.ResolvedObject.Class.Name.PlainText == class_name &&
                        pi.ResolvedObject.TryLoad(out UObject actor))
                    {
                        if (actor.TryGetValue(out USceneComponent? smc, "StaticMeshComponent", "RootComponent", "Root"))
                        {
                            Actors[class_name].Add(smc!.GetRelativeLocation());
                        }

                        if (OutputActorIcons && !exported_icon && ((UClass)actor.Class!).ClassDefaultObject.TryLoad(out var actorclassdefault) && 
                            actorclassdefault.TryGet("MarkerDisplay", out FStructFallback? md) &&
                            md!.TryGet("Icon", out FSoftObjectPath iconpath) && iconpath.TryLoad<UTexture2D>(out UTexture2D? icon_texture))
                        {
                            File.WriteAllBytes(Path.Join(OutPath, $"{actor.Class!.Name}.png"), icon_texture!.Decode()!.Encode(ETextureFormat.Png, out string ext));
                            exported_icon = true;
                        }
                    }
                }
                    
            }
        }

        public void Export(bool export_minimap = false)
        {
            Directory.CreateDirectory(OutPath);
            File.WriteAllText(Path.Join(OutPath, "World.json"), JsonConvert.SerializeObject(this, JsonFormatting));

            if (OutputActorClasses)
            {
                // https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
                ActorClasses = (from entry in ActorClasses orderby entry.Value descending select entry).ToDictionary();
                File.WriteAllText(Path.Join(OutPath, "ActorClasses.json"), JsonConvert.SerializeObject(ActorClasses, JsonFormatting));
            }

            if (export_minimap)
            {
                var icondecode = MinimapTexture?.ToSkBitmap ();
                if (icondecode is not null)
                {
                    using (var image = SKImage.FromBitmap(icondecode))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite($"{OutPath}/map.png"))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
        }

        public static WorldExport? Yes(string mappath, string minimappath)
        {
            if (QuestIndicatorData is null)
            {
                QuestIndicatorData = GlobalProvider.LoadPackageObject("FortniteGame/Content/Quests/QuestIndicatorData.QuestIndicatorData");
            }

            var ret = new WorldExport();
            foreach (var class_name in ActorsToExport)
                ret.Actors[class_name] = new();

            ret.MinimapTexture = GlobalProvider.LoadPackageObject<UTexture2D>(minimappath).Decode();

            string mapname;

            {
                var world = GlobalProvider.LoadPackageObject<UWorld>(mappath);
                mapname = world.Name;
                var level = world.PersistentLevel.Load<ULevel>();
                if (level is null)
                {
                    Console.WriteLine(":(");
                    return null;
                }

                foreach (var actor in level.Actors)
                    ret.Parse(actor);
            }

            if (QuestIndicatorData.TryGet("ChallengeMapsPoiData", out UScriptMap? cmpd))
            {
                foreach (var mapthing in cmpd!.Properties)
                {
                    if (mapthing.Key.GetValue<FName>().PlainText == mapname)
                    {
                        var structthing = mapthing.Value!.GetValue<FStructFallback>();
                        if (structthing!.TryGet("ChallengeMapsPoiData", out FStructFallback[]? cmpd_real))
                        {
                            foreach (var poi in cmpd_real!)
                            {
                                // TODO: Fix duplicate names
                                ret.POIs[poi.Get<FText>("Text").Text] = poi.Get<FVector>("WorldLocation");
                            }
                        }
                        break;
                    }
                }
            }

            // This works but is kinda scuffed, the other way to do it would be to get it from
            // World -> PersistentLevel -> WorldSettings -> WorldPartition -> RuntimeHash -> RuntimeStreamingData -> MainPartition ->
            // SpatiallyLoadedCells -> (foreach) -> LevelStreaming -> WorldAsset -> PersistentLevel
            foreach (var file in GlobalProvider.Files)
            {
                if (file.Key.EndsWith(".umap") && file.Key.Contains($"{mapname}/_Generated_/"))
                {
                    var level = GlobalProvider.LoadPackageObject<ULevel>(file.Key.Replace(".umap", ".PersistentLevel"));
                    foreach (var actor in level.Actors)
                        ret.Parse(actor);
                }
            }

            return ret;
        }
    }
}
