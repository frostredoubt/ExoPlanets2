using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour {

	public GameObject[] monsters;
	public float chance;

	// Use this for initialization
	void Awake () {
		if (Random.value < chance) {
			Transform parentTransform = this.transform.parent;

			GameObject prefab = monsters[Random.Range(0, monsters.Length)];

			GameObject replacement = Instantiate (prefab, this.transform.position, this.transform.rotation) as GameObject;        
			replacement.transform.parent = parentTransform;
		}
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
