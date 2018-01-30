using System;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="ConfigData", menuName="ggj2018/Data/Config Data")]
    [Serializable]
    public sealed class ConfigData : ScriptableObject
    {
#region VR
        [SerializeField]
        private bool _enableGVR;

        public bool EnableGVR => _enableGVR;

        public bool EnableVR => EnableGVR;
#endregion
    }
}
