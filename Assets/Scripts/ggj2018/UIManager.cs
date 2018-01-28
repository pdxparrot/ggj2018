using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.ggj2018.Data;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        public PlayerUIPage[] PlayerHud;

        [SerializeField] private Text _countdown;
    }
}

