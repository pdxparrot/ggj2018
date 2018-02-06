using ggj2018.Core.Util;
using ggj2018.ggj2018.Birds;
using ggj2018.ggj2018.Game;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.ggj2018.VFX
{
    [RequireComponent(typeof(Prey))]
    public class PredatorAlert : MonoBehavior
    {
        [SerializeField]
        [ReadOnly]
        private float _defaultIntensity;

        private Prey _bird;

        private Vignette _vignetteSettings;

#region Unity Lifecycle
        private void Awake()
        {
            _bird = GetComponent<Prey>();
        }

        private void Start()
        {
            if(_bird.Owner.Viewer.GlobalPostProcessVolume.profile.TryGetSettings(out _vignetteSettings)) {
                _defaultIntensity = _vignetteSettings.intensity;
            }
        }

        private void Update()
        {
            if(null == _vignetteSettings) {
                return;
            }

            /*if(_bird.Owner.State.IsDead) {
                _vignetteSettings.intensity.value = 1.0f;
                return;
            }

            if(null == _bird.Owner.NearestPredator) {
                _vignetteSettings.intensity.value = _defaultIntensity;
                return;
            }

            float distance = (_bird.Owner.transform.position - _bird.Owner.NearestPredator.transform.position).magnitude;
            if(distance >= GameManager.Instance.GameTypeData.HawkAlertDistance) {
                _vignetteSettings.intensity.value = _defaultIntensity;
                return;
            }

            float pct = (GameManager.Instance.GameTypeData.HawkAlertDistance - distance) / GameManager.Instance.GameTypeData.HawkAlertDistance;
            float step = (1.0f - _defaultIntensity);

            float intensity = _defaultIntensity + (pct * step);
            _vignetteSettings.intensity.value = intensity;*/
        }

        private void OnDrawGizmos()
        {
            if(GameManager.HasInstance) {
                Gizmos.DrawWireSphere(transform.position, GameManager.Instance.GameTypeData.HawkAlertDistance);
            }
        }
#endregion
    }
}
