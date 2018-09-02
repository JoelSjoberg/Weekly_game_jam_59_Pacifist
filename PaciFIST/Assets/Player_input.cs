using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_input : MonoBehaviour {

    public GameObject super_button;

    public bool attacking = false;
    public bool idle = true;
    public bool vulnerable = true;

    public int[] absorbs_to_next = { 3, 6, 10, 13, 15, 23, 32, 34, 38, 41, 45, 100};

    public int absorbs = 0;
    public int[] absorbs_to_super = { 10, 10, 10, 10, 10, 10, 10, 9, 8, 7, 4, 3};
    public int kyu = 0;
    public int points = 0; // incremented in enemy_behaviour

    public int[] hp_s = { 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 10 };

    public int curr_hp = 0;
    public float attack_speed = 0.2f;
    // cooldown between attacks
    public float cooldown = 3f;

    public float[] a_speeds = new float[12];
    public float[] coolds = new float[12];

    Rigidbody rb;
    enemy_pool ep;
    cam_controll cam;
    SpriteRenderer sr;
    Animator ani;
    AudioSource audio;

    public List<AudioClip> sfx;

    public int combo = 0;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        ep = FindObjectOfType<enemy_pool>();
        cam = FindObjectOfType<cam_controll>();
        //Physics.gravity = new Vector3(0, -5, 0); // affects falling speed
        ani = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        set_animator_bools(false, false);
        super_button.SetActive(false);
        audio = GetComponent<AudioSource>();

        kyu = 0;
        curr_hp = hp_s[kyu];
    }
	
	// Update is called once per frame
	void Update () {

        if(points >= absorbs_to_next[kyu] && kyu < absorbs_to_next.Length - 1)
        {
            kyu++;
            ep.wave++;
            ep.expand_target_boxes(3 + (int)(kyu/2));
            StartCoroutine(ep.pause(4f));
            points = 0;
            cam.small_shake();
            cam.zoom(1);
            curr_hp = hp_s[kyu];
        }

        if(Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (!attacking && idle)
        {
            
            if(Input.GetKeyDown(KeyCode.W))
            {
                attack_in_direction(Vector3.up, false, true);

            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                
                sr.flipX = false;
                attack_in_direction(Vector3.left, true, false);
                
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                sr.flipX = true;
                attack_in_direction(Vector3.right, true, false);
               
            }

            if(Input.GetKeyDown(KeyCode.Space) && absorbs_to_super[kyu] <= absorbs)
            {
                attacking = true;
                excecute_super();
            }
        }


        // Check if the player can do anything
        // print(find_available_boxes());
    }
    public void add_points(int p)
    {
        points += p;
        cam.satis_shake();
        absorbs += p;
        combo++;
        if (absorbs >= absorbs_to_super[kyu]) super_button.SetActive(true);
    }
    public void take_damage()
    {
        points -= 1;
        if(points < 0 )
        {
            points = 0;
            
        }
        cam.bad_shake();
        curr_hp--;

        if(curr_hp <= 0)
        {
            StartCoroutine(game_over());
        }
        combo = 0;
    }

    void attack_in_direction(Vector3 dir, bool anim_bool1, bool anim_bool2)
    {

        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "target_box")
            {
                audio.clip = sfx[0];
                audio.Play();
                set_animator_bools(anim_bool1, anim_bool2);
                attacking = true;
                idle = false;
                vulnerable = false;
                StartCoroutine(start_attack_timer(a_speeds[kyu], hit.collider.transform.position));
                StartCoroutine(enforce_attack_limit(coolds[kyu]));
            }
        }
    }

    void excecute_super()
    {
        points += (int)kyu / 2;
        set_animator_bools(true, true);
        absorbs = 0;
        audio.clip = sfx[1];
        audio.Play();
        super_button.SetActive(false);
        StartCoroutine(super(100));
        

    }

    Collider cast_in_dir(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity))
        {
            return hit.collider;

        }
        return null;
    }

    bool find_available_boxes()
    {
        Vector3[] dirs = { Vector3.up, Vector3.down, Vector3.left, Vector3.right};

        foreach(Vector3 d in dirs)
        {
            Collider c = cast_in_dir(d);

            if (c != null && c.tag == "target_box")
            {
                return true;
            }
        }
        return false;
    }
    void set_animator_bools(bool b1, bool b2)
    {
        ani.SetBool("moving", b1);
        ani.SetBool("attacking", b2);
    }

    IEnumerator start_attack_timer(float speed, Vector3 pos)
    {
        float counter = 0.0f;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        while (transform.position != pos)
        {
            counter += Time.fixedDeltaTime * a_speeds[kyu];
            transform.position = Vector3.Lerp(transform.position, pos, counter);
            yield return new WaitForFixedUpdate();  
        }

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        attacking = false;
        vulnerable = true;
        set_animator_bools(false, false);
        
    }

    // for for duration before able to attack again
    IEnumerator enforce_attack_limit(float duration)
    {
        yield return new WaitForSeconds(duration);
        idle = true;

    }

    IEnumerator super(float speed)
    {
        curr_hp++;
        attacking = true;
        float counter = 0.0f;
        Vector3 target = new Vector3(transform.position.x, 0, transform.position.z);
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        while (transform.position != target)
        {
            counter += Time.fixedDeltaTime * a_speeds[kyu];
            transform.position = Vector3.Lerp(transform.position, target, counter);
            yield return new WaitForFixedUpdate();
        }

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        cam.super_shake();
        ep.set_ki_inactive();
        ep.expand_target_boxes(1 + (int)(kyu / 2));

        attacking = false;
        vulnerable = true;
        
        set_animator_bools(false, false);

    }

    IEnumerator game_over()
    {
        audio.clip = sfx[2];
        audio.Play();

        ani.Play("player_die");
        yield return new WaitForSeconds(1.5f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}