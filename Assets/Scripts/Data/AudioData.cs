using System;
using System.Collections.Generic;

using ggj2018.Util;

using UnityEngine;

namespace ggj2018.Data
{
    [CreateAssetMenu(fileName="AudioData", menuName="ggj2018/Data/Audio Data")]
    [Serializable]
    public sealed class AudioData : ScriptableObject
    {
#region UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/ggj2018/Data/Audio Data")]
        private static void Create()
        {
            ScriptableObjectUtility.CreateAsset<AudioData>();
        }
#endregion

        [Serializable]
        public sealed class AudioDataEntry
        {
            [SerializeField]
            private string _id;

            public string Id => _id;

            [SerializeField]
            private AudioClip _audioClip;

            public AudioClip AudioClip => _audioClip;

            public override string ToString()
            {
                return $"Armor({Id}: {_audioClip.name})";
            }
        }

        [SerializeField]
        private AudioDataEntry[] _audio;

        public IReadOnlyCollection<AudioDataEntry> Audio => _audio;

        private readonly Dictionary<string, AudioDataEntry> _entries = new Dictionary<string, AudioDataEntry>();

        public IReadOnlyDictionary<string, AudioDataEntry> Entries => _entries;

        public void Initialize()
        {
            foreach(AudioDataEntry entry in Audio) {
                _entries.Add(entry.Id, entry);
            }
        }
    }
}
