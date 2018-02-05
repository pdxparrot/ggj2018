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
        private Bird _bird;

        private PostProcessVolume _postProcessVolume;

        private Vignette _vignetteSettings;

#region Unity Lifecycle
        private void Awake()
        {
            _bird = GetComponent<Bird>();

            _postProcessVolume = GetComponent<PostProcessVolumeSelector>().PostProcessVolume;
        }

        private void Update()
        {
            if(null == _bird.Owner.NearestPredator) {
                return;
            }

            float distance = (_bird.Owner.transform.position - _bird.Owner.NearestPredator.transform.position).magnitude;
            if(distance > GameManager.Instance.GameTypeData.HawkAlertDistance) {
                return;
            }

            float intensity = distance / GameManager.Instance.GameTypeData.HawkAlertDistance;
            _vignetteSettings.intensity.value = intensity;
        }
#endregion
    }
}
