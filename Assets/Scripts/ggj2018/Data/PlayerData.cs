using System;

using ggj2018.Game.Data;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="ggj2018/Data/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject, IData
    {
        public const string DataName = "player";

        public string Name => DataName;

        [SerializeField]
        private float _baseSpeed = 1.0f;

        public float BaseSpeed => _baseSpeed;

        [SerializeField]
        private float _baseTurnSpeed = 5.0f;

        public float BaseTurnSpeed => _baseTurnSpeed;

        [SerializeField]
        private float _basePitchSpeed = 5.0f;

        public float BasePitchSpeed => _basePitchSpeed;

        public void Initialize()
        {
        }
    }
}
