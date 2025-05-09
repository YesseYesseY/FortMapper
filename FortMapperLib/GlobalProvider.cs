using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUE4Parse.Compression;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.FileProvider.Vfs;
using CUE4Parse.MappingsProvider;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;

namespace FortMapper
{
    public static class GlobalProvider
    {
        public static DefaultFileProvider _provider = new DefaultFileProvider(@"C:\Program Files\Epic Games\Fortnite\FortniteGame\Content\Paks", SearchOption.AllDirectories, new VersionContainer(EGame.GAME_UE5_LATEST), StringComparer.OrdinalIgnoreCase);
        public static void Init()
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);

            _provider.MappingsContainer = new FileUsmapTypeMappingsProvider("./mappings.usmap");
            _provider.Initialize();
            _provider.SubmitKey(new FGuid(), new FAesKey("0x17243B0E3E66DA90347F7C4787692505EC5E5285484633D71B09CD6ABB714E9B"));
            _provider.PostMount();
            _provider.LoadVirtualPaths();
        }

        public static FileProviderDictionary Files => _provider.Files;
        public static UObject LoadPackageObject(string path) => _provider.LoadPackageObject(path);
        public static T LoadPackageObject<T>(string path) where T : UObject => _provider.LoadPackageObject<T>(path);
    }
}

