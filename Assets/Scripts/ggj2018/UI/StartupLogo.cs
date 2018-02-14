using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018.UI
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
