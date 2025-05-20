using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse_Conversion.Textures;

namespace FortMapper
{
    public static class ExportUtils
    {
        public static void ExportTexture2D(UTexture2D texture, string? out_path)
        {
            if (out_path is null)
                out_path = $"./{texture.Name}.png";

            File.WriteAllBytes(out_path, texture.Decode()!.Encode(ETextureFormat.Png, out string ext));
        }

        public static void ExportTexture2D(string texture_path, string? out_path)
        {
            if (!GlobalProvider._provider.TryLoadPackageObject<UTexture2D>(texture_path, out UTexture2D? texture_obj))
                return;

            ExportTexture2D(texture_obj, out_path);
        }

        public static string ValidFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name.Replace(c, '_');
            return name;
        }
    }
}
