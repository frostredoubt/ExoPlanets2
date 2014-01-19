using UnityEngine;
using System.Collections;

public class ProbabilityDestroy : MonoBehaviour {

	public float chanceToDestroy;

	// Use this for initialization
	void Start () {
		if (Random.value < chanceToDestroy) {
			Destroy(this.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
