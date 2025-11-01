using Newtonsoft.Json;
using CUE4Parse.UE4.Objects.Core.Math;

public class FortMapInfoCamera {
    public float Width;
    public FVector Position;
    public FRotator Rotation;
}

public class FortMapInfo {
    public required string DisplayName;
    [JsonIgnore]
    public required string LevelPath; // TODO Get it from OverrideMinimapMaterial in FortWorldSettings???
    [JsonIgnore]
    public required string MinimapPath;
    public Dictionary<string, List<FVector>> Actors = new();
    public FortMapInfoCamera Camera = new();
}
