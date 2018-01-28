using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.Core.UI
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        private GameObject _uiContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");
        }

        protected override void OnDestroy()
        {
            Destroy(_uiContainer);
            _uiContainer = null;
        }
#endregion
    }
}
