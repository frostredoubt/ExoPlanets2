using UnityEngine;
using System.Collections;

public class Enemy_spawner : MonoBehaviour {

    private float Last_spawn = 0;

    public float Spawn_interval_seconds = 3;
    public GameObject Enemy_type;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Last_spawn + Spawn_interval_seconds < Time.time)
        {
            Spawn_enemy();
            Last_spawn = Time.time;
        }
	}

    void Spawn_enemy()
    {
        Vector3 pos = GameObject.Find("Spawner").transform.position;
        Instantiate(Enemy_type, pos, Quaternion.identity);
    }
}
