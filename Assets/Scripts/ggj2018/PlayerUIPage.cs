using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Canvas))]
    public sealed class PlayerUIPage : SingletonBehavior<PlayerUIPage>
    {
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private GameObject HudPanel;
        [SerializeField] private GameObject FinishPanel;

        [SerializeField] private GameObject JoinPanel;
        [SerializeField] private GameObject CharSelPanel;
        [SerializeField] private GameObject ReadyPanel;

        [SerializeField] private GameObject AllReady;
        [SerializeField] private GameObject Waiting;

        [SerializeField] private GameObject FinishWin;
        [SerializeField] private GameObject FinishLose;

        [SerializeField] private GameObject Hawk1;
        [SerializeField] private GameObject Hawk2;
        [SerializeField] private GameObject Carrier1;
        [SerializeField] private GameObject Carrier2;

        [SerializeField] private GameObject KillCard;
        [SerializeField] private GameObject Kill1;
        [SerializeField] private GameObject Kill2;
        [SerializeField] private GameObject Kill3;

        [SerializeField] private GameObject PauseMenu;

        [SerializeField] private GameObject GoalCard;
        [SerializeField] private Text GoalDist;

        [SerializeField] private Text Speed;
        [SerializeField] private Image Boost;

        //[SerializeField] private Text JoinPrompt;
        //[SerializeField] private Text BirdLabel;
        //[SerializeField] private Text BirdValue;
        //[SerializeField] private Text ReadyPrompt;

        public void Hide() {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);

            //JoinPrompt.gameObject.SetActive(false);
            //BirdLabel.gameObject.SetActive(false);
            //BirdValue.gameObject.SetActive(false);
            //ReadyPrompt.gameObject.SetActive(false);
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

            FinishWin.SetActive(won);
            FinishLose.SetActive(!won);
        }

        public void SetStatus(bool joined,
                              bool ready,
                              string bird,
                              bool allready) {
            SwitchToMenu();

            JoinPanel.SetActive(!joined && !ready);
            CharSelPanel.SetActive(joined && !ready);
            ReadyPanel.SetActive(ready);

            Hawk1.SetActive(bird == "hawk");
            Hawk2.SetActive(bird == "hawk");
            Carrier1.SetActive(bird != "hawk");
            Carrier2.SetActive(bird != "hawk");

            if(ready) {
                AllReady.SetActive(allready);
                Waiting.SetActive(!allready);
            }
        }

        public void SetSpeedAndBoost(int speed, float boost) {
            Speed.text = $"{speed}";
            Boost.fillAmount = boost;

            KillCard.SetActive(false);
            GoalCard.SetActive(false);
        }

        public void EnablePauseMenu(bool enable) {
            PauseMenu.SetActive(enable);
        }

        /*
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

