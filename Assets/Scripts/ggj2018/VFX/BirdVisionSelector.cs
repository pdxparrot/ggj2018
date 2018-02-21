using pdxpartyparrot.ggj2018.Data;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.VFX
{
    public class BirdVisionSelector : MonoBehaviour
    {
        [SerializeField]
        private HawkVisionMaterialPropertyChanger _predatorVision;

        public void SetVision(BirdTypeData birdType)
        {
            _predatorVision.Enabled = birdType.IsPredator;
        }
    }
}
