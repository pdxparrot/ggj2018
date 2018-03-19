using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace pdxpartyparrot.ggj2018.Data
{
    [CreateAssetMenu(fileName="BirdData", menuName="ggj2018/Data/Bird Data")]
    [Serializable]
    public sealed class BirdData : ScriptableObject
    {
#region Flight
        [Header("Flight")]

        [SerializeField]
        private float _maxAttackAngle = 45.0f;

        public float MaxAttackAngle => _maxAttackAngle;

        [SerializeField]
        private float _maxBankAngle = 45.0f;

        public float MaxBankAngle => _maxBankAngle;
#endregion

        [Space(10)]

#region FOVs
        [Header("Predator FOV")]

        [SerializeField]
        private float _predatorFOV = 60.0f;

        public float PredatorFOV => _predatorFOV;

        [SerializeField]
        private float _predatorBoostFOVChange = -15.0f;

        public float PredatorBoostFOVChange => _predatorBoostFOVChange;

        [SerializeField]
        private float _predatorBrakeFOVChange = 15.0f;

        public float PredatorBrakeFOVChange => _predatorBrakeFOVChange;

        [Header("Prey FOV")]

        [SerializeField]
        private float _preyFOV = 90.0f;

        public float PreyFOV => _preyFOV;

        [SerializeField]
        private float _preyBoostFOVChange = -15.0f;

        public float PreyBoostFOVChange => _preyBoostFOVChange;

        [SerializeField]
        private float _preyBrakeFOVChange = 15.0f;

        public float PreyBrakeFOVChange => _preyBrakeFOVChange;
#endregion

        [Space(10)]

#region Layers
// TODO: player/render layers can probably be combined here

        [Header("Layers")]

        [SerializeField]
        private string _predatorPlayerLayer;

        public LayerMask PredatorPlayerLayer => LayerMask.NameToLayer(_predatorPlayerLayer);

        [SerializeField]
        private string _predatorRenderLayer;

        public LayerMask PredatorRenderLayer => LayerMask.NameToLayer(_predatorRenderLayer);

        [SerializeField]
        private string _preyPlayerLayer;

        public LayerMask PreyPlayerLayer => LayerMask.NameToLayer(_preyPlayerLayer);

        [SerializeField]
        private string _preyRenderLayer;

        public LayerMask PreyRenderLayer => LayerMask.NameToLayer(_preyRenderLayer);
#endregion

        [Space(10)]

#region Post Processing
        [Header("Post Processing")]

        [SerializeField]
        private PostProcessProfile _predatorPostProcessProfile;

        public PostProcessProfile PredatorPostProcessProfile => _predatorPostProcessProfile;

        [SerializeField]
        private PostProcessProfile _preyPostProcessProfile;

        public PostProcessProfile PreyPostProcessProfile => _preyPostProcessProfile;
#endregion

        [Space(10)]

        [Header("Birds")]

        [SerializeField]
        private BirdTypeData[] _birds;

        public IReadOnlyCollection<BirdTypeData> Birds => _birds;

        public void Initialize()
        {
            foreach(BirdTypeData bird in Birds) {
                bird.Initialize(this);
            }
        }
    }
}
