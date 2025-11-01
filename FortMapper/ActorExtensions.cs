using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Component;

public static class ActorExtensions {
    public static FVector GetActorLocation(this AActor actor) {
        if (actor.TryGetValue(out USceneComponent component, "RootComponent")) {
            return component.GetComponentTransform().Translation;
        }

        return FVector.ZeroVector;
    }

    public static FRotator GetActorRotation(this AActor actor) {
        if (actor.TryGetValue(out USceneComponent component, "RootComponent")) {
            return component.GetComponentTransform().Rotator();
        }

        return FRotator.ZeroRotator;
    }
}
