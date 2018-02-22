using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public class StartupLogo : MonoBehavior
    {
#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Canvas>().sortingOrder = 9990;
        }
#endregion
    }
}
