using System.Collections.Generic;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Data;

using JetBrains.Annotations;
using UnityEngine;
using Random = System.Random;

namespace ggj2018.ggj2018
{
    public sealed class SpawnManager : SingletonBehavior<SpawnManager>
    {
        private static readonly Random Random = new Random();

        private class SpawnPoints
        {
            private readonly string _gameTypeId;

            private readonly List<SpawnPoint> _predatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPredatorSpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _preySpawnPoints = new List<SpawnPoint>();

            private readonly List<SpawnPoint> _usedPreySpawnPoints = new List<SpawnPoint>();

            public SpawnPoints(string gameTypeId)
            {
                _gameTypeId = gameTypeId;
            }

            public void Add(SpawnPoint spawnPoint)
            {
                if(spawnPoint.IsPredatorSpawn) {
                    _predatorSpawnPoints.Add(spawnPoint);
                } else {
                    _preySpawnPoints.Add(spawnPoint);
                }
            }

            public void Remove(SpawnPoint spawnPoint)
            {
                if(spawnPoint.IsPredatorSpawn) {
                    _predatorSpawnPoints.Remove(spawnPoint);
                    _usedPredatorSpawnPoints.Remove(spawnPoint);
                } else {
                    _preySpawnPoints.Remove(spawnPoint);
                    _usedPreySpawnPoints.Remove(spawnPoint);
                }
            }

            [CanBeNull]
            public SpawnPoint GetSpawnPoint(BirdType birdType)
            {
                GameTypeData.GameTypeDataEntry gameTypeData = GameManager.Instance.GameTypeData.Entries.GetOrDefault(_gameTypeId);
                if(null == gameTypeData) {
                    Debug.LogError($"No such game type {_gameTypeId}!");
                    return null;
                }
                return birdType.BirdDataEntry.IsPredator ? GetPredatorSpawnPoint(gameTypeData) : GetPreySpawnPoint(gameTypeData);
            }

            [CanBeNull]
            private SpawnPoint GetPredatorSpawnPoint(GameTypeData.GameTypeDataEntry gameTypeData)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints);
                if(null == spawnPoint && !gameTypeData.BirdTypesShareSpawnPoints) {
                    return null;
                }
                return GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints);
            }

            [CanBeNull]
            private SpawnPoint GetPreySpawnPoint(GameTypeData.GameTypeDataEntry gameTypeData)
            {
                SpawnPoint spawnPoint = GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints);
                if(null == spawnPoint && !gameTypeData.BirdTypesShareSpawnPoints) {
                    return null;
                }
                return GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints);
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
                _predatorSpawnPoints.AddRange(_usedPredatorSpawnPoints);
                _usedPredatorSpawnPoints.Clear();

                _preySpawnPoints.AddRange(_usedPreySpawnPoints);
                _usedPreySpawnPoints.Clear();
            }
        }

        // game type => spawnpoints
        private readonly Dictionary<string, SpawnPoints> _spawnPoints = new Dictionary<string, SpawnPoints>();

#region Registration
        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(string gameTypeId in spawnPoint.GameTypeIds) {
                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeId);
                if(null == spawnPoints) {
                    spawnPoints = new SpawnPoints(gameTypeId);
                    _spawnPoints.Add(gameTypeId, spawnPoints);
                }
                spawnPoints.Add(spawnPoint);
            }
        }

        public void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            foreach(string gameTypeId in spawnPoint.GameTypeIds) {
                SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeId);
                spawnPoints?.Remove(spawnPoint);
            }
        }
#endregion

        [CanBeNull]
        public SpawnPoint GetSpawnPoint(string gameTypeId, BirdType birdType)
        {
            SpawnPoints spawnPoints = _spawnPoints.GetOrDefault(gameTypeId);
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
