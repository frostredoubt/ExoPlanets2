using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public enum FacingDirections {
		LEFT, RIGHT
	}

    public int H_Force      = 200;
    public int Jump_Force   = 100;

	public float H_Scale = 1;
	public float Jump_Start_Velocity;
	public float Jump_Decay;
	public int max_jump_frames;
	private bool on_ground;
	private int jumpFrames;
	public FacingDirections facing;

    public float Max_x_velocity = (float)20;
	public float Max_fall_velocity = 10f;
    public CharacterController CC;

    private Vector2 Forces;
    private bool Jump_pressed;
	private bool Jump_down;
    private Vector3 Current_direction;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Gather_inputs();
        Change_displayed_components();
        //Apply_forces();
		Set_Velocity ();

        Debug_output();
	}

    private void Change_displayed_components()
    {
        if (Current_direction.x < 0 && Forces.x > 0)
            Change_character_direction();
        if (Current_direction.x > 0 && Forces.x < 0)
            Change_character_direction();
        if( Forces.x != 0 )
            Current_direction = Forces.normalized;

        Animator animator = GetComponent<Animator>();
        Vector2 old_v = rigidbody2D.velocity;
        old_v.y = 0;
        animator.SetFloat("Velocity", old_v.magnitude);
    }

    void Change_character_direction()
    {
        Vector3 old = transform.localScale;
        Vector2 force_vector = Forces;
        float backup_y = old.y;
        float backup_z = old.z;

        force_vector.y = 0;
        old.y = 0;
        old.z = 0;

        old.x = old.magnitude * force_vector.normalized.x;
        old.y = backup_y;
        old.z = backup_z;
        transform.localScale = old;
    }

    void Gather_inputs()
    {
        Forces.Set(0, 0);
        float x_input = Input.GetAxis("Horizontal");
        Forces.Set(x_input * H_Force, 0);
		Jump_down = Input.GetButtonDown("Jump");
		Jump_pressed = Input.GetButton ("Jump");
    }

	void Set_Velocity()
	{
		Vector2 velocity = gameObject.rigidbody2D.velocity;
		velocity.x = Input.GetAxis("Horizontal") * H_Scale;
		if (Mathf.Abs (Input.GetAxis ("Horizontal")) > 0.05) {
			facing = Input.GetAxis("Horizontal") < 0 ? FacingDirections.LEFT : FacingDirections.RIGHT;
		}

		Transform ground = transform.Find("Ground_collider");
		on_ground = Physics2D.Linecast(transform.position, ground.position, 1 << LayerMask.NameToLayer("Ground"));
		if (Jump_down && on_ground) {
			velocity.y += Jump_Start_Velocity;
			jumpFrames = max_jump_frames;
		}

		if (jumpFrames > 0 && Jump_pressed) {
			velocity.y += Jump_Start_Velocity;
			jumpFrames -= 1;
		}

		if (! on_ground) {
			velocity.y = Mathf.Max(velocity.y - Jump_Decay, -Max_fall_velocity);
		}

		gameObject.rigidbody2D.velocity = velocity;

	}

    void Apply_forces()
    {
        float old_x = gameObject.rigidbody2D.velocity.x;
        Vector2 old_v = gameObject.rigidbody2D.velocity;
        Vector2 old_v_x_mag = gameObject.rigidbody2D.velocity;

        old_v_x_mag.Set(old_x, 0);

        if (old_v_x_mag.magnitude < Max_x_velocity)
        {
            gameObject.rigidbody2D.AddForce(Forces);
        }
        else if ((new Vector2(old_v.x + Forces.x, 0)).magnitude < old_v_x_mag.magnitude)
        {
            gameObject.rigidbody2D.AddForce(Forces);
        }
	

        // Jump
        Transform ground = transform.Find("Ground_collider");
        bool hit = Physics2D.Linecast(transform.position, ground.position, 1 << LayerMask.NameToLayer("Ground"));
        if (Jump_pressed && hit)
        {
            gameObject.rigidbody2D.AddForce(new Vector2(0, Jump_Force));
        }
    }


    void Debug_output()
    {
       
    }
}
