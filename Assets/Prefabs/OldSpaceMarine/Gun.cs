using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    [SerializeField] private ParticleSystem system;
    public bool play = false;

    void Awake()
    {
        
    }

    void Start()
    {
        system.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (Input.GetButton("Stop"))
            system.Play();

        //if (play)
        //{
        //    system.Play();
        //    play = false;

        //}
    }
}
