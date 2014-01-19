using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

    public int H_Force      = 200;
    public int Jump_Force   = 100;
    public float Max_x_velocity = (float)20;
    public CharacterController CC;

    private Vector2 Forces;
    private bool Jump_pressed;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Gather_inputs();
        Correct_speed();
        Apply_forces();

        Debug_output();
	}

    void Correct_speed()
    {
    }

    void Gather_inputs()
    {
        Forces.Set(0, 0);
        float x_input = Input.GetAxis("Horizontal");
        Forces.Set(x_input * H_Force, 0);
        Jump_pressed = Input.GetButton("Jump");
    }

    void Apply_forces()
    {
        float old_x = gameObject.rigidbody2D.velocity.x;
        Vector2 old_v = gameObject.rigidbody2D.velocity;
        Vector2 old_v_x_mag = gameObject.rigidbody2D.velocity;

        old_v_x_mag.Set(old_x, 0);

        if(old_v_x_mag.magnitude < Max_x_velocity)
            gameObject.rigidbody2D.AddForce(Forces);
        else if((new Vector2(old_v.x + Forces.x,0)).magnitude < old_v_x_mag.magnitude)
            gameObject.rigidbody2D.AddForce(Forces);

        // Jump
        Transform ground = transform.Find("Ground_collider");
        bool hit = Physics2D.Linecast(transform.position, ground.position, 1 << LayerMask.NameToLayer("Ground"));
        //Debug.DrawLine(transform.position, ground.position);
        if (Jump_pressed && hit)
        {
            gameObject.rigidbody2D.AddForce(new Vector2(0, Jump_Force));
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Trigger");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision"); 
    }

    void Debug_output()
    {
        //Debug.Log(gameObject.rigidbody2D.velocity);
    }
}
