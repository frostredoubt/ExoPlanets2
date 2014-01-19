using UnityEngine;
using System.Collections;

public class TileSelector : MonoBehaviour {

	public Sprite[] possibilities;

	// Use this for initialization
	void Start () {
		int select = Random.Range (0, possibilities.Length);
		Sprite which = possibilities[select];

		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		sr.sprite = which;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
