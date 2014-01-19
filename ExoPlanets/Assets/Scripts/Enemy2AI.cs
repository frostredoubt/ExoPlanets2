using UnityEngine;
using System.Collections;

public class Enemy2AI : MonoBehaviour {

	private GameObject player;
	private Transform groundCheck;
	private float last_charge_time;
	private GameObject sprites;

	private enum State {
		PREPARING,
		CHARGING,
		PACING,
		RECOVERY
	}

	private enum Facing {
		LEFT,
		RIGHT
	}

	private State current_state;
	private Facing facing;

	private int preparing_frames;
	private int recovery_frames;

	public float bounce_force;
	public float walk_force;
	public float charge_target_zone;
	public int max_preparing_frames;
	public int max_recovery_frames;
	public float charge_force;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		groundCheck = transform.Find("groundCheck");
		sprites = transform.Find ("Minotaur").gameObject;
	
	}

	void Update () {
		float direction = facing == Facing.LEFT ? -1 : 1;
		sprites.transform.localScale = new Vector3(Mathf.Abs(sprites.transform.localScale.x) * direction,
		                                           sprites.transform.localScale.y, sprites.transform.localScale.z);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Animator anim = sprites.GetComponent<Animator> ();

		float direction = (facing == Facing.LEFT) ? -1 : 1;

		if (current_state == State.PACING) {

			gameObject.rigidbody2D.AddForce (new Vector2 (direction * walk_force, 0));

			// Chase the player
			if (Mathf.Abs (player.transform.position.y - transform.position.y) < charge_target_zone) {
					current_state = State.PREPARING;
					preparing_frames = max_preparing_frames;
			}
			anim.SetBool("Walk", true);
			anim.SetBool("Charge", false);
		} else if (current_state == State.PREPARING) {
			preparing_frames -= 1;

			Vector3 direction_to_player = transform.position - player.transform.position;
			if (direction_to_player.x > 0) {
					facing = Facing.LEFT;
			} else {
					facing = Facing.RIGHT;
			}

			if (preparing_frames <= 0) {
					current_state = State.CHARGING;
			}
			anim.SetBool("Walk", true);
			anim.SetBool("Charge", false);
		} else if (current_state == State.CHARGING) {
			gameObject.rigidbody2D.AddForce (new Vector2 (direction * charge_force, 0));
			anim.SetBool("Walk", false);
			anim.SetBool("Charge", true);
		} else if (current_state == State.RECOVERY) {
			recovery_frames -= 1;
			if (recovery_frames <= 0) {
					current_state = State.PACING;
			}
			anim.SetBool("Walk", false);
			anim.SetBool("Charge", false);
		}

	}

	void OnCollisionEnter2D(Collision2D coll) {
		Vector2 distance = transform.position - coll.gameObject.transform.position;
		
		if (Mathf.Abs (distance.y) < 0.9) {
			if (current_state == State.CHARGING) {
				float bounce_direction = (facing == Facing.LEFT) ? 1 : 1;
				current_state = State.RECOVERY;
				gameObject.rigidbody2D.AddForce(new Vector2(bounce_direction * bounce_force * 2, bounce_force));
				recovery_frames = max_recovery_frames;
			} else if (current_state == State.PACING) {
				facing = facing == Facing.LEFT ? Facing.RIGHT : Facing.LEFT;
			}	
		}
	}
}
