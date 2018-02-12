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
        private GameObject _waiting;

        [SerializeField]
        private GameObject _allReady;

        [SerializeField]
        private Text _birdLabel;

        [SerializeField]
        private Image _birdImage1;

        [SerializeField]
        private Image _birdImage2;

        [SerializeField]
        private Image _characterFrame;

        [SerializeField]
        private Image _readyCharacterFrame;

        public void Initialize(Color color)
        {
            _joinPanel.SetActive(true);
            _characterSelectPanel.SetActive(false);
            _readyPanel.SetActive(false);

            color.a = 1.0f;
            _characterFrame.color = color;
            _readyCharacterFrame.color = color;
        }

        public void Update(CharacterSelectState selectState, bool allReady)
        {
            _joinPanel.SetActive(!selectState.IsJoinedOrReady);
            _characterSelectPanel.SetActive(selectState.IsJoined);
            _readyPanel.SetActive(selectState.IsReady);

            if(selectState.IsReady) {
                _allReady.SetActive(allReady);
                _waiting.SetActive(!allReady);
            }

            _birdLabel.text = selectState.PlayerBirdData.Name;
            _birdImage1.sprite = selectState.PlayerBirdData.Icon;
            _birdImage2.sprite = selectState.PlayerBirdData.Icon;
        }
    }
}
