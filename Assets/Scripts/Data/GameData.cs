using System;
using System.Text;

using UnityEngine;

namespace ggj2018.Data
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
        private AudioData _audio;

        public AudioData Audio => _audio;

        public void Initialize()
        {
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
