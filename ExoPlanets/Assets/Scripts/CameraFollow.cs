using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	public float deadZone;
	public float followSpeed;
	public float heightOffset;
	public float horizontalOffset;


	private Transform player;		// Reference to the player's transform.
	
	
	void Awake ()
	{
		// Setting up the reference.
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	
	void FixedUpdate ()
	{
		TrackPlayer();
	}
	
	
	void TrackPlayer ()
	{
		if ((player.transform.position - transform.position).magnitude > deadZone) {

			Move playerMove = player.gameObject.GetComponent<Move>();
			float horizontalOffsetDirection = playerMove.facing == Move.FacingDirections.LEFT ? -1 : 1;

			// By default the target x and y coordinates of the camera are it's current x and y coordinates.
			Vector3 target = new Vector3(player.position.x + horizontalOffset * horizontalOffsetDirection,
			                             player.position.y + heightOffset, transform.position.z);

			// The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
			Vector3 new_position = Vector3.Lerp(transform.position, target, followSpeed);

			// Set the camera's position to the target position with the same z component.
			transform.position = new_position;
		}
	}
}

