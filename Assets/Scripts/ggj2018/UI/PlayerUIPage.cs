using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Camera;
using pdxpartyparrot.ggj2018.GameTypes;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.UI
{
    [RequireComponent(typeof(Canvas))]
    public sealed class PlayerUIPage : MonoBehavior
    {
        [SerializeField]
        private CharacterSelectCard _characterSelectCard;

        public CharacterSelectCard CharacterSelect => _characterSelectCard;

        [SerializeField]
        private GameOverCard _gameOverCard;

        [SerializeField]
        private PlayerHUDCard _playerHUDCard;

        public PlayerHUDCard PlayerHUD => _playerHUDCard;

        [SerializeField]
        private GameObject _deadPanel;

        public void Initialize(Viewer viewer)
        {
            GetComponent<Canvas>().worldCamera = viewer.UICamera;
        }

        public void Hide()
        {
            _characterSelectCard.gameObject.SetActive(false);
            _gameOverCard.gameObject.SetActive(false);
            _playerHUDCard.gameObject.SetActive(false);

            _deadPanel.SetActive(false);
        }

        public void SwitchToCharacterSelect(CharacterSelectState selectState)
        {
            Hide();

            _characterSelectCard.gameObject.SetActive(true);

            // TODO: tying this to the controller index is a biiiig assumption :\
            _characterSelectCard.Initialize(PlayerManager.Instance.PlayerData.GetPlayerColor(selectState.ControllerIndex));
        }

        public void SwitchToGame(Player player, GameType gameType)
        {
            Hide();

            _playerHUDCard.gameObject.SetActive(true);
            _playerHUDCard.Initialize(player, gameType);
        }

        public void SwitchToDead()
        {
            Hide();

            _playerHUDCard.gameObject.SetActive(true);

            _deadPanel.SetActive(true);
        }

        public void SwitchToGameOver(Player player, GameType gameType)
        {
            Hide();

            _playerHUDCard.gameObject.SetActive(true);
            _gameOverCard.gameObject.SetActive(true);

            switch(player.State.GameOverState)
            {
            case PlayerState.GameOverType.Win:
                _gameOverCard.SetText(gameType.GameTypeData.GetWinText(player.Bird.Type));
                break;
            case PlayerState.GameOverType.Loss:
                _gameOverCard.SetText(gameType.GameTypeData.GetLossText(player.Bird.Type));
                break;
            case PlayerState.GameOverType.TimerUp:
                _gameOverCard.SetText(gameType.GameTypeData.TimesUpText);
                break;
            }
        }
    }
}

