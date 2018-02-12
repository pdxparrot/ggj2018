using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.ggj2018.Data
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
        [Header("FOV")]

        [SerializeField]
        private float _predatorFOV = 60.0f;

        public float PredatorFOV => _predatorFOV;

        [SerializeField]
        private float _preyFOV = 90.0f;

        public float PreyFOV => _preyFOV;
#endregion

        [Space(10)]

#region Layers
        [Header("Layers")]

        [SerializeField]
        private string _predatorLayer;

        public string PredatorLayer => _predatorLayer;

        [SerializeField]
        private string _preyLayer;

        public string PreyLayer => _preyLayer;
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
