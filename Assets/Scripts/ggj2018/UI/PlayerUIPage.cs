using System.Collections;

using ggj2018.Core.Util;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
// TODO: this needs a serious cleanup pass
// and it needs to be split into multiple cards/prefabs

    [RequireComponent(typeof(Canvas))]
    public sealed class PlayerUIPage : MonoBehavior
    {
        [SerializeField]
        private CharacterSelectCard _characterSelectCard;

        [SerializeField]
        private GameOverCard _gameOverCard;

        [SerializeField] private GameObject HudPanel;
        [SerializeField] private GameObject IntroPanel;

        [SerializeField] private GameObject DeadPanel;

        [SerializeField] private GameObject KillPanel;
        [SerializeField] private GameObject KillCardPrefab;
        [SerializeField] private LayoutGroup KillCardLayout;

        [SerializeField] private GameObject GoalDistancePanel;
        [SerializeField] private Text GoalDistanceText;

        [SerializeField] private Text Speed;
        [SerializeField] private Image Boost;
        [SerializeField] private Text Goal;

        [SerializeField] private GameObject GameTimerPanel;
        [SerializeField] private Text GameTimer;

        [SerializeField] private float _introPanelTime = 3.0f;

        [Space(10)]

#region Debug
        [Header("Debug")]

        [SerializeField] private GameObject DebugVisualizer;
        [SerializeField] private GameObject DebugVelocityPanel;
        [SerializeField] private Text DebugVelocityText;
        [SerializeField] private Text DebugAngularVelocityText;
        [SerializeField] private Text DebugBankForceText;
#endregion

        private Player _ownerPlayer;

        private CharacterSelectState _ownerSelectState;

        public void Initialize(Player owner)
        {
            _ownerPlayer = owner;
        }

        public void Initialize(CharacterSelectState owner)
        {
            _ownerSelectState = owner;
        }

        public void Hide()
        {
            _characterSelectCard.gameObject.SetActive(false);
            _gameOverCard.gameObject.SetActive(false);

            HudPanel.SetActive(false);
            IntroPanel.SetActive(false);
            DeadPanel.SetActive(false);
            KillPanel.SetActive(false);
            GoalDistancePanel.SetActive(false);
        }

        public void SwitchToCharacterSelect()
        {
            Hide();

            _characterSelectCard.gameObject.SetActive(true);

            // TODO: tying this to the controller index is a biiiig assumption :\
            _characterSelectCard.Initialize(PlayerManager.Instance.PlayerData.GetPlayerColor(_ownerSelectState.ControllerIndex));
        }

        public void SwitchToGame(GameType gameType)
        {
            Hide();

            HudPanel.SetActive(true);

            KillPanel.SetActive(gameType.PredatorsKillPrey && _ownerPlayer.Bird.Type.IsPredator);
            GoalDistancePanel.SetActive(!gameType.PredatorsKillPrey || _ownerPlayer.Bird.Type.IsPrey);

            Goal.text = gameType.GameTypeData.GetWinConditionDescription(_ownerPlayer.Bird.Type);
            GameTimerPanel.SetActive(gameType.ShowTimer);

            StartCoroutine(CloseIntroPanel());
        }

        private IEnumerator CloseIntroPanel()
        {
            yield return new WaitForSeconds(_introPanelTime);

            IntroPanel.SetActive(false);
        }

        public void SwitchToDead()
        {
            Hide();

            HudPanel.SetActive(true);
            DeadPanel.SetActive(true);
        }

        public void SwitchToGameOver(GameType gameType)
        {
            Hide();

            HudPanel.SetActive(true);
            KillPanel.SetActive(gameType.PredatorsKillPrey && _ownerPlayer.Bird.Type.IsPredator);

            _gameOverCard.gameObject.SetActive(true);

            switch(_ownerPlayer.State.GameOverState)
            {
            case PlayerState.GameOverType.Win:
                _gameOverCard.SetText(gameType.GameTypeData.GetWinText(_ownerPlayer.Bird.Type));
                break;
            case PlayerState.GameOverType.Loss:
                _gameOverCard.SetText(gameType.GameTypeData.GetLossText(_ownerPlayer.Bird.Type));
                break;
            case PlayerState.GameOverType.TimerUp:
                _gameOverCard.SetText(gameType.GameTypeData.TimesUpText);
                break;
            }
        }

        public void SetStatus(CharacterSelectState characterSelectState, bool allReady)
        {
            Hide();

            _ownerSelectState = characterSelectState;
            SwitchToCharacterSelect();

            _characterSelectCard.Update(characterSelectState, allReady);
        }

        public void SetSpeedAndBoost(float speed, float boost)
        {
            //Speed.text = $"{(int)UnitUtils.MetersPerSecondToKilometersPerHour(speed)} km/h";
            Speed.text = $"{(int)speed} m/s";
            Boost.fillAmount = boost;
        }

        public void SetTimer(float timerSeconds)
        {
            float minutes = Mathf.Floor(timerSeconds / 60.0f);
            float seconds = Mathf.Floor(timerSeconds % 60.0f);

            GameTimer.text = $"{minutes:0}:{seconds:00}";
        }

        public void SetDistanceToGoal(float distance)
        {
            /*if(distance > 1000.0f) {
                GoalDistanceText.text = $"{(distance / 1000.0f):0.00} km";
            } else {*/
                GoalDistanceText.text = $"{(int)distance} m";
            //}
        }

        public void AddKill()
        {
            Instantiate(KillCardPrefab, KillCardLayout.transform);
        }

#region Debug
        public void EnableDebugVisualizer(bool enable)
        {
            Debug.Log($"Enabling debug visualizer: {enable}");
            DebugVisualizer.SetActive(enable);
        }

        public void UpdateDebugVisualizer(Rigidbody rb, Vector3 bankForce)
        {
            DebugVelocityText.text = $"Velocity: {rb.velocity} m/s";
            DebugAngularVelocityText.text = $"Angular Velocity: {rb.angularVelocity} m/s";
            DebugBankForceText.text = $"Bank Force: {bankForce}N";

            DebugVisualizer.transform.position = _ownerPlayer.transform.position + (5.0f * _ownerPlayer.transform.forward);
        }
#endregion
    }
}

