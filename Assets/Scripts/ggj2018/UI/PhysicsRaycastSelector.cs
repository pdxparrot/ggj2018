using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;

using UnityEngine;
using UnityEngine.EventSystems;

namespace pdxpartyparrot.ggj2018.UI
{
// TODO: this is core... but we need the game manager... ugh
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
