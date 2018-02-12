using UnityEngine;

namespace ggj2018.Core.Camera
{
    public interface IFollowTarget
    {
        Vector3 LookAxis { get; }

        bool IsPaused { get; }
    }
}
