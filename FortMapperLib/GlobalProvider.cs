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
using CUE4Parse_Conversion.Textures.BC;

namespace FortMapper
{
    public static class GlobalProvider
    {
        public static DefaultFileProvider _provider = new DefaultFileProvider(@"C:\Program Files\Epic Games\Fortnite\FortniteGame\Content\Paks", SearchOption.AllDirectories, new VersionContainer(EGame.GAME_UE5_LATEST), StringComparer.OrdinalIgnoreCase);
        public static void Init()
        {
            OodleHelper.DownloadOodleDll();
            OodleHelper.Initialize(OodleHelper.OODLE_DLL_NAME);
            DetexHelper.LoadDll();
            DetexHelper.Initialize(DetexHelper.DLL_NAME);

            _provider.MappingsContainer = new FileUsmapTypeMappingsProvider("./mappings.usmap");
            _provider.Initialize();
            var game_custom_path = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FortniteGame", "Saved", "PersistentDownloadDir", "GameCustom", "InstalledBundles");
            var dash_berry_path = Path.Join(game_custom_path, "d27febeb-d6db-4cdc-8b53-d9958a212787");
            if (Directory.Exists(dash_berry_path))
                _provider.RegisterVfs(Path.Join(dash_berry_path, "plugin.utoc"));
            dash_berry_path = Path.Join(game_custom_path, "6d357f46-2a0f-433d-893b-228a8d7b1362");
            if (Directory.Exists(dash_berry_path))
                _provider.RegisterVfs(Path.Join(dash_berry_path, "plugin.utoc"));
            dash_berry_path = Path.Join(game_custom_path, "9e025f27-5750-43bb-b0dd-052b55a99d35");
            if (Directory.Exists(dash_berry_path))
                _provider.RegisterVfs(Path.Join(dash_berry_path, "plugin.utoc"));
            _provider.SubmitKey(new FGuid(), new FAesKey("0x67E992943B63878FEF3C02DE9E0100C127A6C34A569231ED153E03E6CDB0F5A2"));
            _provider.PostMount();
            _provider.LoadVirtualPaths();
        }

        public static FileProviderDictionary Files => _provider.Files;
        public static UObject LoadPackageObject(string path) => _provider.LoadPackageObject(path);
        public static T LoadPackageObject<T>(string path) where T : UObject => _provider.LoadPackageObject<T>(path);
    }
}

