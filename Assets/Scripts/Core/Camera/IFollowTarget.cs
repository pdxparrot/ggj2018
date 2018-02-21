using UnityEngine;

namespace pdxpartyparrot.Core.Camera
{
    public interface IFollowTarget
    {
        Vector3 LookAxis { get; }

        bool IsPaused { get; }
    }
}
