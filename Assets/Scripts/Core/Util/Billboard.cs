using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

namespace pdxpartyparrot.Core.UI
{
    public class Billboard : MonoBehavior
    {
        [CanBeNull]
        public UnityEngine.Camera Camera { get; set; }

#region Unity Lifecycle
        private void Update()
        {
            if(null != Camera) {
                transform.forward = (Camera.transform.position - transform.position).normalized;
            }
        }
#endregion
    }
}
