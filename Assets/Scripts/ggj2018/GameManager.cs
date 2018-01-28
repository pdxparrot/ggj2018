using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GameManager : SingletonBehavior<GameManager>
    {
        [SerializeField]
        private int _maxPlayers = 4;

        public int MaxPlayers => _maxPlayers;

        [SerializeField]
        private BirdData _birdData;

        public BirdData BirdData => _birdData;

        public void Initialize()
        {
            _birdData.Initialize();
        }
    }
}
