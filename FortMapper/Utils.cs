using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse_Conversion.Textures;

public static class Utils {
    public static bool IsFilePath(string path) {
        if (path.EndsWith('/') || path.EndsWith('\\')) return false;

        if (Path.HasExtension(path)) return true;

        return false;
    }

    public static void ExportTexture2D(UTexture2D texture, string outPath) {
        if (!IsFilePath(outPath)) {
            outPath = Path.Join(outPath, $"{texture.Name}.png");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? "");

        var decoded = TextureDecoder.Decode(texture);
        if (decoded is null) return;

        File.WriteAllBytes(outPath, decoded.Encode(ETextureFormat.Png, false, out string ext));
    }
}
