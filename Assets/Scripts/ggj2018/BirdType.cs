using System;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    [Serializable]
    public class BirdType
    {
        [SerializeField]
        [ReadOnly]
        private BirdData.BirdDataEntry _birdDataEntry;

        public BirdData.BirdDataEntry BirdDataEntry => _birdDataEntry;

        public BirdType(string id)
        {
            _birdDataEntry = GameManager.Instance.BirdData.Entries.GetOrDefault(id);
        }
    }
}
