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

namespace FortMapper
{
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
        public CTexture? MinimapTexture;
        [JsonIgnore]
        public Dictionary<string, int> ActorClasses = new();


        [JsonProperty("actors")]
        public Dictionary<string, List<Vector3>> Actors = new();
        [JsonProperty("camera_pos")]
        public Vector3 CameraPos;
        [JsonProperty("camera_rot")]
        public FRotator CameraRot;
        [JsonProperty("camera_relrot")]
        public FRotator CameraRelRot;
        // All values below are the same on hermes and figment so i see no reason to dynamically get them
        // Also you prob dont need them
        [JsonProperty("camera_field_of_view")]
        public readonly float CameraFieldOfView = 14.0f;
        [JsonProperty("camera_ortho_width")]
        public readonly float CameraOrthoWidth = 262426.0f;
        [JsonProperty("camera_ortho_near_clip_plane")]
        public readonly float CameraOrthoNearClipPlane = 0.0f;
        [JsonProperty("camera_ortho_far_clip_plane")]
        public readonly float CameraOrthoFarClipPlane = 200000.0f;
        [JsonProperty("camera_aspect_ratio")]
        public readonly float CameraAspectRatio = 1.0f;

        private void Parse(FPackageIndex? pi)
        {
            if (pi is null || pi.IsNull)
                return;

            if (pi.Name.Contains("CameraActor") &&
                pi.TryLoad(out UObject? camera) &&
                camera is not null &&
                camera.TryGet("Tags", out FName[]? tags) &&
                tags is not null &&
                tags.Length > 0 &&
                tags[0].PlainText == "MinimapCaptureCamera" &&
                camera.TryGet("SceneComponent", out USceneComponent? sc) &&
                sc is not null &&
                camera.TryGet("CameraComponent", out UCameraComponent? cc) &&
                cc is not null
            )
            {
                if (cc.TryGet("RelativeRotation", out FRotator cc_relrot))
                    CameraRelRot = cc_relrot;
                CameraPos = sc.GetRelativeLocation();
                CameraRot = sc.GetRelativeRotation();
            }

            if (pi.ResolvedObject is not null && 
                pi.ResolvedObject.Class is not null)
            {
                if (!ActorClasses.ContainsKey(pi.ResolvedObject.Class.Name.Text))
                    ActorClasses[pi.ResolvedObject.Class.Name.Text] = 0;

                ActorClasses[pi.ResolvedObject.Class.Name.Text]++;

                foreach (var class_name in ActorsToExport)
                {
                    if (pi.ResolvedObject.Class.Name.PlainText == class_name &&
                        pi.ResolvedObject.TryLoad(out UObject actor) &&
                        actor.TryGet("StaticMeshComponent", out UStaticMeshComponent? smc) && smc is not null
                        )
                    {
                        Actors[class_name].Add(smc.GetRelativeLocation());
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
                var icondecode = MinimapTexture?.ToSkBitmap();
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
