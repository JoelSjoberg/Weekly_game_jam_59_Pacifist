using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_behavior : MonoBehaviour {

    public Vector3 dir;
    public float speed;
    bool dead;

    public Animator ani;
    SpriteRenderer sr;

    private void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!dead)
        {
            transform.position += dir * speed * Time.deltaTime;
            ani.Play("ki_alive");

            if (dir.x < 0) sr.flipX = false;
            else sr.flipX = true;
        }
        else
        {
            ani.Play("ki_peace");
        }
	}
    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        ani.Play("ki_spawn");
        dead = false;
        //StartCoroutine(spawn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LIMIT")
        {
            gameObject.SetActive(false);
        }
        
        if (other.tag == "Player" && !dead)
        {
            dead = true;
            // deal damage or take damage, depending on if player is attacking or not
            Player_input p = other.transform.GetComponent<Player_input>();
            if(p.attacking)
            {
                p.add_points((int) transform.position.y + 1);
                StartCoroutine(die());
            }
            
            else
            {
                p.take_damage();
                StartCoroutine(die());
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player" && !dead)
        {
            dead = true;
            // deal damage or take damage, depending on if player is attacking or not
            Player_input p = other.transform.GetComponent<Player_input>();
            if (p.attacking)
            {
                this.ani.Play("ki_peace");
                p.add_points((int)transform.position.y + 1);
                StartCoroutine(die());
            }

            else
            {
                ani.Play("ki_discord");
                p.take_damage();
                StartCoroutine(die());
            }
        }
    }

    public IEnumerator die()
    {
        yield return new WaitForSeconds(0.4f);
        gameObject.SetActive(false);
    }

    IEnumerator spawn()
    {
        ani.Play("ki_spawn");
        yield return new WaitForSeconds(0.4f);
        ani.Play("ki_alive");
    }
}