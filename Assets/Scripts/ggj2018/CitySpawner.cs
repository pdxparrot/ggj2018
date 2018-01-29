using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawner : MonoBehaviour {

	[System.Serializable] public struct BlockPrefab {
		public GameObject prefab;
		public int frequency;
	}

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

    float RandSign() {
        return (Random.Range(0,1) == 0) ? -1 : 1;
    }

	// Use this for initialization
	void Start () {
		maxFrequency = 0;
		for(int i = 0; i < blockPrefabs.Count; ++i)
			maxFrequency += blockPrefabs[i].frequency;

        int minSize = pigeonSpawnRange.x;
        int maxSize =  pigeonSpawnRange.y;
        pigeonStart = new Vector3(
            Mathf.Min(Random.Range(minSize, maxSize), citySize.x) * RandSign() * blockDimensions,
            0.0f,
            Mathf.Min(Random.Range(minSize, maxSize), citySize.y) * RandSign()) * blockDimensions;

        minSize = hawkSpawnRange.x;
        maxSize =  hawkSpawnRange.y;
        hawkStart = new Vector3(
            Mathf.Clamp(pigeonStart.x + Random.Range(minSize, maxSize), -citySize.x, citySize.x) *
            blockDimensions,
            0.0f,
            Mathf.Clamp(pigeonStart.y + Random.Range(minSize, maxSize), -citySize.y, citySize.y))
            * blockDimensions;

        goalPos = -pigeonStart;

		Spawn();
	}

	// Random block spawning
	private float maxFrequency = 0;
	private GameObject RandomBlock() {
		var rnd = Random.Range(0, maxFrequency - 1);
		int i = 0;

		for(; i < blockPrefabs.Count; ++i) {
			rnd -= blockPrefabs[i].frequency;
			if(rnd < 0)
				break;
		}

		return blockPrefabs[i].prefab;
	}

	// Spawn
	public void Spawn() {
        Destroy(_root);
        _root = null;

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

                Spawn(RandomBlock(), pos);
			}
		}

        Spawn(pigeonSpawnPrefab, pigeonStart);
        Spawn(hawkSpawnPrefab, hawkStart);
        Spawn(goalPrefab, goalPos);
	} 

    private GameObject Spawn(GameObject p, Vector3 pos) {
        var block = Instantiate(p) as GameObject;
        block.transform.parent = _root.transform;
        block.transform.position = pos;
        return block;
    }
}
