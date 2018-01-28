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
        [SerializeField] private Text JoinPrompt;
        [SerializeField] private Text BirdLabel;
        [SerializeField] private Text BirdValue;
        [SerializeField] private Text ReadyPrompt;

        List<string> birdNames = new List<string>();

        void Start() {
            birdNames.Add("Hawk");
            birdNames.Add("Pigeon");
            birdNames.Add("Sparrow");
            birdNames.Add("Owl");
        }

        public void Hide() {
            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(false);
        }

        public void SetStatus(bool joined,
                              bool ready,
                              int bird) {
            Hide();

            if(ready) {
                ReadyPrompt.text = "Ready!";
                ReadyPrompt.gameObject.SetActive(true);
            }
            else if(joined) {
                BirdLabel.text = birdNames[bird];
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
    }
}

