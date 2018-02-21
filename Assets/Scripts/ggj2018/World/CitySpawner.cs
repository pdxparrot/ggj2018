using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2018.World
{
    public class CitySpawner : MonoBehavior
    {
	    [Serializable]
        public struct BlockPrefab
        {
		    public GameObject prefab;
		    public int frequency;
	    }

        [SerializeField] private bool generateOnAwake = true;

        [SerializeField] private GameObject hawkSpawnPrefab;
        [SerializeField] private GameObject pigeonSpawnPrefab;
        [SerializeField] private GameObject goalPrefab;

	    [SerializeField] private List<BlockPrefab> blockPrefabs;
	    [SerializeField] private int blockDimensions;
	    [SerializeField] private Vector2Int citySize;

        [SerializeField] private Vector2Int pigeonSpawnRange;
        [SerializeField] private Vector2Int hawkSpawnRange;

        private GameObject _root = null;
        private Vector3 pigeonStart;
        private Vector3 hawkStart;
        private Vector3 goalPos;

        private readonly System.Random random = new System.Random();

        // Random block spawning
        private float maxFrequency;

    #region Unity Lifecycle
	    private void Start ()
        {
            if(generateOnAwake) {
                Generate(false);
            }
	    }
    #endregion

        public void Generate(bool fromEditorGenerate)
        {
            maxFrequency = 0.0f;
            for(int i = 0; i < blockPrefabs.Count; ++i) {
                maxFrequency += blockPrefabs[i].frequency;
            }

            int minSize = pigeonSpawnRange.x;
            int maxSize =  pigeonSpawnRange.y;
            pigeonStart = new Vector3(
                Mathf.Min(random.Next(minSize, maxSize), citySize.x) * random.NextSign(),
                0.0f,
                Mathf.Min(random.Next(minSize, maxSize), citySize.y) * random.NextSign());

            minSize = hawkSpawnRange.x;
            maxSize =  hawkSpawnRange.y;
            hawkStart = new Vector3(
                Mathf.Clamp(pigeonStart.x + random.Next(minSize, maxSize), -citySize.x, citySize.x),
                0.0f,
                Mathf.Clamp(pigeonStart.y + random.Next(minSize, maxSize), -citySize.y, citySize.y));

            pigeonStart.x *= blockDimensions;
            pigeonStart.z *= blockDimensions;
            hawkStart.x *= blockDimensions;
            hawkStart.z *= blockDimensions;
            goalPos = -pigeonStart;

            Spawn(fromEditorGenerate);
        }

	    private GameObject RandomBlock()
        {
		    var rnd = random.NextDouble(0, maxFrequency - 1.0f);
		    int i = 0;

		    for(; i < blockPrefabs.Count; ++i) {
			    rnd -= blockPrefabs[i].frequency;
			    if(rnd < 0.0f)
				    break;
		    }

		    return blockPrefabs[i].prefab;
	    }

	    // Spawn
        private void Spawn(bool fromEditorGenerate)
        {
            DestroyRoot(fromEditorGenerate);

		    Vector2Int max = new Vector2Int(citySize.x, citySize.y);
		    Vector2Int min = new Vector2Int(-max.x, -max.y);

            _root = new GameObject("City");

		    for(int x = min.x; x <= max.x; ++x) {
			    for(int y = min.y; y <= max.y; ++y) {
                    Vector3 pos = new Vector3(x * blockDimensions, 0,
                                              y * blockDimensions);

                    // -- if it's any of the spawnpoints, no building here
                    float dist = 999.0f;
                    dist = Mathf.Min(dist, (pos - pigeonStart).magnitude);
                    dist = Mathf.Min(dist, (pos - hawkStart).magnitude);
                    dist = Mathf.Min(dist, (pos - goalPos).magnitude);
                    if(dist < 10.0f) {
                        continue;
                    }

                    Spawn(RandomBlock(), pos, fromEditorGenerate);
			    }
		    }

            Spawn(pigeonSpawnPrefab, pigeonStart, fromEditorGenerate);
            Spawn(hawkSpawnPrefab, hawkStart, fromEditorGenerate);
            Spawn(goalPrefab, goalPos, fromEditorGenerate);
	    } 

        private GameObject Spawn(GameObject p, Vector3 pos, bool fromEditorGenerate)
        {
    #if UNITY_EDITOR
            var block = fromEditorGenerate ? UnityEditor.PrefabUtility.InstantiatePrefab(p) as GameObject : Instantiate(p);
    #else
            var block = Instantiate(p);
    #endif
            block.transform.parent = _root.transform;
            block.transform.position = pos;
            return block;
        }

        private void DestroyRoot(bool fromEditorGenerate)
        {
            DestroyRoot(_root, fromEditorGenerate);
            _root = null;

            GameObject danglingRoot = GameObject.Find("City");
            DestroyRoot(danglingRoot, fromEditorGenerate);
        }

        private void DestroyRoot(GameObject root, bool fromEditorGenerate)
        {
            if(fromEditorGenerate) {
                DestroyImmediate(root);
            } else {
                Destroy(root);
            }
        }
    }
}
