using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Assets
{
    public sealed class AssetManager : SingletonBehavior<AssetManager>
    {
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

// TODO: asset bundles
    }
}
