﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_cube : MonoBehaviour {

    Animator ani;
    bool player_present = false;
    int max_hp = 2;
    int hp = 2;

    AudioSource audio;
	// Use this for initialization

	void Start () {
        ani = gameObject.GetComponent<Animator>();
	}
	

    private void OnEnable()
    {
        ani = gameObject.GetComponent<Animator>();
        audio = gameObject.GetComponent<AudioSource>();
        hp = max_hp;
        ani.SetBool("collided", false);
        ani.Play("Lock_off");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player_present = true;
            ani.SetBool("collided", false);
            if (hp < max_hp)
            {
                audio.Play();
            }
            hp = max_hp;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player_present = false;
            hp = max_hp;
            ani.SetBool("collided", false);
        }

        if (other.tag == "ki")
        {
            if(!player_present)
            {
                ani.SetBool("collided", true);
                hp--;
                if (hp <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
