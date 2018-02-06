using ggj2018.Core.Util;
using UnityEngine;

namespace ggj2018.ggj2018.Testing
{
    public class TestCitySpawner : MonoBehavior
    {
        [SerializeField]
        private GameObject _buildingPrefab;

        [SerializeField]
        private int _width = 100;

        [SerializeField]
        private int _depth = 100;

        [SerializeField]
        private float _minHeight = 2.0f;

        [SerializeField]
        private float _maxHeight = 10.0f;

        [SerializeField]
        private float _padding = 0.5f;

        [SerializeField]
        private float _buildingScale = 1.0f;

        [SerializeField]
        private GameObject _ground;

        private GameObject _container;

#region Unity Lifecycle
        private void Awake()
        {
            _container = new GameObject("City");

            SetColor(_ground, Color.black);
        }

        private void Start()
        {
            System.Random random = new System.Random();

            float padding = _padding * _buildingScale;

            float sz=0.0f;
            for(int z=0; z<_depth; ++z) {
                float sx = 0.0f;
                for(int x=0; x<_width; ++x) {
                    GameObject go = Instantiate(_buildingPrefab, _container.transform);
                    SetColor(go, Color.green);

                    float height = random.NextSingle(_minHeight, _maxHeight);
                    go.transform.localScale = new Vector3(_buildingScale, height, _buildingScale);

                    go.transform.position = new Vector3(sx, height / 2.0f, sz);

                    sx += _buildingScale + padding;
                }
                sz +=_buildingScale + padding;
            }
        }
#endregion

        private void SetColor(GameObject go, Color color)
        {
            Renderer r = go.GetComponent<Renderer>();
            r.material.color = color;
        }
    }
}
