using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

    private bool Is_fire = false;
    private Vector2 attack_vector;

    public float Deadzone_x = (float)0.2;
    public float Deadzone_y = (float)0.2;

	// Use this for initialization
	void Start () {
        attack_vector = new Vector2();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Get_input();
        Do_attack();
	}

    void Get_input()
    {
        float attack_x = Input.GetAxis("Attack_direction_x");
        float attack_y = Input.GetAxis("Attack_direction_y");
        attack_vector.Set(attack_x, attack_y);

        float attack_trigger = Input.GetAxis("Fire1");

        Is_fire = false;

        if (Mathf.Abs(attack_x) > Deadzone_x || Mathf.Abs(attack_y) > Deadzone_y)
        {
            if (attack_trigger < -0.9)
                Is_fire = true;
        }
    }

    void Do_attack()
    {
        float attack_angle;
        Animator animator = GetComponent<Animator>();
        Vector3 lscale = transform.localScale;
        
        lscale.y = 0;
        lscale.z = 0;
        lscale.Normalize();

        Vector3 cross = Vector3.Cross(attack_vector.normalized, Vector2.right);
        attack_angle = Vector2.Angle(attack_vector.normalized, Vector2.right);
        if( lscale.x < 0 )
            attack_angle = Vector2.Angle(attack_vector.normalized, -Vector2.right);
        if (cross.z > 0)
            attack_angle = 360 - attack_angle;

        Debug.Log(attack_angle);

        if (Is_fire)
        {
            Debug.Log("fire");
            if ((45 < attack_angle) && (attack_angle <= 110))
                animator.SetBool("up_attack", true);
            else if ((250 < attack_angle) && (attack_angle < 325))
                animator.SetBool("down_attack", true);
            else if ((325 < attack_angle) || (attack_angle <= 45))
                animator.SetBool("do_attack", true);
        }
        else
        {
            animator.SetBool("do_attack", false);
            animator.SetBool("up_attack", false);
            animator.SetBool("down_attack", false);
        }
    }
}
