using ggj2018.Core.Camera;

using UnityEngine;

namespace ggj2018.ggj2018
{
// TODO: remove this when NetworkPlayer is gone
    public interface IPlayer : IFollowTarget
    {
        GameObject GameObject { get; }

        PlayerState State { get; }

        PlayerController Controller { get; }

        void Initialize();
    }
}
