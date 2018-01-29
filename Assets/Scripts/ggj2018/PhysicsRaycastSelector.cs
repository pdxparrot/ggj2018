using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.EventSystems;

namespace ggj2018.ggj2018
{
    public class PhysicsRaycastSelector : MonoBehavior
    {
        [SerializeField]
        private PhysicsRaycaster _physicsRaycaster;

        [SerializeField]
        private GvrPointerPhysicsRaycaster _gvrPhysicsRaycaster;

#region Unity Lifecycle
        private void Awake()
        {
            _physicsRaycaster.enabled = !GameManager.Instance.ConfigData.EnableVR;
            _gvrPhysicsRaycaster.enabled = GameManager.Instance.ConfigData.EnableGVR;
        }
#endregion
    }
}
