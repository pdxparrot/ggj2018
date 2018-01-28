using System;
using System.Collections.Generic;

using ggj2018.Core.Util;

using JetBrains.Annotations;

namespace ggj2018.ggj2018
{
    public sealed class SpawnManager : SingletonBehavior<SpawnManager>
    {
        private readonly List<SpawnPoint> _predatorSpawnPoints = new List<SpawnPoint>();

        private readonly List<SpawnPoint> _usedPredatorSpawnPoints = new List<SpawnPoint>();

        private readonly List<SpawnPoint> _preySpawnPoints = new List<SpawnPoint>();

        private readonly List<SpawnPoint> _usedPreySpawnPoints = new List<SpawnPoint>();

        private readonly Random _random = new Random();

        public void RegisterSpawnPoint(SpawnPoint spawnPoint)
        {
            if(spawnPoint.IsPredatorSpawn) {
                _predatorSpawnPoints.Add(spawnPoint);
            } else {
                _preySpawnPoints.Add(spawnPoint);
            }
        }

        public void UnregisterSpawnPoint(SpawnPoint spawnPoint)
        {
            if(spawnPoint.IsPredatorSpawn) {
                _predatorSpawnPoints.Remove(spawnPoint);
            } else {
                _preySpawnPoints.Remove(spawnPoint);
            }
        }

        [CanBeNull]
        public SpawnPoint GetSpawnPoint(BirdType birdType)
        {
            return birdType.BirdDataEntry.IsPredator ? GetPredatorSpawnPoint() : GetPreySpawnPoint();
        }

        [CanBeNull]
        private SpawnPoint GetPredatorSpawnPoint()
        {
            return GetSpawnPoint(_predatorSpawnPoints, _usedPredatorSpawnPoints);
        }

        [CanBeNull]
        private SpawnPoint GetPreySpawnPoint()
        {
            return GetSpawnPoint(_preySpawnPoints, _usedPreySpawnPoints);
        }

        private SpawnPoint GetSpawnPoint(IList<SpawnPoint> from, IList<SpawnPoint> to)
        {
            if(from.Count < 1) {
                return null;
            }

            SpawnPoint spawnPoint = _random.RemoveRandomEntry(from);
            to.Add(spawnPoint);
            return spawnPoint;
        }
    }
}
