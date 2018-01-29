using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.ggj2018.Data;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018
{
    public sealed class PlayerUIPage : SingletonBehavior<PlayerUIPage>
    {
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private GameObject HudPanel;
        [SerializeField] private GameObject FinishPanel;

        [SerializeField] private GameObject JoinPanel;
        [SerializeField] private GameObject CharSelPanel;
        [SerializeField] private GameObject ReadyPanel;

        [SerializeField] private Text JoinPrompt;
        [SerializeField] private Text BirdLabel;
        [SerializeField] private Text BirdValue;
        [SerializeField] private Text ReadyPrompt;

        public void Hide() {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);

            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(false);
        }

        public void SwitchToMenu() {
            MenuPanel.SetActive(true);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);
        }
        public void SwitchToGame() {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(true);
            FinishPanel.SetActive(false);
        }
        public void SwitchToVictory(bool won) {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(true);
        }

        /*
        public void SetStatus(bool joined,
                              bool ready,
                              string bird) {
            Hide();

            if(ready) {
                ReadyPrompt.text = "Ready!";
                ReadyPrompt.gameObject.SetActive(true);
            }
            else if(joined) {
                BirdLabel.text = bird;
                BirdLabel.gameObject.SetActive(true);
            }
            else {
                JoinPrompt.gameObject.SetActive(true);
            }
        }

        public void HideCountdown() {
            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(false);
        }

        public void SetCountdown(int i) {
            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(true);

            if(i > 0)
                ReadyPrompt.text = $"{i}";
            else
                ReadyPrompt.text = "Go!";
        }
        */
    }
}

