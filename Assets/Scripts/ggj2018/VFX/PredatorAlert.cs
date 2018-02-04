using ggj2018.Core.Util;
using ggj2018.Core.VFX;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.ggj2018.VFX
{
    [RequireComponent(typeof(Bird))]
    [RequireComponent(typeof(PostProcessVolumeSelector))]
    public class PredatorAlert : MonoBehavior
    {
        [SerializeField]
        [ReadOnly]
        private float _defaultIntensity;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _maxIntensity;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _intensityRate;

        private Bird _bird;

        private PostProcessVolume _postProcessVolume;

        private Vignette _vignetteSettings;

#region Unity Lifecycle
        private void Awake()
        {
            _bird = GetComponent<Bird>();
            if(_bird.Type.IsPredator) {
                enabled = false;
                return;
            }

            _postProcessVolume = GetComponent<PostProcessVolumeSelector>().PostProcessVolume;
            if(_postProcessVolume?.profile.TryGetSettings(out _vignetteSettings) ?? false) {
                _defaultIntensity = _vignetteSettings.intensity;
            }
        }

        private void Update()
        {
            float distance;
            Player predator = PlayerManager.Instance.GetNearestPredator(_bird.Owner, out distance);
            if(null == predator) {
                return;
            }

            float dt = Time.deltaTime;

            if(distance <= GameManager.Instance.GameTypeData.HawkAlertDistance) {
                if(_vignetteSettings.intensity < _maxIntensity) {
                    _vignetteSettings.intensity.value = Mathf.Clamp(_vignetteSettings.intensity.value + (_intensityRate * dt), _defaultIntensity, _maxIntensity);
                }
            } else {
                if(_vignetteSettings.intensity > _defaultIntensity) {
                    _vignetteSettings.intensity.value = Mathf.Clamp(_vignetteSettings.intensity.value - (_intensityRate * dt), _defaultIntensity, _maxIntensity);
                }
            }
        }
#endregion
    }
}
