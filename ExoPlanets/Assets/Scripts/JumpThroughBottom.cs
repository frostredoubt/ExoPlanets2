﻿using UnityEngine;
using System.Collections;

public class JumpThroughBottom : MonoBehaviour {

	private Collider2D block;

	private bool through = false;

	// Use this for initialization
	void Start () {
		block = this.transform.parent.gameObject.collider2D;
	}
	
	// Update is called once per frame
	void Update () {
		block.enabled = !through;
	}

	void OnTriggerEnter2D(Collider2D coll) {
		Vector3 directionToCollider = block.transform.position - coll.transform.position;
		if (coll.gameObject.tag == "Player" && directionToCollider.y > 0) {

			through = true;
		}
			
		
	}

	void OnTriggerStay2D(Collider2D coll) {
		Vector3 directionToCollider = block.transform.position - coll.transform.position;
		if (coll.gameObject.tag == "Player" && directionToCollider.y > 0)
			through = true;
	}

	void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player")
						through = false;
		
	}
}
