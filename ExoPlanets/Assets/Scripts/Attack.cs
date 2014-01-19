using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

    private bool Is_fire = false;
    private bool go_attack = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Get_input();
        Do_attack();
	}

    void Get_input()
    {
        Is_fire = Input.GetButton("Fire1");
    }

    void Do_attack()
    {
        Animator animator = GetComponent<Animator>();
        if (Is_fire)
            animator.SetBool("do_attack", true);
        else
            animator.SetBool("do_attack", false);
    }
}
