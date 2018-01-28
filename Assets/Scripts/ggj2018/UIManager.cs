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

        public void Countdown(int c) {
            for(int i = 0; i < PlayerHud.Length; ++i)
                PlayerHud[i].SetCountdown(c);
        }
        public void HideCountdown() {
            for(int i = 0; i < PlayerHud.Length; ++i)
                PlayerHud[i].HideCountdown();
        }

        public void HideMenu() {
            for(int i = 0; i < PlayerHud.Length; ++i)
                PlayerHud[i].Hide();
        }
    }
}

