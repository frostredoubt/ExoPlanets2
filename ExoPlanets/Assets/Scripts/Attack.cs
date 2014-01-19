using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {

    private bool Is_fire = false;

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
        if (Is_fire)
            GetComponent<Animator>().SetBool("do_swing", true);
        else
            GetComponent<Animator>().SetBool("do_swing", false);
    }
}
