using System.Collections;
using UnityEngine;

public class Player_input : MonoBehaviour {

    public bool attacking = false;
    public bool idle = true;
    public bool vulnerable = true;

    public int[] absorbs_to_next = { 3, 6, 10, 13, 15, 23, 32, 34, 38, 41, 45, 100};
    public int kyu = 0;
    public int points = 0; // incremented in enemy_behaviour

    public float attack_speed = 0.2f;
    // cooldown between attacks
    public float cooldown = 3f;

    public float[] a_speeds = new float[12];

    public float[] coolds = new float[12];

    Rigidbody rb;
    enemy_pool ep;
    cam_controll cam;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        ep = FindObjectOfType<enemy_pool>();
        cam = FindObjectOfType<cam_controll>();
	}
	
	// Update is called once per frame
	void Update () {

        if(points >= absorbs_to_next[kyu] && kyu < absorbs_to_next.Length)
        {
            kyu++;
            ep.wave++;
            ep.set_ki_inactive();
            ep.expand_target_boxes();
            StartCoroutine(ep.pause(3f));
            points = 0;
            cam.small_shake();
            cam.zoom(1);
        }

        if (!attacking && idle)
        {
            
            if(Input.GetKeyDown(KeyCode.W))
            {
                attack_in_direction(Vector3.up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                attack_in_direction(Vector3.left);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                attack_in_direction(Vector3.right);
            }

        }
    }
    public void add_points(int p)
    {
        points += p;
        cam.satis_shake();
    }
    public void take_damage()
    {
        points -= 1;
        if(points < 0 )
        {
            points = 0;
            if(kyu > 0)kyu -= 1;
            
        }
        cam.bad_shake();
    }


    void attack_in_direction(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "target_box")
            {
                attacking = true;
                idle = false;
                vulnerable = false;
                StartCoroutine(start_attack_timer(a_speeds[kyu], hit.collider.transform.position));
                StartCoroutine(enforce_attack_limit(coolds[kyu]));
            }
        }
    }

    IEnumerator start_attack_timer(float speed, Vector3 pos)
    {
        float counter = 0.0f;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        while (transform.position != pos)
        {
            //transform.position += (pos - transform.position) * attack_speed * Time.fixedDeltaTime;
            counter += Time.fixedDeltaTime * a_speeds[kyu];
            transform.position = Vector3.Slerp(transform.position, pos, counter);
            yield return new WaitForFixedUpdate();  
        }

        rb.constraints = RigidbodyConstraints.None;
        rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        attacking = false;
        vulnerable = true;
    }

    // for for duration before able to attack again
    IEnumerator enforce_attack_limit(float duration)
    {
        yield return new WaitForSeconds(duration);
        idle = true;
    }
}
