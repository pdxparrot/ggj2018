using UnityEngine;

namespace ggj2018.ggj2018
{
    public class Prey : Bird
    {
        [SerializeField]
        private GameObject _bloodObject;

#region Unity Lifecycle
        protected override void Start()
        {
            base.Start();

            ShowBlood(false);
        }
#endregion

        public void ShowBlood(bool show)
        {
            _bloodObject.SetActive(show);
        }
    }
}
