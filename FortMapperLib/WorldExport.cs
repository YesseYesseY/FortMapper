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
using System;
using System.Diagnostics;
using System.Xml.Linq;

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
    public class ActorGroup
    {
        public ActorGroup(string name, string icon_path, params string[] classes)
        {
            Name = name;
            Classes = classes;
            Icon = GlobalProvider.LoadPackageObject<UTexture2D>(icon_path);
            Positions = new();
            Disabled = false;
        }
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("classes")]
        public string[] Classes;
        [JsonIgnore]
        public UTexture2D Icon;
        [JsonProperty("positions")]
        public List<FVector> Positions;
        [JsonProperty("disabled")]
        public bool Disabled;
    }
    public class WorldExport
    {
        [JsonIgnore]
        public string OutPath = "./World/";
        [JsonIgnore]
        public bool OutputActorClasses = false;
        [JsonIgnore]
        public Formatting JsonFormatting = Formatting.None;
        [JsonIgnore]
        public bool UseScuffedScan = false;

        [JsonIgnore]
        public static UObject? QuestIndicatorData = null;

        [JsonIgnore]
        public UTexture2D? MinimapTexture = null;
        [JsonIgnore]
        public Dictionary<string, int> ActorClasses = new();

        [JsonProperty("actors")]
        public List<ActorGroup> ActorGroups = new()
        {
            new("Chests", "FortniteGame/Content/UI/Foundation/Textures/Icons/Athena/T-T-Icon-BR-SM-Athena-Chest-01.T-T-Icon-BR-SM-Athena-Chest-01", "Tiered_Chest_6_Figment_C", "Tiered_Chest_Sunflower_C", "Tiered_Chest_Athena_C", "AlwaysSpawn_NormalChest_C"),
            new("Rare Chests", "FortniteGame/Content/UI/Foundation/Textures/Icons/Athena/T-T-Icon-BR-RareBlueChest.T-T-Icon-BR-RareBlueChest", "AlwaysSpawn_RareChest_C"),
            new("Ammo Boxes", "FortniteGame/Content/UI/Foundation/Textures/Icons/Athena/T-T-Icon-BR-AmmoBoxMarkIII.T-T-Icon-BR-AmmoBoxMarkIII", "Tiered_Ammo_Figment_C", "Tiered_Ammo_Athena_C"),
            new("Campfires", "FortniteGame/Content/Athena/Items/Traps/CampFire/T-Icon-BR-SM-Athena-Campfire.T-Icon-BR-SM-Athena-Campfire", "B_BGA_Athena_EnvCampFire_C", "BP_BGA_Outdoor_Campfire_A_C"),
            new("Reboot Vans", "FortniteGame/Content/UI/Foundation/Textures/Icons/Athena/T_Icon_BR_RebootVan_BuyBack.T_Icon_BR_RebootVan_BuyBack", "BGA_Athena_SCMachine_Figment_C", "BGA_Athena_SCMachine_Redux_C"),
            new("Vending Machines", "FortniteGame/Content/Athena/HUD/Interaction/T_UI_VendingMachine.T_UI_VendingMachine", "B_Athena_VendingMachine_Figment_C"),
            new("Rifts", "FortniteGame/Content/Athena/HUD/Interaction/T_UI_NPC_Rift.T_UI_NPC_Rift", "BGA_RiftPortal_Figment_C"),
            //new("Dirtbike", "FortniteGame/Content/UI/Foundation/Textures/Icons/SkillTree/T-Icon-ST-Dirtbike-128.T-Icon-ST-Dirtbike-128", "LW_DirtbikeVehicle_Spawner_C"),
        };
        [JsonProperty("pois")]
        public Dictionary<string, Vector3> POIs = new();
        [JsonProperty("ziplines")]
        public List<(FVector start, FVector end)> Ziplines = new();
        [JsonProperty("camera")]
        public CameraProperties Camera = new();
        [JsonProperty("minimap_path")]
        public string MinimapPath = "";
        [JsonProperty("map_name")]
        public string MapName = "";

        private void ParseActor(FPackageIndex? pi)
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

                switch (pi.ResolvedObject.Class.Name.Text)
                {
                    case "BP_Athena_Environmental_ZipLine_Spline_C":
                        var spline = pi.Load();
                        Ziplines.Add((spline!.Get<FVector>("HigherEndLocation"), spline!.Get<FVector>("LowerEndLocation")));
                        break;
                    default:
                        foreach (var actor_group in ActorGroups)
                        {
                            if (actor_group.Classes.Contains(pi.ResolvedObject.Class.Name.PlainText) &&
                                pi.ResolvedObject.TryLoad(out UObject actor))
                            {
                                if (actor.TryGetValue(out USceneComponent? smc, "StaticMeshComponent", "RootComponent", "Root"))
                                {
                                    actor_group.Positions.Add(smc!.GetRelativeLocation());
                                }
                            }
                        }
                        break;
                }

                    
            }
        }

        public void Export(string? custom_name = null)
        {
            if (OutputActorClasses)
            {
                // https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
                ActorClasses = (from entry in ActorClasses orderby entry.Value descending select entry).ToDictionary();
                File.WriteAllText(Path.Join(OutPath, ExportUtils.ValidFileName($"{(custom_name is not null ? custom_name : "MapName")}_ActorClasses.json")), JsonConvert.SerializeObject(ActorClasses, JsonFormatting));
            }

            if (MinimapTexture is not null)
            {
                MinimapPath = Path.Join("Images", $"{MinimapTexture.Name}.png");
                ExportUtils.ExportTexture2D(MinimapTexture, ExportUtils.ValidFileName(Path.Join(OutPath, MinimapPath)));
            }

            foreach (var actor_group in ActorGroups)
            {
                ExportUtils.ExportTexture2D(actor_group.Icon, ExportUtils.ValidFileName(Path.Join(OutPath, "Images", $"{actor_group.Name}.png")));
            }

            File.WriteAllText(Path.Join(OutPath, ExportUtils.ValidFileName($"{(custom_name is not null ? custom_name : "MapName")}.json")), JsonConvert.SerializeObject(this, JsonFormatting));
        }

        public bool Parse(UWorld world, UTexture2D minimap)
        {
            Directory.CreateDirectory(Path.Join(OutPath, "Images"));

            if (QuestIndicatorData is null)
                QuestIndicatorData = GlobalProvider.LoadPackageObject("FortniteGame/Content/Quests/QuestIndicatorData.QuestIndicatorData");

            MinimapTexture = minimap;

            MapName = world.Name;

            if (world.PersistentLevel.TryLoad(out ULevel? level))
                foreach (var actor in level!.Actors)
                    ParseActor(actor);
            
            // Can maybe get this from poi volumes?
            if (QuestIndicatorData.TryGet("ChallengeMapsPoiData", out UScriptMap? cmpd))
            {
                foreach (var mapthing in cmpd!.Properties)
                {
                    if (mapthing.Key.GetValue<FName>().PlainText == MapName)
                    {
                        var structthing = mapthing.Value!.GetValue<FStructFallback>();
                        if (structthing!.TryGet("ChallengeMapsPoiData", out FStructFallback[]? cmpd_real))
                        {
                            foreach (var poi in cmpd_real!)
                            {
                                // TODO: Fix duplicate names
                                POIs[poi.Get<FText>("Text").Text] = poi.Get<FVector>("WorldLocation");
                            }
                        }
                        break;
                    }
                }
            }

            if (UseScuffedScan)
            {
                foreach (var file in GlobalProvider.Files)
                {
                    if (file.Key.EndsWith(".umap") && file.Key.Contains($"{MapName}/_Generated_/"))
                    {
                        level = GlobalProvider.LoadPackageObject<ULevel>(file.Key.Replace(".umap", ".PersistentLevel"));
                        foreach (var actor in level.Actors)
                            ParseActor(actor);
                    }
                }
            }
            else
            {
                var world_settings = level!.Get<UObject>("WorldSettings");
                var world_partition = world_settings!.Get<UObject>("WorldPartition");
                var runtime_hash = world_partition!.Get<UObject>("RuntimeHash");
                if (runtime_hash!.TryGet<FStructFallback[]>("RuntimeStreamingData", out var runtime_streaming_data)) {
                    // NonSpatiallyLoadedCells?
                    foreach (var cell in runtime_streaming_data[0].Get<UObject[]>("SpatiallyLoadedCells"))
                    {
                        var level_streaming = cell.Get<UObject>("LevelStreaming");
                        var wrld = level_streaming.Get<FSoftObjectPath>("WorldAsset").Load<UWorld>();
                        var lvl = wrld.PersistentLevel.Load<ULevel>();
                        foreach (var actor in lvl!.Actors)
                            ParseActor(actor);
                    }
                }
                else if (runtime_hash!.TryGet<FStructFallback[]>("StreamingGrids", out var streaming_grids))
                {
                    Console.WriteLine("not implemented streaming grids");
                }
            }

            return true;
        }
    }
}
