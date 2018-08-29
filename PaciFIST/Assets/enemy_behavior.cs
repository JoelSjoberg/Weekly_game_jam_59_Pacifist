using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_behavior : MonoBehaviour {

    public Vector3 dir;
    public float speed;
	
	// Update is called once per frame
	void Update ()
    {
        transform.position += dir * speed * Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LIMIT")
        {
            gameObject.SetActive(false);
        }
        
        if (other.tag == "Player")
        {
            // deal damage or take damage, depending on if player is attacking or not
            Player_input p = other.transform.GetComponent<Player_input>();
            if(p.attacking)
            {
                gameObject.SetActive(false);
                p.add_points((int) transform.position.y);
                print((int)transform.position.y);
                
            }
            
            else
            {
                p.take_damage();
            }
        }
        
    }
}