using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

    private bool Is_fire = false;
    private Vector2 attack_vector;
    private int cooldown_frames = 0;
    public int cooldown = 20;

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
            if (attack_trigger == 1)
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

        //Debug.Log(attack_angle);

        if (Is_fire && cooldown_frames == 0)
        {
            //Debug.Log("fire");
            Animator a = GetComponent<Animator>();
            if ((45 < attack_angle) && (attack_angle <= 110))
            {
                transform.Find("Attack_cone_up").GetComponent<PolygonCollider2D>().enabled = true;
                animator.SetBool("up_attack", true);
                cooldown_frames = cooldown;
            }
            else if ((250 < attack_angle) && (attack_angle < 325))
            {
                transform.Find("Attack_cone_down").GetComponent<PolygonCollider2D>().enabled = true;
                animator.SetBool("down_attack", true);
                cooldown_frames = cooldown;
            }
            else if ((325 < attack_angle) || (attack_angle <= 45))
            {
                transform.Find("Attack_cone_forward").GetComponent<PolygonCollider2D>().enabled = true;
                animator.SetBool("do_attack", true);
                cooldown_frames = cooldown;
            }
        }
        else
        {
            animator.SetBool("do_attack", false);
            animator.SetBool("up_attack", false);
            animator.SetBool("down_attack", false);
        }
        if (cooldown_frames > 0)
        {
            cooldown_frames -= 1;
            if (cooldown_frames == 0)
            {
                transform.Find("Attack_cone_up").GetComponent<PolygonCollider2D>().enabled = false;
                transform.Find("Attack_cone_down").GetComponent<PolygonCollider2D>().enabled = false;
                transform.Find("Attack_cone_forward").GetComponent<PolygonCollider2D>().enabled = false;
            }
        }
    }
}
