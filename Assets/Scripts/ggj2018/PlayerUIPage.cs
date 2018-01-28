using ggj2018.Core.Util;
using ggj2018.Core.Input;
using ggj2018.ggj2018.Data;

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

        void Hide() {
            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(false);
        }

        public void SetStatus(bool joined,
                              bool ready,
                              int bird) {
            Hide();

            if(ready)
                ReadyPrompt.gameObject.SetActive(true);
            else if(joined) {
                BirdLabel.text = "birdybird";
                BirdLabel.gameObject.SetActive(true);
            }
            else {
                JoinPrompt.gameObject.SetActive(true);
            }

        }
    }
}

