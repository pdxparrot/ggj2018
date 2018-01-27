using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawner : MonoBehaviour {

	[System.Serializable] public struct BlockPrefab {
		public GameObject prefab;
		public int frequency;
	}

	[SerializeField] private List<BlockPrefab> blockPrefabs;
	[SerializeField] private Vector2 blockDimensions;
	[SerializeField] private Vector2Int citySize;

    private readonly List<GameObject> _tiles = new List<GameObject>();

	// Use this for initialization
	void Start () {
		maxFrequency = 0;
		for(int i = 0; i < blockPrefabs.Count; ++i)
			maxFrequency += blockPrefabs[i].frequency;
        Debug.LogError($"'{maxFrequency}'");

		Spawn();
	}

	// Random block spawning
	private float maxFrequency = 0;
	private GameObject RandomBlock() {
		var rnd = Random.Range(0, maxFrequency - 1);
		int i = 0;

		for(; i < blockPrefabs.Count; ++i) {
	        Debug.LogError($"'{i} {rnd}'");
			rnd -= blockPrefabs[i].frequency;
			if(rnd < 0)
				break;
		}

		return blockPrefabs[i].prefab;
	}

	// Spawn
	public void Spawn() {
		_tiles.Clear();

		Vector2Int max = new Vector2Int(citySize.x / 2, citySize.y / 2);
		Vector2Int min = new Vector2Int(-max.x, -max.y);

		for(int x = min.x; x <= max.x; ++x) {
			for(int y = min.y; y <= max.y; ++y) {
				var block = Instantiate(RandomBlock());
				block.transform.position = new Vector3(x * blockDimensions.x, 0,
													   y * blockDimensions.y);
				_tiles.Add(block);
			}
		}
	} 
}
