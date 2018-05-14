using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticleScript : MonoBehaviour {

    //public GameObject prticleSystem;

	// Use this for initialization
	void Start () {

        List<int> t1 = new List<int>();
        List<int> t2 = new List<int>();

        t1.Add(1);
        t1.Add(2);
        t1.Add(3);

        t2.Add(2);
        t2.Add(3);
        t2.Add(4);

        t2.Remove(t1[1]);

        print("t1");
        for (int i = 0; i < t1.Count; i++)
        {
            print("t" + i + " = " + t1[i]);
        }
        print("t2");
        for (int i = 0; i < t2.Count; i++)
        {
            print("t" + i + " = " + t2[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    GetComponent<ParticleSystem>().GetComponent<Rigidbody>().velocity = Vector3.right * 5;
        //}
	}
}
