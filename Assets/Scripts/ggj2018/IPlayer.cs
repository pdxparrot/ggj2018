using UnityEngine;

namespace ggj2018.ggj2018
{
    public interface IPlayer
    {
        GameObject GameObject { get; }

        PlayerState State { get; }

        PlayerController Controller { get; }
    }
}
