using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_input : MonoBehaviour {

    bool attacking = false;
    bool idle = true;

    // cooldown between attacks
    float attack_time = 0.5f;

    // hitboxes beside player
    public Transform boxes;
    Vector3[] hit_boxes = new Vector3[3];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < boxes.GetChildCount(); i++)
        {
            hit_boxes[i] = boxes.GetChild(i).position;
        }
	}
	
	// Update is called once per frame
	void Update () {

        if(!attacking)
        {
            if(Input.GetKeyDown(KeyCode.W))
            {
                attacking = true;
                StartCoroutine(start_attack_timer(attack_time, transform.position));
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                attacking = true;
                StartCoroutine(start_attack_timer(attack_time, hit_boxes[1]));
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                attacking = true;
                StartCoroutine(start_attack_timer(attack_time, hit_boxes[2]));
            }
        }
    }

    IEnumerator start_attack_timer(float dur, Vector3 pos)
    {
        transform.position = Vector3.Slerp(transform.position, pos, dur);
        yield return new WaitForSeconds(dur);
        attacking = false;
    }
}
