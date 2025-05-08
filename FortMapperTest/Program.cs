using CUE4Parse.UE4.Assets.Exports.Actor;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse.Utils;
using CUE4Parse_Conversion.Textures;
using SkiaSharp;
using System.IO;
using System.Numerics;

FortMapper.LootExport.Yes(
    "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/FigmentLootTierData.FigmentLootTierData", 
    "FortniteGame/Plugins/GameFeatures/Figment/Figment_LootTables/Content/DataTables/FigmentLootPackages.FigmentLootPackages"
);

#if false
var yes = FortMapper.WorldExport.Yes("FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_Map/Content/Athena_Terrain_S03.Athena_Terrain_S03",
    "FortniteGame/Plugins/GameFeatures/Figment/Figment_S03_MapUI/Content/MiniMapAthena_S03.MiniMapAthena_S03");

if (yes is null || yes.MinimapTexture is null)
{
    Console.WriteLine(":(");
    return;
}

float ManualRotation = 90.0f;

Vector2 GetMapPos(Vector3 pos)
{
    Vector2 relative = new(pos.X - yes.Camera_Pos.X, pos.Y - yes.Camera_Pos.Y);

    float rot = -(yes.Camera_Rot.Z + ManualRotation).ToRadians();
    float cos = MathF.Cos(rot);
    float sin = MathF.Sin(rot);

    float rotatedX = relative.X * cos - relative.Y * sin;
    float rotatedY = relative.X * sin + relative.Y * cos;

    return new Vector2(
        (rotatedX / yes.Camera_OrthoWidth + 0.5f) * 2048,
        (rotatedY / yes.Camera_OrthoWidth + 0.5f) * 2048
    );
}

var bmp = yes.MinimapTexture.ToSkBitmap();
using (var canvas = new SKCanvas(bmp))
{
    var paint = new SKPaint
    {
        Color = SKColors.Red,
        IsAntialias = true,
        Style = SKPaintStyle.Fill,
        StrokeWidth = 8
    };

    foreach (var pos in yes.Actors)
    {
        var mappos = GetMapPos(pos);
        canvas.DrawCircle(mappos.X, mappos.Y, 2.5f, paint);
    }

    using (var image = SKImage.FromBitmap(bmp))
    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
    using (var stream = File.OpenWrite($"map.png"))
    {
        data.SaveTo(stream);
    }
}
#endif