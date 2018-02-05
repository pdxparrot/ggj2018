﻿using System;
using System.Collections;

using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;
using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class PlayerUIPage : MonoBehavior
    {
        [SerializeField] private GameObject MenuPanel;
        [SerializeField] private GameObject HudPanel;
        [SerializeField] private GameObject FinishPanel;
        [SerializeField] private GameObject IntroPanel;

        [SerializeField] private Image CharacterFrame;

        [SerializeField] private GameObject JoinPanel;
        [SerializeField] private GameObject CharSelPanel;
        [SerializeField] private GameObject ReadyPanel;

        [SerializeField] private GameObject AllReady;
        [SerializeField] private GameObject Waiting;

        [SerializeField] private GameObject DeadPanel;

        [SerializeField] private GameObject WinLossPanel;
        [SerializeField] private Text WinLossText;

        [SerializeField] private GameObject KillCard;
        [SerializeField] private GameObject Kill1;
        [SerializeField] private GameObject Kill2;
        [SerializeField] private GameObject Kill3;

        [SerializeField] private GameObject GoalCard;
        [SerializeField] private Text GoalDist;

        [SerializeField] private Text Speed;
        [SerializeField] private Image Boost;
        [SerializeField] private Text Goal;

        [SerializeField] private Text BirdLabel;
        [SerializeField] private Image BirdImage1;
        [SerializeField] private Image BirdImage2;

        [SerializeField] private GameObject GameTimerPanel;
        [SerializeField] private Text GameTimer;

        [SerializeField] private GameObject ScorePanel;
        [SerializeField] private Text Score;

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
            DeadPanel.SetActive(false);
        }

        public void SwitchToCharacterSelect()
        {
            MenuPanel.SetActive(true);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(false);
            IntroPanel.SetActive(false);
            DeadPanel.SetActive(false);

            CharacterFrame.color = _owner?.PlayerColor ?? PlayerManager.Instance.PlayerData.DefaultPlayerColor;
        }

        public void SwitchToGame(GameType gameType)
        {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(true);
            FinishPanel.SetActive(false);
            IntroPanel.SetActive(true);
            DeadPanel.SetActive(false);

            Goal.text = gameType.GameTypeData.GetWinConditionDescription(_owner.Bird.Type);
            GameTimerPanel.SetActive(gameType.ShowTimer);

            ScorePanel.SetActive(gameType.ShowScore(_owner.Bird.Type));

            StartCoroutine(CloseIntroPanel());
        }

        private IEnumerator CloseIntroPanel()
        {
            yield return new WaitForSeconds(_introPanelTime);

            IntroPanel.SetActive(false);
        }

        public void SwitchToDead()
        {
            DeadPanel.SetActive(true);
        }

        public void SwitchToGameOver(GameType gameType)
        {
            MenuPanel.SetActive(false);
            HudPanel.SetActive(false);
            FinishPanel.SetActive(true);
            IntroPanel.SetActive(false);

            WinLossPanel.SetActive(true);
            switch(_owner.State.GameOverState)
            {
            case PlayerState.GameOverType.Win:
                WinLossText.text = gameType.GameTypeData.GetWinText(_owner.Bird.Type);
                break;
            case PlayerState.GameOverType.Loss:
                WinLossText.text = gameType.GameTypeData.GetLossText(_owner.Bird.Type);
                break;
            case PlayerState.GameOverType.TimerUp:
                WinLossText.text = gameType.GameTypeData.TimesUpText;
                break;
            }
        }

        public void SetStatus(CharacterSelectState characterSelectState, bool allready)
        {
            SwitchToCharacterSelect();

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

        public void SetTimer(TimeSpan timeSpan)
        {
            GameTimer.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public void SetScore(int score, int maxScore)
        {
            Score.text = $"{score} / {maxScore}";
        }
    }
}
