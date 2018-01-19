using System.Collections;
using System.Collections.Generic;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Assets
{
// https://unity3d.com/learn/tutorials/topics/best-practices/assetbundle-fundamentals?playlist=30089

/*
TODO:

need to load assets without bundles
need to be able to build bundles
need to be able to load from bundles

Resources should *not* be used unless absolutely necessary
 */

    public sealed class AssetManager : SingletonBehavior<AssetManager>
    {
        private static string AssetBundlePath(string name)
        {
            return name;
        }

        [SerializeField]
        private string _baseAssetBundleName = "base";

        [SerializeField]
        [ReadOnly]
        private AssetBundle _baseAssetBundle;

        private readonly List<AssetBundle> _loadedAssetBundles = new List<AssetBundle>();

        [SerializeField]
        [ReadOnly]
        private AssetManifest _baseManifest;

        private readonly Dictionary<string, AssetManifest> _assetManifests = new Dictionary<string, AssetManifest>();

        private readonly Dictionary<string, AssetBundle> _assets = new Dictionary<string, AssetBundle>();

        public IEnumerator LoadAssetsRoutine()
        {
            IEnumerator routine = LoadBaseManifestRoutine();
            while(routine.MoveNext()) {
                yield return null;
            }
        }

        private IEnumerator LoadBaseManifestRoutine()
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(AssetBundlePath(_baseAssetBundleName));
            while(!request.isDone) {
                yield return null;
            }

            if(null == request.assetBundle) {
                Debug.LogError("Unable to load base asset bundle!");
                yield break;
            }

            AssetBundle baseAssetBundle = request.assetBundle;
            _baseManifest = baseAssetBundle.LoadAsset<AssetManifest>("AssetManifest.asset");

            var assetNames = baseAssetBundle.GetAllAssetNames();
            foreach(string assetName in assetNames) {
                _assets.Add(assetName, baseAssetBundle);
            }
        }

        public void UnloadAssets()
        {
            foreach(AssetBundle assetBundle in _loadedAssetBundles) {
                assetBundle.Unload(true);
            }
            _loadedAssetBundles.Clear();
        }

#region Resources
        public GameObject LoadResourcePrefab(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        public T LoadResourcePrefabComponent<T>(string path) where T: Component
        {
            GameObject prefab = LoadResourcePrefab(path);
            return prefab?.GetComponent<T>();
        }

        public GameObject LoadAndInstantiateResourcePrefab(string path)
        {
            GameObject prefab = LoadResourcePrefab(path);
            return Instantiate(prefab);
        }

        public T LoadAndInstantiatePrefab<T>(string path) where T: Component
        {
            GameObject instance = LoadAndInstantiateResourcePrefab(path);
            return instance?.GetComponent<T>();
        }

        public T LoadResourceScriptableObject<T>(string path) where T: ScriptableObject
        {
            return Resources.Load<T>(path);
        }
#endregion
    }
}
