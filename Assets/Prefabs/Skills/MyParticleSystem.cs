using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyParticleSystem : MonoBehaviour {

    private ParticleSystem system;
    private float t = 0;
    public float speed = 0.1f;

	// Use this for initialization
	void Start () {
        system = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime * speed;

        transform.position = new Vector3(Mathf.Lerp(70, 600, t), transform.position.y, transform.position.z);
        if (Input.GetButtonDown("Stop"))
            t = 0;
	}
}
