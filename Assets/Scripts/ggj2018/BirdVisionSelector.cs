using UnityEngine;

namespace ggj2018.ggj2018
{
    public class BirdVisionSelector : MonoBehaviour
    {
        [SerializeField]
        private HawkVisionMaterialPropertyChanger _predatorVision;

        public void SetVision(BirdType birdType)
        {
            _predatorVision.Enabled = birdType.BirdDataEntry.IsPredator;
        }
    }
}
