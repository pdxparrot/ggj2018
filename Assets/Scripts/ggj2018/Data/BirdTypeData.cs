using System;

using ggj2018.ggj2018.Birds;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="BirdTypeData", menuName="ggj2018/Data/Bird Type Data")]
    [Serializable]
    public sealed class BirdTypeData : ScriptableObject
    {
        [SerializeField]
        private string _id;

        public string Id => _id;

        [SerializeField]
        private string _name;

        public string Name => _name;

        [SerializeField]
        private Sprite _icon;

        public Sprite Icon => _icon;

        [SerializeField]
        private Bird _modelPrefab;

        public Bird ModelPrefab => _modelPrefab;

        [SerializeField]
        private BirdPhysicsData _physics;

        public BirdPhysicsData Physics => _physics;

#region Bird Type

        [SerializeField]
        private bool _isPredator;

        public bool IsPredator => _isPredator;

        public bool IsPrey => !_isPredator;
#endregion

        [Space(10)]

#region Viewer
        [Header("Viewer")]

        [SerializeField]
        private float _followOrbitRadius = 5.0f;

        public float FollowOrbitRadius => _followOrbitRadius;
#endregion

        [Space(10)]

#region Abilities
        [Header("Abilities")]

        [SerializeField]
        private bool _canBoost;

        public bool CanBoost => _canBoost;
#endregion

        [Space(10)]

#region Audio
        [Header("Audio")]

        [SerializeField]
        private AudioClip _spawnAudioClip;

        public AudioClip SpawnAudioClip => _spawnAudioClip;

        [SerializeField]
        private AudioClip _flightAudioClip;

        public AudioClip FlightAudioClip => _flightAudioClip;

        [SerializeField]
        private AudioClip _winAudioClip;

        public AudioClip WinAudioClip => _winAudioClip;

        [SerializeField]
        private AudioClip _lossAudioClip;

        public AudioClip LossAudioClip => _lossAudioClip;
#endregion

        private BirdData _birdData;

        public float ViewFOV => IsPredator
            ? _birdData.PredatorFOV
            : _birdData.PreyFOV;

        public string Layer => IsPredator
            ? _birdData.PredatorLayer
            : _birdData.PreyLayer;

        // TODO: rename this
        public string OtherLayer => IsPrey
            ? _birdData.PredatorLayer
            : _birdData.PreyLayer;

        public PostProcessProfile PostProcessProfile => IsPredator
            ? _birdData.PredatorPostProcessProfile
            : _birdData.PreyPostProcessProfile;

        public void Initialize(BirdData birdData)
        {
            _birdData = birdData;
        }
    }
}
