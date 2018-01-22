using System;
using System.Collections.Generic;

using UnityEngine;

namespace ggj2018.Core.Assets
{
    [CreateAssetMenu(fileName="AssetManifest", menuName="ggj2018/Data/Asset Manifest")]
    [Serializable]
    public sealed class AssetManifest : ScriptableObject
    {
        [SerializeField]
        private string[] _assetBundles;

        public IReadOnlyCollection<string> AssetBundles => _assetBundles;
    }
}
