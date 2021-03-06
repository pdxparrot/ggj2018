﻿using UnityEngine;

namespace pdxpartyparrot.ggj2018.Birds
{
    public class Prey : Bird
    {
        [SerializeField]
        private ParticleSystem _bloodObject;

#region Unity Lifecycle
        protected override void Start()
        {
            base.Start();

            ShowBlood(false);
        }
#endregion

        public void ShowBlood(bool show)
        {
            _bloodObject.gameObject.SetActive(show);
        }
    }
}
