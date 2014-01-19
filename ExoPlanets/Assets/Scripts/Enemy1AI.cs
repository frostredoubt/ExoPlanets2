using UnityEngine;
using System.Collections;

public class Enemy1AI : MonoBehaviour {

	private GameObject player;
	private Transform groundCheck;
	private float last_jump_time;
	private bool move_away_from_character = false;
	private Animator animation;
	private GameObject beetle;


	public float seconds_between_jumps;
	public float walk_speed;
	public float jump_speed;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		groundCheck = transform.Find("groundCheck");
		beetle = transform.Find ("Beetle").gameObject;
		animation = beetle.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		bool onGround = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

		animation.SetBool ("walk", true);
		animation.SetBool("jump", !onGround);

		if (onGround && (Time.time - last_jump_time) > seconds_between_jumps) {
			Vector3 playerDirection = player.transform.position - this.gameObject.transform.position;
			Vector3 jump = new Vector2(playerDirection.x, Mathf.Abs (playerDirection.x));

			beetle.transform.localScale = new Vector3(Mathf.Abs(beetle.transform.localScale.x) * (jump.x > 0 ? -1 : 1), beetle.transform.localScale.y, beetle.transform.localScale.z);

			gameObject.rigidbody2D.AddForce(jump.normalized * jump_speed);
			last_jump_time = Time.time;
			move_away_from_character = false;
		} else {
			Vector3 playerDirection = player.transform.position - this.gameObject.transform.position;
			Vector3 horizontal = new Vector2(move_away_from_character ? -playerDirection.x : playerDirection.x, 0);

			beetle.transform.localScale = new Vector3(Mathf.Abs(beetle.transform.localScale.x) * (horizontal.x > 0 ? -1 : 1), beetle.transform.localScale.y, beetle.transform.localScale.z);

			gameObject.rigidbody2D.AddForce(horizontal.normalized * walk_speed);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		Vector2 distance = transform.position - coll.gameObject.transform.position;

		if (Mathf.Abs (distance.x) > 0.25 && Mathf.Abs (distance.y) < 0.9) {
			move_away_from_character = !move_away_from_character;
			Debug.Log("hit wall");
		}
		
	}
}
