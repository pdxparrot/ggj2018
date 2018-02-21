using System;

using pdxpartyparrot.Core.Input;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.Data
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

        [SerializeField]
        private int _targetFrameRate = 60;

        public int TargetFrameRate => _targetFrameRate;

        [SerializeField]
        private bool _enableNetwork;

        public bool EnableNetwork => _enableNetwork;

        public int MaxLocalPlayers => (EnableGVR || EnableNetwork) ? 1 : InputManager.Instance.MaxControllers;

        [SerializeField]
        private string _godRayAlphaProperty = "_Usealphatexture";

        public string GodRayAlphaProperty => _godRayAlphaProperty;
    }
}
