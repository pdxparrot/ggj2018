using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    public class CharacterSelectCard : MonoBehavior
    {
        [SerializeField]
        private GameObject _joinPanel;

        [SerializeField]
        private GameObject _characterSelectPanel;

        [SerializeField]
        private GameObject _readyPanel;

        [SerializeField]
        private Image _characterFrame;

        [SerializeField]
        private Image _readyCharacterFrame;

        public void SetPlayerColor(Color color)
        {
            color.a = 1.0f;
            _characterFrame.color = color;
            _readyCharacterFrame.color = color;
        }

        public void ShowJoin()
        {
            _joinPanel.SetActive(true);
            _characterSelectPanel.SetActive(false);
            _readyPanel.SetActive(false);
        }

        public void ShowCharacterSelect()
        {
            _joinPanel.SetActive(false);
            _characterSelectPanel.SetActive(true);
            _readyPanel.SetActive(false);
        }

        public void ShowReady()
        {
            _joinPanel.SetActive(false);
            _characterSelectPanel.SetActive(false);
            _readyPanel.SetActive(true);
        }
    }
}
