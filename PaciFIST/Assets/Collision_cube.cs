using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_cube : MonoBehaviour {

    Animator ani;

	// Use this for initialization
	void Start () {
        ani = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ki")
        {
            ani.SetBool("collided", true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ki")
        {
            ani.SetBool("collided", false);
        }
    }
}
