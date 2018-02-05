using ggj2018.Core.Util;
using ggj2018.Core.VFX;
using ggj2018.ggj2018.Birds;

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

        private Bird _bird;

        private PostProcessVolume _postProcessVolume;

        private Vignette _vignetteSettings;

#region Unity Lifecycle
        private void Awake()
        {
            _bird = GetComponent<Bird>();

            _postProcessVolume = GetComponent<PostProcessVolumeSelector>().PostProcessVolume;
            if(_postProcessVolume.profile.TryGetSettings(out _vignetteSettings)) {
                _defaultIntensity = _vignetteSettings.intensity;
            }
        }

        private void Update()
        {
            if(null == _vignetteSettings) {
                return;
            }

            if(null == _bird.Owner.NearestPredator) {
                _vignetteSettings.intensity.value = _defaultIntensity;
                return;
            }

            float distance = (_bird.Owner.transform.position - _bird.Owner.NearestPredator.transform.position).magnitude;
            if(distance > GameManager.Instance.GameTypeData.HawkAlertDistance) {
                _vignetteSettings.intensity.value = _defaultIntensity;
                return;
            }

            float step = (1.0f - _defaultIntensity) / GameManager.Instance.GameTypeData.HawkAlertDistance;
            float pct = 1.0f - (distance / GameManager.Instance.GameTypeData.HawkAlertDistance);

            float intensity = _defaultIntensity + (pct * step);
            _vignetteSettings.intensity.value = intensity;
        }
#endregion
    }
}
