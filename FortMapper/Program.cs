using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports.WorldPartition;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.MappingsProvider;
using CUE4Parse.Compression;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Objects.UObject;
using CUE4Parse.UE4.Assets.Exports;
using Newtonsoft.Json;

string[] WhitelistedActorTypes = {
    "Tiered_Chest_6_Figment_C"
};

FortMapInfo[] Maps = {
    new () {
        DisplayName = "Fortnite OG",
        MinimapPath = "FortniteGame/Plugins/GameFeatures/Figment/Figment_S06_MapUI/Content/MiniMapAthena_S06Temp.MiniMapAthena_S06Temp",
        LevelPath   = "FortniteGame/Plugins/GameFeatures/Figment/Figment_S06_Map/Content/Athena_Terrain_S06.PersistentLevel"
    }
};

OodleHelper.Initialize();

var provider = new DefaultFileProvider("/home/yes/Games/Fortnite/FortniteGame/Content/Paks", SearchOption.TopDirectoryOnly, new VersionContainer(EGame.GAME_UE5_LATEST));
provider.MappingsContainer = new FileUsmapTypeMappingsProvider("/home/yes/Downloads/++Fortnite+Release-37.51-CL-46968237_br.usmap");
// provider.ReadScriptData = true;
provider.Initialize();
provider.SubmitKey(new FGuid(), new FAesKey("0x40F92B7D44B95E105F9E8CCCCAD2140B7019949B0E047C06D37342CB430E39CE"));
provider.PostMount();
provider.LoadVirtualPaths();

var outputPath = Path.Join(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? "", "Output");
var worldOutputPath = Path.Join(outputPath, "World");

foreach (var Map in Maps) {
    if (!provider.TryLoadPackageObject<UTexture2D>(Map.MinimapPath, out UTexture2D? minimapTexture) || minimapTexture is null) {
        Console.WriteLine($"Failed to load minimap of {Map.DisplayName}");
        break;
    }

    Utils.ExportTexture2D(minimapTexture, Path.Join(worldOutputPath, $"{Map.DisplayName}.png"));

    if (!provider.TryLoadPackageObject<ULevel>(Map.LevelPath, out ULevel? mainLevel) || mainLevel is null) {
        Console.WriteLine($"Failed to lead level of {Map.DisplayName}");
        break;
    }

    List<ULevel> LevelsToSearch = new () {
        mainLevel
    };

    if (mainLevel.TryGetValue(out UObject worldSettings, "WorldSettings") &&
        worldSettings.TryGetValue(out UObject worldPartition, "WorldPartition") &&
        worldPartition.TryGetValue(out UWorldPartitionRuntimeHashSet runtimeHash, "RuntimeHash")) {
        foreach (var cell in runtimeHash.RuntimeStreamingData[0].SpatiallyLoadedCells) {
            if (cell.ResolvedObject is null) continue;
            var cell2 = cell.ResolvedObject.Load<UWorldPartitionRuntimeLevelStreamingCell>();

            if (cell2 is null || cell2.LevelStreaming is null) continue; 
            var thing = cell2.LevelStreaming.Load<CUE4Parse.UE4.Assets.Exports.WorldPartition.UWorldPartitionLevelStreamingDynamic>();

            if (thing is null || thing.WorldAsset is null) continue;
            if (!thing.WorldAsset!.Value.TryLoad<UWorld>(out UWorld? newWorld) || newWorld is null) continue;
            var newLevel = newWorld.PersistentLevel.Load<ULevel>();

            if (newLevel is null) continue;
            LevelsToSearch.Add(newLevel);
        }
    }

    foreach (var lvl in LevelsToSearch) {
        foreach (var actor in lvl.Actors) {
            if (actor is null || actor.ResolvedObject is null) continue;
            
            var actorType = actor.ResolvedObject.Class?.Name.Text ?? "";
            if (actorType == "CameraActor") {
                var camera = actor.ResolvedObject.Load<AActor>();
                bool correctCam = false;
                if (camera is null) continue;
                if (camera.TryGetValue(out FName[] tags, "Tags")) {
                    foreach (var tag in tags) {
                        if (tag.Text == "MinimapCaptureCamera") {
                            correctCam = true;
                            break;
                        }
                    }
                }

                if (!correctCam) continue;

                if (!camera.TryGetValue(out UObject component, "CameraComponent")) {
                    Console.WriteLine("Failed to get CameraComponent from MinimapCaptureCamera");
                    continue;
                }

                if (!component.TryGetValue(out float OrthoWidth, "OrthoWidth")) {
                    Console.WriteLine("Failed to get OrthoWidth from CameraComponent");
                    continue;
                }

                Map.Camera.Width = OrthoWidth;
                Map.Camera.Position = camera.GetActorLocation();
                Map.Camera.Rotation = camera.GetActorRotation();
            } else {
                var aactor = actor.ResolvedObject.Load<AActor>();
                if (aactor is null || !WhitelistedActorTypes.Contains(actorType)) continue;

                if (!Map.Actors.ContainsKey(actorType)) Map.Actors[actorType] = new();

                Map.Actors[actorType].Add(aactor.GetActorLocation());
            }
        }
    }

    File.WriteAllText(Path.Join(worldOutputPath, $"{Map.DisplayName}.json"), "const data = " + JsonConvert.SerializeObject(Map));
}

var replaced = "";
foreach (var Map in Maps) {
    replaced += $"<a href='World.html?name={Map.DisplayName}'>{Map.DisplayName}</a>\n";
}

File.WriteAllText(Path.Join(outputPath, "Main.html"), $@"
<DOCTYPE html>
<html>
    <head>
        <title>Main</title>
    </head>
    <body>
        REPLACE THIS PLEASE <3
    </body>
</html>
".Replace("REPLACE THIS PLEASE <3", replaced));
