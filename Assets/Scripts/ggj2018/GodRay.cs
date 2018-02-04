using System.Linq;

using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GodRay : MonoBehavior
    {
        public enum Mode { Hawk, Carrier, Goal }; 
        [SerializeField] private Mode mode = Mode.Goal;

        [SerializeField] private float FadeNearDist;
        [SerializeField] private float FadeFarDist;

        [SerializeField] private MeshRenderer mesh;

#region Unity Lifecycle
        private void Update()
        {
            var dist = FadeFarDist + 1;

            if(mode == Mode.Carrier) {
                foreach(Player player in PlayerManager.Instance.Players) {
                    if(player.Bird.Type.IsPredator) {
                        var vec = player.gameObject.transform.position - transform.position;
                        vec.y = 0;
                        dist = vec.magnitude;
                        break;
                    }
                }
            } else if(mode == Mode.Goal) {
                foreach(Player player in PlayerManager.Instance.Players) {
                    if(player.Bird.Type.IsPredator) {
                        var vec = player.gameObject.transform.position - transform.position;
                        vec.y = 0;
                        var d = vec.magnitude;
                        dist = Mathf.Min(dist, d);
                    }
                }
            }

            float alpha = Mathf.Clamp01((dist - FadeNearDist) / (FadeFarDist - FadeNearDist));

            Color c = mesh.material.color;
            c.a = alpha;
            mesh.material.color = c;
        }
#endregion

        public void Setup(Player player)
        {
            mode = player.Bird.Type.IsPredator ? Mode.Hawk : Mode.Carrier;

            gameObject.SetActive(player.Bird.Type.IsPrey);
            SetLayer(gameObject, player.Bird.Type.OtherLayer);
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
