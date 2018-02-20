using System.Collections;

using ggj2018.Core.Util;
using ggj2018.ggj2018.Game;
using ggj2018.ggj2018.GameState;
using ggj2018.ggj2018.GameTypes;
using ggj2018.ggj2018.Players;
using ggj2018.Game.State;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    public class PlayerHUDCard : MonoBehavior
    {
#region Intro
        [SerializeField]
        private GameObject _introPanel;

        [SerializeField]
        private Text _goalText;

        [SerializeField]
        private float _introPanelTime = 3.0f;
#endregion

#region Timer
        [SerializeField]
        private GameObject _timer;

        [SerializeField]
        private Text _timerText;
#endregion

#region Speed
        [SerializeField]
        private GameObject _speed;

        [SerializeField]
        private Text _speedText;
#endregion

#region Boost
        [SerializeField]
        private Image _boostMeter;
#endregion

#region Goal Distance
        [SerializeField]
        private GameObject _goalDistance;

        [SerializeField]
        private Text _goalDistanceText;
#endregion

#region Kills
        [SerializeField]
        private GameObject _killPanel;

        [SerializeField]
        private GameObject _killCardPrefab;

        [SerializeField]
        private LayoutGroup _killCardLayout;
#endregion

#region Debug
        [SerializeField]
        private DebugVisualizer _debugVisualizer;

        public DebugVisualizer DebugVisualizer => _debugVisualizer;
#endregion

        public void Initialize(Player player, GameType gameType)
        {
            _timer.SetActive(gameType.ShowTimer);
            _goalDistance.SetActive(!gameType.PredatorsKillPrey || player.Bird.Type.IsPrey);

            _killPanel.SetActive(gameType.PredatorsKillPrey && player.Bird.Type.IsPredator);
            RemoveKills();

            _introPanel.SetActive(true);
            _goalText.text = gameType.GameTypeData.GetWinConditionDescription(player.Bird.Type);

            EnableDebugVisualizer(false);

            StartCoroutine(CloseIntroPanel());
        }

        private IEnumerator CloseIntroPanel()
        {
            yield return new WaitForSeconds(_introPanelTime);

            _introPanel.SetActive(false);
        }

        public void SetState(Player player)
        {
            SetTimer((GameStateManager.Instance.CurrentState as GameStateGame)?.GameTimer ?? 0.0f);

            //_speedText.text = $"{(int)UnitUtils.MetersPerSecondToKilometersPerHour(player.Controller.Speed)} km/h";
            _speedText.text = $"{(int)player.Controller.Speed} m/s";
            _boostMeter.fillAmount = player.State.BoostRemainingPercent;

            /*if(distance > 1000.0f) {
                _goalDistanceText.text = $"{(player.NearestGoalDistance / 1000.0f):0.00} km";
            } else {*/
                _goalDistanceText.text = $"{(int)player.NearestGoalDistance} m";
            //}
        }

        private void SetTimer(float timerSeconds)
        {
            float minutes = Mathf.Floor(timerSeconds / 60.0f);
            float seconds = Mathf.Floor(timerSeconds % 60.0f);

            _timerText.text = $"{minutes:0}:{seconds:00}";
        }

        public void AddKill()
        {
            Instantiate(_killCardPrefab, _killCardLayout.transform);
        }

        private void RemoveKills()
        {
            _killCardLayout.transform.Clear();
        }

        public void EnableDebugVisualizer(bool enable)
        {
            Debug.Log($"Enabling debug visualizer: {enable}");
            _debugVisualizer.gameObject.SetActive(enable);
        }
    }
}
