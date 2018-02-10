using System;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="BirdPhysicsData", menuName="ggj2018/Data/Bird Physics Data")]
    [Serializable]
    public sealed class BirdPhysicsData : ScriptableObject
    {
        [SerializeField]
        private float _mass = 1.0f;

        public float Mass => _mass;

        [SerializeField]
        private float _drag = 1.0f;

        public float Drag => _drag;

        [SerializeField]
        private float _angularDrag = 1.0f;

        public float AngularDrag => _angularDrag;

        [SerializeField]
        private float _terminalVelocity = 10.0f;

        public float TerminalVelocity => _terminalVelocity;

        [SerializeField]
        private float _linearThrust = 10.0f;

        public float LinearThrust => _linearThrust;

        [SerializeField]
        private float _angularThrust = 10.0f;

        public float AngularThrust => _angularThrust;

        [SerializeField]
        private float _turnSpeed = 10.0f;

        public float TurnSpeed => _turnSpeed;

        [SerializeField]
        private float _brakeThrust = 5.0f;

        public float BrakeThrust => _brakeThrust;

        [SerializeField]
        private float _boostThrust = 5.0f;

        public float BoostThrust => _boostThrust;
    }
}
