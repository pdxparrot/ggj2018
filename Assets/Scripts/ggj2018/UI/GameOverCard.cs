using ggj2018.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace ggj2018.ggj2018.UI
{
    public class GameOverCard : MonoBehavior
    {
        [SerializeField]
        private Text _winLossText;

        public void SetText(string text)
        {
            _winLossText.text = text;
        }
    }
}
