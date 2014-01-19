using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

    public Transform Held_weapon;

    private bool go_attack = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (go_attack)
        {
            Debug.Log("Stuff");
            Held_weapon.GetComponent<Animator>().SetBool("do_attack", true);
            go_attack = false;
        }
        else
        {
            Held_weapon.GetComponent<Animator>().SetBool("do_attack", false);
        }
	}

    public void Perform_attack()
    {
        go_attack = true;
    }
}
