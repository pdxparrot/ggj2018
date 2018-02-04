using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace ggj2018.Core.VFX
{
    public class PostProcessVolumeSelector : MonoBehavior
    {
        [SerializeField]
        [TagSelector]
        private string _postProcessVolumeTag;

        public PostProcessVolume PostProcessVolume { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            PostProcessVolume = CameraManager.Instance.PostProcessVolumes.GetOrDefault(_postProcessVolumeTag);
            if(null == PostProcessVolume) {
                Debug.LogError($"No such PostProcessVolume with tag {_postProcessVolumeTag}!");
            }
        }
#endregion
    }
}
