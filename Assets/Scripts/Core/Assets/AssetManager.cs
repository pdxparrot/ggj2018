using System.Collections;
using System.Collections.Generic;

using ggj2018.Core.Util;

using JetBrains.Annotations;

using UnityEngine;

namespace ggj2018.Core.Assets
{
    public sealed class AssetManager : SingletonBehavior<AssetManager>
    {
#if UNITY_EDITOR
        private const string UseAssetBundlesKey = "useAssetBundles";
        private const string UseAssetBundlesMenuItemName = "ggj2018/Use Asset Bundles";

        public static bool UseAssetBundles { get; private set; }

        static AssetManager()
        {
            UnityEditor.EditorApplication.delayCall += () => {
                UseAssetBundles = UnityEditor.EditorPrefs.GetBool(UseAssetBundlesKey, false);
                UnityEditor.Menu.SetChecked(UseAssetBundlesMenuItemName, UseAssetBundles);
                Debug.Log($"Use Asset Bundles: {UseAssetBundles}");
            };
        }

        [UnityEditor.MenuItem(UseAssetBundlesMenuItemName)]
        private static void UseAssetBundlesMenuItem()
        {
            UseAssetBundles = !UseAssetBundles;
            UnityEditor.Menu.SetChecked(UseAssetBundlesMenuItemName, UseAssetBundles);
            UnityEditor.EditorPrefs.SetBool(UseAssetBundlesKey, UseAssetBundles);
        }
#endif

        [SerializeField]
        private string _assetManifestBundleName = "asset_manifest";

        [SerializeField]
        private string _assetManifestPath = "AssetManifest.asset";

        [CanBeNull]
        private AssetBundle _assetManifestBundle;

        private AssetManifest _assetManifest;

        // asset bundle name => asset bundle
        private readonly Dictionary<string, AssetBundle> _loadedAssetBundles = new Dictionary<string, AssetBundle>();

        public IEnumerator InitializeRoutine()
        {
            IEnumerator runner = LoadAssetManifestRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            runner = LoadAssetBundlesRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }
        }

        private IEnumerator LoadAssetManifestRoutine()
        {
            IEnumerator<AssetBundle> loader = LoadAssetBundleFromFileRoutine(_assetManifestBundleName);
            while(loader.MoveNext()) {
                yield return null;
            }
            _assetManifestBundle = loader.Current;

#if UNITY_EDITOR
            if(!UseAssetBundles) {
                _assetManifest = LoadAssetFromBundle<AssetManifest>(_assetManifestBundleName, _assetManifestPath);
            } else {
#endif
            _assetManifest = LoadAssetFromBundle<AssetManifest>(_assetManifestBundle, _assetManifestPath);
#if UNITY_EDITOR
            }
#endif

            if(null == _assetManifest) {
                Debug.LogError("Unable to load asset manifest!");
            }
        }

        private IEnumerator LoadAssetBundlesRoutine()
        {
            foreach(string assetBundleName in _assetManifest.AssetBundles) {
                IEnumerator<AssetBundle> loader = LoadAssetBundleFromFileRoutine(assetBundleName);
                while(loader.MoveNext()) {
                    yield return null;
                }

                AssetBundle assetBundle = loader.Current;
                if(null == assetBundle) {
#if UNITY_EDITOR
                    if(UseAssetBundles) {
#endif
                    Debug.LogError($"Unable to load asset bundle {assetBundleName}!");
#if UNITY_EDITOR
                    }
#endif
                    continue;
                }

                _loadedAssetBundles.Add(assetBundleName, assetBundle);
            }
        }

        public T LoadAsset<T>(string path) where T: UnityEngine.Object
        {
            foreach(string assetBundleName in _assetManifest.AssetBundles) {
                var asset = LoadAssetFromBundle<T>(assetBundleName, path);
                if(null != asset) {
                    return asset;
                }
            }
            return null;
        }

        public void UnloadAssets(bool unloadManifest=false)
        {
            foreach(var kvp in _loadedAssetBundles) {
                kvp.Value.Unload(true);
            }
            _loadedAssetBundles.Clear();

            if(unloadManifest) {
                _assetManifest = null;
                _assetManifestBundle?.Unload(true);
                _assetManifestBundle = null;
            }
        }

        private IEnumerator<AssetBundle> LoadAssetBundleFromFileRoutine(string assetBundleName)
        {
            Debug.Log($"Loading asset bundle {assetBundleName} from file...");

#if UNITY_EDITOR
            if(!UseAssetBundles) {
                yield break;
            }
#endif

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(assetBundleName);
            while(!request.isDone) {
                yield return null;
            }
            yield return request.assetBundle;
        }

        private T LoadAssetFromBundle<T>(string assetBundleName, string path) where T: UnityEngine.Object
        {
            //Debug.Log($"Loading asset {path} from bundle {assetBundleName}...");

#if UNITY_EDITOR
            if(!UseAssetBundles) {
                string fullPath = $"Assets/Data/{assetBundleName}/{path}";
                //Debug.Log($"Editor loading asset from disk at {fullPath}...");
                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
            }
#endif

            AssetBundle assetBundle = _loadedAssetBundles.GetOrDefault(assetBundleName);
            return LoadAssetFromBundle<T>(assetBundle, path);
        }

        private T LoadAssetFromBundle<T>(AssetBundle assetBundle, string path) where T: UnityEngine.Object
        {
            return assetBundle?.LoadAsset<T>(path);
        }
    }
}
