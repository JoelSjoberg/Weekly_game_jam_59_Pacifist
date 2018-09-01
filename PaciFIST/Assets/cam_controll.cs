using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_controll : MonoBehaviour {

    Vector3 anchor;
    Camera cam;
    float shake_intensity = 0.1f;
    public float zoom_amount;
	// Use this for initialization
	void Start () {
        anchor = transform.position;
        cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void satis_shake()
    {
        StartCoroutine(shake(0.1f, shake_intensity * 0.75f));
    }

    public void bad_shake()
    {
        StartCoroutine(shake(0.5f, shake_intensity * 3));
    }

    public void small_shake()
    {
        StartCoroutine(shake(0.2f, shake_intensity));
    }
    public void super_shake()
    {
        StartCoroutine(shake(1f, shake_intensity * 3f));
    }

    public void zoom(float mult)
    {
        StartCoroutine(zoom_out(zoom_amount * mult));
    }

    IEnumerator shake(float t, float intensity)
    {
        float timer = 0.0f;
        while(timer < t)
        {
            timer += Time.deltaTime;
            Vector3 s = anchor + Random.insideUnitSphere * intensity;
            s.z = anchor.z;
            transform.localPosition = s;
            yield return new WaitForFixedUpdate();
        }
        transform.position = anchor;
    }

    IEnumerator zoom_out(float amount)
    {
        float original_size = cam.orthographicSize;
        while (cam.orthographicSize < original_size + amount)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, amount + original_size, Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

    }
}
