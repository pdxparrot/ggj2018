using System;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.Assets
{
    [CreateAssetMenu(fileName="AssetManifest", menuName="ggj2018/Data/Asset Manifest")]
    [Serializable]
    public sealed class AssetManifest : ScriptableObject
    {
#region UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/ggj2018/Data/Asset Manifest")]
        private static void Create()
        {
            ScriptableObjectUtility.CreateAsset<AssetManifest>();
        }
#endregion
    }
}
