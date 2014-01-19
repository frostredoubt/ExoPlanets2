using UnityEngine;
using System.Collections;

public class TileReplaceSelf : MonoBehaviour {

	public GameObject prefab;

	// Use this for initialization
	void Start () {
		GameObject parent = this.transform.parent.gameObject;
		GameObject replacement = Instantiate (prefab, transform.position, transform.rotation) as GameObject;
		replacement.transform.parent = parent.transform;
		Destroy (this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
