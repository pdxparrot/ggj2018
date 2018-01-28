using System.Collections.Generic;

using ggj2018.Core.Util;

namespace ggj2018.ggj2018
{
    public sealed class SpawnManager : SingletonBehavior<SpawnManager>
    {
        private List<SpawnPoint> _predatorSpawnPoints = new List<SpawnPoint>();
        private List<SpawnPoint> _preySpawnPoints = new List<SpawnPoint>();

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

        public SpawnPoint GetSpawnPoint(IPlayer player)
        {
            if(player.State.BirdType.BirdDataEntry.IsPredator) {
                return _predatorSpawnPoints[0];
            }
            return _predatorSpawnPoints[0];
        }
    }
}
