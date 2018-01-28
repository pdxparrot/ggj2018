using ggj2018.Core.Camera;
using ggj2018.Core.Util;

using UnityEngine;

namespace ggj2018.ggj2018
{
    public sealed class GodRay : MonoBehavior
    {
        public enum Mode { Hawk, Carrier, Goal };
        [SerializeField] private Mode mode;

        [SerializeField] private float FadeNearDist;
        [SerializeField] private float FadeFarDist;

        public void Setup(Mode m) {
            mode = m;
            if(mode == Mode.Hawk)
                gameObject.SetActive(false);
            else if(mode == Mode.Carrier) {
                gameObject.SetActive(true);
                SetLayer(gameObject, 8); // hawk
            }
            else {
                gameObject.SetActive(true);
                SetLayer(gameObject, 7); // carrier
            }
        }

        void SetLayer(GameObject obj, int layer) {
            obj.layer = layer;
            foreach(Transform c in obj.transform) {
                SetLayer(c.gameObject, layer);
            }
        }

        private void Update()
        {
            var dist = FadeFarDist + 1;

            if(mode == Mode.Carrier) {
                for(int i = 0; i < 4; ++i) {
                    var p = PlayerManager.Instance.Player(i) as LocalPlayer;
                    if(p.State.BirdType.BirdDataEntry.IsPredator) {
                        var vec = p.gameObject.transform.position - transform.position;
                        vec.y = 0;
                        dist = vec.magnitude;
                        break;
                    }
                }
            }
            else if(mode == Mode.Goal) {
                for(int i = 0; i < 4; ++i) {
                    var p = PlayerManager.Instance.Player(i) as LocalPlayer;
                    if(!p.State.BirdType.BirdDataEntry.IsPredator) {
                        var vec = p.gameObject.transform.position - transform.position;
                        vec.y = 0;
                        var d = vec.magnitude;
                        dist = Mathf.Min(dist, d);
                    }
                }
            }
            else if(mode == Mode.Hawk) {
            }

            var alpha = 1.0f;
            alpha = (dist - FadeNearDist) / (FadeFarDist - FadeNearDist);
                            alpha = Mathf.Clamp(alpha, 0, 1);

            var c = gameObject.GetComponent<MeshRenderer>().material.color;
            c.a = alpha;
            gameObject.GetComponent<MeshRenderer>().material.color = c;
             

        }
    }
}
