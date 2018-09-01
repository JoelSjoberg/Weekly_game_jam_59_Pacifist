using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_pool : MonoBehaviour {

    public GameObject ki;
    public GameObject hit_box;
    List<GameObject> pool;
    List<GameObject> hit_box_pool;
    public int pool_size;
    private int[] dirs = { 1, -1 };
    private bool spawning = true;


    float timer = 0f;

    public int wave = 0;

    public float[] speeds = new float[12];
    public float[] freqs = new float[12];

    void Start () {

        pool = new List<GameObject>();
        hit_box_pool = new List<GameObject>();

        for (int i = 0; i < pool_size; i++)
        {
            GameObject ob = (GameObject)Instantiate(ki);

            ob.SetActive(false);
            pool.Add(ob);
            ob = (GameObject)Instantiate(hit_box);
            ob.SetActive(false);
            hit_box_pool.Add(ob);
        }

        // activate hitboxes the player start with
        spawn(Vector3.zero, hit_box_pool);
        spawn(Vector3.left, hit_box_pool);
        spawn(Vector3.right, hit_box_pool);
        spawn(Vector3.up, hit_box_pool);
    }

	// TODO: provide direction of instantiated object
    void spawn_ki()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if(!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                enemy_behavior e = pool[i].GetComponent<enemy_behavior>();


                int dir_multiplyer = dirs[Random.Range(0, 2)];
                e.transform.position = get_spawn_point_ki(dir_multiplyer);
                e.speed = speeds[wave];
                e.dir = Vector3.right * -dir_multiplyer;
                break;
            }
        }
    }

    void spawn(Vector3 pos, List<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeInHierarchy)
            {
                pool[i].SetActive(true);
                pool[i].transform.position = pos;
                break;
            }
        }
    }


    List<GameObject> get_active_boxes()
    {
        List<GameObject> active_boxes = new List<GameObject>();

        for (int i = 0; i < hit_box_pool.Count; i++)
        {
            if (hit_box_pool[i].activeInHierarchy)
            {
                active_boxes.Add(hit_box_pool[i]);
            }
        }
        return active_boxes;
    }

    Vector3 get_spawn_point_ki(int mult)
    {

        List<GameObject> boxes = get_active_boxes();
        Vector3 ret_pos = boxes[Random.Range(0, boxes.Count)].transform.position;
        
        ret_pos.x = 9.5f * mult;

        return (ret_pos);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= freqs[wave] + Random.Range(0, 2) && spawning)
        {
            timer = 0;
            spawn_ki();
        }
    }

    public IEnumerator pause(float t)
    {
        spawning = false;
        yield return new WaitForSeconds(t);
        spawning = true;
    }

    public void set_ki_inactive()
    {
        for (int i = 0; i < pool.Count; i++)
        {

            if (pool[i].activeInHierarchy)
            {
                enemy_behavior e = pool[i].GetComponent<enemy_behavior>();
                e.ani.Play("ki_peace");
                StartCoroutine(e.die());
            }

        }
    }

    public void expand_target_boxes(int amount)
    {
        List<GameObject> boxes = get_active_boxes();
        int boxes_expanded = 0;

        Vector3[] dirs = { Vector3.left, Vector3.right, Vector3.up, Vector3.down};

        for(int i = 0; i < boxes.Count; i++)
        {
            if (boxes_expanded < amount)
            {
                for (int j = 0; j < dirs.Length; j++)
                {
                    // Cast ray in direction to see if something is hit
                    if (cast_ray(boxes[i].transform.position, dirs[j]))
                    {
                        spawn(boxes[i].transform.position + dirs[j], hit_box_pool);
                        boxes_expanded++;
                    }
                }

            }
            else break;
        }
    }

    // return true if nothing is hit
    bool cast_ray(Vector3 from, Vector3 dir)
    {
        RaycastHit hit;
        if (!Physics.Raycast(from, dir, out hit, 0.75f))
        {
            return true;
        }
        else return false;
    }
}
