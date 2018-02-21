using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2018.Game;
using pdxpartyparrot.ggj2018.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.VFX
{
    public sealed class GodRay : MonoBehavior
    {
        [SerializeField]
        private Renderer _renderer;

        public void SetupGoal()
        {
            _renderer.material.SetFloat(GameManager.Instance.ConfigData.GodRayAlphaProperty, 0.0f);
        }

        public void SetupPlayer(Player player)
        {
            // TODO: this should come from the game type
            gameObject.SetActive(player.Bird.Type.IsPrey);

            SetLayer(gameObject, player.Bird.Type.OtherLayer);

            _renderer.material.SetColor(PlayerManager.Instance.PlayerData.PlayerColorProperty, player.PlayerColor);
            _renderer.material.SetFloat(GameManager.Instance.ConfigData.GodRayAlphaProperty, 1.0f);
        }

        private void SetLayer(GameObject obj, string layer)
        {
            obj.layer = LayerMask.NameToLayer(layer);
            foreach(Transform c in obj.transform) {
                SetLayer(c.gameObject, layer);
            }
        }
    }
}
