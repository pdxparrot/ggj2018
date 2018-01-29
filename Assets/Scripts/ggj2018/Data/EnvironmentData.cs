using System;

using UnityEngine;

namespace ggj2018.ggj2018.Data
{
    [CreateAssetMenu(fileName="EnvironmentData", menuName="ggj2018/Data/Environment Data")]
    [Serializable]
    public sealed class EnvironmentData : ScriptableObject
    {
        [SerializeField]
        private float _boundaryCollisionPushback = 2.0f;

        public float BoundaryCollisionPushback => _boundaryCollisionPushback;
    }
}
