using System;
using System.Collections.Generic;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.GameTypes;

using JetBrains.Annotations;

using UnityEngine;

namespace ggj2018.ggj2018.World
{
    public sealed class SpawnManager : SingletonBehavior<SpawnManager>
    {
        private static readonly System.Random Random = new System.Random();

        private class SpawnPoints
        {
            private readonly GameType.GameTypes _gameType;

// TODO: dictionary would work better
            private readonly List<SpawnPoint> _predatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPredatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _preySpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPreySpawnPoints = new List<SpawnPoint>();

            public SpawnPoints(GameType.GameTypes gameType)
            {
                _gameType = gameType;
            }

            public void Add(SpawnPoint spawnPoint)
            {
                switch(spawnPoint.Type)
                {
                case SpawnPoint.SpawnPointType.Predator:
                    _predatorSpawnPoints.Add(spawnPoint);
                    break;
                case SpawnPoint.SpawnPointType.Prey:
                    _preySpawnPoints.Add(spawnPoint);
                    break;
                default:
                    Debug.LogError($"TODO: support spawn point type {spawnPoint.Type}");
                    break;
                }
            }

            public void Remove(SpawnPoint spawnPoint)
            {
                switch(spawnPoint.Type)
                {
                case SpawnPoint.SpawnPointType.Predator:
                    _predatorSpawnPoints.Remove(spawnPoint);
                    _usedPredatorSpawnPoints.Remove(spawnPoint);
                    break;
                case SpawnPoint.SpawnPointType.Prey:
                    _preySpawnPoints.Remove(spawnPoint);
                    _usedPreySpawnPoints.Remove(spawnPoint);
                    break;
                default:
                    Debug.LogError($"TODO: support spawn point type {spawnPoint.Type}");
                    break;
                }
            }

            [CanBeNull]
            public SpawnPoint GetSpawnPoint(BirdData.BirdDataEntry birdType)
            {
                return birdType.IsPredator
                    ? GetPredatorSpawnPoint(GameManager.Instance.State.GameType)
                    : GetPreySpawnPoint(GameManager.Instance.State.GameType);
            }

            [CanBeNull]
            private SpawnPoint GetPredatorSpawnPoint(GameType gameType)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints);
                if(null != spawnPoint) {
                    return spawnPoint;
                }
                return gameType.BirdTypesShareSpawnpoints ? GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints) : null;
            }

            [CanBeNull]
            private SpawnPoint GetPreySpawnPoint(GameType gameType)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints);
                if(null != spawnPoint) {
                    return spawnPoint;
                }
                return gameType.BirdTypesShareSpawnpoints ? GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints) : null;
            }

            [CanBeNull]
            private SpawnPoint GetSpawnPoint(IList<SpawnPoint> from, IList<SpawnPoint> to)
            {
                if(from.Count < 1) {
                    return null;
                }

                SpawnPoint spawnPoint = Random.RemoveRandomEntry(from);
                to.Add(spawnPoint);
                return spawnPoint;
            }

            public void Reset()
            {
                Debug.Log($"Resetting used spawnpoints for game type {_gameType}");

                _predatorSpawnPoints.AddRange(_usedPredatorSpawnPoints);
                _usedPredatorSpawnPoints.Clear();

                _preySpawnPoints.AddRange(_usedPreySpawnPoints);
                _usedPreySpawnPoints.Clear();
            }
        }

        // game type => spawnpoints
        private readonly Dictionary<GameType.GameTypes, SpawnPoints> _spawnPoints = new Dictionary<GameType.GameTypes, SpawnPoints>();

#region Registration
        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(GameType.GameTypes gameType in spawnPoint.GameTypes) {
                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameType);
                if(null == spawnPoints) {
                    spawnPoints = new SpawnPoints(gameType);
                    _spawnPoints.Add(gameType, spawnPoints);
                }
                spawnPoints.Add(spawnPoint);
            }
        }

        public void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(GameType.GameTypes gameType in spawnPoint.GameTypes) {
                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameType);
                spawnPoints?.Remove(spawnPoint);
            }
        }
#endregion

        [CanBeNull]
        public SpawnPoint GetSpawnPoint(GameType.GameTypes gameType, BirdData.BirdDataEntry birdType)
        {
            SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameType);
            return spawnPoints?.GetSpawnPoint(birdType);
        }

        public void ReleaseSpawnPoints()
        {
            foreach(var kvp in _spawnPoints) {
                kvp.Value.Reset();
            }
        }
    }
}
