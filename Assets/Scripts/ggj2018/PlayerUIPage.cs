using System.Collections;

using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018
{
    [RequireComponent(typeof(Canvas))]
    public sealed class PlayerUIPage : MonoBehavior
    {
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private GameObject HudPanel;
        [SerializeField] private GameObject FinishPanel;
        [SerializeField] private GameObject IntroPanel;

        [SerializeField] private GameObject JoinPanel;
        [SerializeField] private GameObject CharSelPanel;
        [SerializeField] private GameObject ReadyPanel;

        [SerializeField] private GameObject AllReady;
        [SerializeField] private GameObject Waiting;

        [SerializeField] private GameObject FinishWin;
        [SerializeField] private GameObject FinishLose;

        [SerializeField] private GameObject KillCard;
        [SerializeField] private GameObject Kill1;
        [SerializeField] private GameObject Kill2;
        [SerializeField] private GameObject Kill3;

        [SerializeField] private GameObject GoalCard;
        [SerializeField] private Text GoalDist;

        [SerializeField] private Text Speed;
        [SerializeField] private Image Boost;
        [SerializeField] private Text Goal;

        //[SerializeField] private Text JoinPrompt;
        [SerializeField] private Text BirdLabel;
        [SerializeField] private Image BirdImage1;
        [SerializeField] private Image BirdImage2;
        //[SerializeField] private Text BirdValue;
        //[SerializeField] private Text ReadyPrompt;

        [SerializeField] private float _introPanelTime = 3.0f;

        private Player _owner;

        public void Initialize(Player owner)
        {
            _owner = owner;
        }

        public void Hide()
        {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);
            IntroPanel.SetActive(false);

            //JoinPrompt.gameObject.SetActive(false);
            //BirdLabel.gameObject.SetActive(false);
            //BirdValue.gameObject.SetActive(false);
            //ReadyPrompt.gameObject.SetActive(false);
        }

        public void SwitchToMenu()
        {
            MenuPanel.SetActive(true);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);
            IntroPanel.SetActive(false);
        }

        public void SwitchToGame()
        {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(true);
            FinishPanel.SetActive(false);
            IntroPanel.SetActive(true);

            Goal.text = GameManager.Instance.State.GameType.GameTypeData.GetWinConditionDescription(_owner.State.BirdType);
            StartCoroutine(CloseIntroPanel());
        }

        private IEnumerator CloseIntroPanel()
        {
            yield return new WaitForSeconds(_introPanelTime);

            IntroPanel.SetActive(false);
        }

        public void SwitchToVictory(bool won)
        {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(true);
            IntroPanel.SetActive(false);

            FinishWin.SetActive(won);
            FinishLose.SetActive(!won);
        }

        public void SetStatus(PlayerManager.CharacterSelectState characterSelectState, bool allready)
        {
            SwitchToMenu();

            JoinPanel.SetActive(!characterSelectState.IsJoinedOrReady);
            CharSelPanel.SetActive(characterSelectState.IsJoined);
            ReadyPanel.SetActive(characterSelectState.IsReady);

            BirdLabel.text = characterSelectState.PlayerBirdData.Name;
            BirdImage1.sprite = characterSelectState.PlayerBirdData.Icon;
            BirdImage2.sprite = characterSelectState.PlayerBirdData.Icon;

            if(characterSelectState.IsReady) {
                AllReady.SetActive(allready);
                Waiting.SetActive(!allready);
            }
        }

        public void SetSpeedAndBoost(int speed, float boost)
        {
            Speed.text = $"{speed}";
            Boost.fillAmount = boost;

            KillCard.SetActive(false);
            GoalCard.SetActive(false);
        }

        /*
        public void HideCountdown()
        {
            JoinPrompt.gameObject.SetActive(false);
            BirdLabel.gameObject.SetActive(false);
            BirdValue.gameObject.SetActive(false);
            ReadyPrompt.gameObject.SetActive(false);
        }

        public void SetCountdown(int i)
        {
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

