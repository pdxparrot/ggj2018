using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

namespace ggj2018.Game.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="ggj2018/Data/Game Data")]
    [Serializable]
    public sealed class GameData : ScriptableObject
    {
        // TODO: this sucks
#region Version
        public const int CurrentVersion = 1;

        [SerializeField]
        private int _version = 1;

        public int Version => _version;

        public bool IsValid => CurrentVersion == _version;
#endregion

        [SerializeField]
        private ScriptableObject[] _data;

        [SerializeField]
        private readonly Dictionary<string, ScriptableObject> _dataCollection = new Dictionary<string, ScriptableObject>();

        public IReadOnlyDictionary<string, ScriptableObject> Data => _dataCollection;

        public void Initialize()
        {
            foreach(ScriptableObject data in _data) {
                IData idata = data as IData;
                if(null == idata) {
                    Debug.LogError($"Data object {data.name} does not implement IData interface!");
                    continue;
                }

                _dataCollection[idata.Name] = data;
                idata.Initialize();
            }
        }

        public void DebugDump()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Game Data:");

            builder.AppendLine($"Version: {Version} ({CurrentVersion}), valid: {IsValid}");

            Debug.Log(builder.ToString());
        }
    }
}
